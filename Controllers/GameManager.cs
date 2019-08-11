using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/// <summary>
/// Manages the state of the game, player progression, scene transitions
/// and has access to any and all components in the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// The name of the scene for the main menu
    /// </summary>
    [SerializeField]
    string m_mainMenuSceneName = "MainMenu";

    /// <summary>
    /// The format for scene level names
    /// </summary>
    [SerializeField]
    string m_levelSceneNamePrefix = "Level_";    

    /// <summary>
    /// The name of the scene for the end credits
    /// </summary>
    [SerializeField]
    string m_creditsSceneName = "Credits";

    /// <summary>
    /// The name of the save file
    /// </summary>
    [SerializeField]
    string m_saveFileName = "gmtk2019.gd";

    /// <summary>
    /// Keeps track of the current level
    /// </summary>
    [SerializeField]
    int m_currentLevel = 0;
    public int CurrentLevel
    {
        get {
            if(m_currentLevel == 0) {
                string sceneNumber = Regex.Match(CurrentSceneName, @"\d+").Value;
                if (!string.IsNullOrEmpty(sceneNumber)) {
                    m_currentLevel = int.Parse(sceneNumber);
                }
            }
            return m_currentLevel;
        }
        private set { m_currentLevel = value; }
    }

    /// <summary>
    /// Returns the current loaded scene's name
    /// </summary>
    string CurrentSceneName
    {
        get {
            return SceneManager.GetActiveScene().name;
        }
    }

    /// <summary>
    /// Where to save the game file data
    /// </summary>
    string SaveFilePath
    {
        get {
            return string.Format("{0}/{1}", Application.persistentDataPath, m_saveFileName);
        }
    }    

    /// <summary>
    /// The container for loading and storing the data to save
    /// </summary>
    SavedData m_savedData = new SavedData();
    public LevelProgress[] AllLevelProgress
    {
        get {

            if (m_savedData == null || m_savedData.Levels == null || m_savedData.Levels.Length < 1) {
                InitialGameLoad();
            }

            return m_savedData.Levels;
        }
    }

    /// <summary>
    /// True when the in game menu is opened
    /// </summary>
    public bool IsGamePaused { get; set; } = false;

    /// <summary>
    /// True when the current level is completed
    /// </summary>
    public bool IsLevelCompleted { get; set; } = false;

    /// <summary>
    /// True during GameOver sequence
    /// </summary>
    public bool IsGameOver { get; set; } = false;

    /// <summary>
    /// True when the game is in a condition that should disable player input
    /// or disable loops like Updates
    /// </summary>
    public bool DisableActions { get { return IsGameOver || IsLevelCompleted || IsGameOver; } }

    /// <summary>
    /// Handles the initial application load
    /// Initializes the AudioManager
    /// Initializes the main MenuController
    /// </summary>
    void InitialGameLoad()
    {
        // Default volumes settings to AudioManager's defaults
        float musicVolume = AudioManager.Instance.MusicVolume;
        float fxVolume = AudioManager.Instance.SoundFxVolume;

        m_savedData = new SavedData();

        // Saved game loaded
        if (LoadSavedGame()) {
            musicVolume = m_savedData.MusicVolume;
            fxVolume = m_savedData.FxVolume;

        // Create new save data 
        } else {

            var totalLevels = 0;

            // Level numbers start at 1 so we skip level 0
            // Note: We have more built scenes than levels (i.e MainMenu/Credits)
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
                string sceneName = $"{m_levelSceneNamePrefix}{i}";

                if (Application.CanStreamedLevelBeLoaded(sceneName)) {
                    totalLevels++;
                }
            }
            m_savedData.SetDefaults(musicVolume, fxVolume, totalLevels);
        }

        AudioManager.Instance.MusicVolume = musicVolume;
        AudioManager.Instance.SetFxVolumeWithoutDemoClip(fxVolume);
    }

    /// <summary>
    /// Triggers the game to start
    /// </summary>
    public void OnMainMenuPlayButtonPressed()
    {
        StartGame();
    }

    /// <summary>
    /// Takes you back to the main menu
    /// </summary>
    public void OnCreditsSceneMainMenuButtonPressed()
    {
        TransitionToMainMenu();
    }

    /// <summary>
    /// True when the level is marked as unlocked
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsLevelUnlocked(int level)
    {
        bool isUnlocked = false;

        if (IsLevelWithinBounds(level)) {
            isUnlocked = AllLevelProgress[level].IsUnlocked;
        }

        return isUnlocked;
    }

    /// <summary>
    /// True if the level is within the level progress bounds
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    bool IsLevelWithinBounds(int level)
    {
        return level > 0 && level < AllLevelProgress.GetLength(0);
    }

    /// <summary>
    /// Resets the level counter to 1 and loads the level
    /// </summary>
    public void StartGame()
    {
        CurrentLevel = 1;
        LoadCurrentLevel();
    }

    /// <summary>
    /// Sets the current level to the one given and transitions to that level
    /// </summary>
    /// <param name="level"></param>
    public void PlayLevel(int level)
    {
        CurrentLevel = level;
        LoadCurrentLevel();
    }

    /// <summary>
    /// Loads any saved file and 
    /// Returns true when the file is loaded
    /// </summary>
    bool LoadSavedGame()
    {
        if (!File.Exists(SaveFilePath)) {
            return false;
        }

        bool isLoaded = false;
        m_savedData = new SavedData();

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SaveFilePath, FileMode.Open);
        SavedData data = formatter.Deserialize(stream) as SavedData;
        stream.Close();

        // File appears to be healthy
        if (data != null && data.Levels != null && data.Levels.Length > 0) {
            m_savedData = data;
        }

        if (m_savedData.Levels != null && m_savedData.Levels.Length > 0) {
            isLoaded = true;
        }

        // If we have levels then we loaded a game
        return isLoaded;
    }

    /// <summary>
    /// Saves the current progress
    /// </summary>
    public void SaveGame()
    {
        m_savedData.MusicVolume = AudioManager.Instance.MusicVolume;
        m_savedData.FxVolume = AudioManager.Instance.SoundFxVolume;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SaveFilePath, FileMode.Create);

        formatter.Serialize(stream, m_savedData);
        stream.Close();
    }

    /// <summary>
    /// Increases the current level counter and triggers a transition to it
    /// When the level cannot be loaded it defaults to the end credits
    /// </summary>
    public void LoadNextLevel()
    {
        // Defaults action to credits screen
        Action transitionTo = TransitionToCredits;

        // Switches to loading the level if it can be loaded
        int nextLevel = CurrentLevel + 1;

        // Unlock the level if it is available
        if (IsLevelWithinBounds(nextLevel)) {
            AllLevelProgress[nextLevel].IsUnlocked = true;
        }

        if (LevelSceneCanBeLoaded(nextLevel)) {
            CurrentLevel = nextLevel;
            transitionTo = LoadCurrentLevel;
        }

        // Quick save before moving to the next level
        SaveGame();
        transitionTo?.Invoke();
    }    

    /// <summary>
    /// True when the given level number is a scene that can be loaded
    /// </summary>
    /// <param name="levelNumber"></param>
    /// <returns></returns>
    bool LevelSceneCanBeLoaded(int levelNumber)
    {
        string levelName = $"{m_levelSceneNamePrefix}{levelNumber}";
        return Application.CanStreamedLevelBeLoaded(levelName);
    }    

    /// <summary>
    /// Loads the credits scene
    /// </summary>
    public void TransitionToCredits()
    {
        TransitionToScene(m_creditsSceneName);
    }

    /// <summary>
    /// Triggers a scene change back to the main menu
    /// </summary>
    public void TransitionToMainMenu()
    {
        TransitionToScene(m_mainMenuSceneName);
    }

    /// <summary>
    /// Loads the current level
    /// </summary>
    public void LoadCurrentLevel()
    {
        string levelName = $"{m_levelSceneNamePrefix}{CurrentLevel}";
        TransitionToScene(levelName);
    }

    /// <summary>
    /// Loads the given scene if it can be loaded
    /// </summary>
    /// <param name="sceneName"></param>
    void TransitionToScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName)) {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        } else {
            Debug.LogErrorFormat("Scene '{0}' cannot be loaded", sceneName);
            // Failsafe
            ReloadScene();
        }
    }

    /// <summary>
    /// Reloads the currently loaded scene
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Terminates the application
    /// Todo: Remove when done debugging
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Save the game before termination
    /// </summary>
    void OnApplicationQuit()
    {
        SaveGame();
    }
}
