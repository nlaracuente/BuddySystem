using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Controls the flow a level
/// </summary>
public class LevelController : Singleton<LevelController>
{
    /// <summary>
    /// A collection of goalTiles to check on their status
    /// </summary>
    List<GoalTile> m_goalTiles;

    /// <summary>
    /// 
    /// </summary>
    bool m_levelCompleteRoutineRunning = false;
    
    /// <summary>
    /// Setup
    /// </summary>
    void Start()
    {
        m_goalTiles = FindObjectsOfType<GoalTile>().ToList();
        StartCoroutine(LevelStartRoutine());

        GameManager.Instance.IsLevelCompleted = false;
        GameManager.Instance.IsGameOver = false;
    }

    /// <summary>
    /// Check for Win/Lose Condition
    /// </summary>
    void Update()
    {
        // No need to check
        if (GameManager.Instance.IsLevelCompleted || GameManager.Instance.IsGamePaused) {
            return;
        }

        // At least one tile is not yet active
        if (m_goalTiles.Select(g => g).Where(t => !t.IsActive).FirstOrDefault() != null) {
            return;
        }

        // Level Completed
        StartCoroutine(LevelCompletedRoutine());
    }

    /// <summary>
    /// Fade the screen in 
    /// Attaches the battery to the current player controlled robot to grant player control
    /// </summary>
    /// <returns></returns>
    IEnumerator LevelStartRoutine()
    {
        // Begin the fade
        AudioClipInfo clipInfo = AudioManager.Instance.GetClipInfo(AudioClipName.RobotPoweredOn);
        SceneFader.Instance.FadeIn(clipInfo.Clip.length);

        // Wait for the fade to finish
        yield return new WaitForSeconds(clipInfo.Clip.length);

        PlayerController.Instance.LevelLoaded();
    }

    /// <summary>
    /// Fade the screen in 
    /// Attaches the battery to the current player controlled robot to grant player control
    /// </summary>
    /// <returns></returns>
    IEnumerator LevelCompletedRoutine()
    {
        GameManager.Instance.IsLevelCompleted = true;
        Time.timeScale = 1f; // just in case

        // Begin the fade
        AudioSource source = AudioManager.Instance.Play2DSound(AudioClipName.LevelCompleted);
        SceneFader.Instance.FadeOut(source.clip.length);

        // Wait for the fade to finish
        yield return new WaitForSeconds(source.clip.length);

        GameManager.Instance.LoadNextLevel();
    }

    /// <summary>
    /// Triggers the game over routine
    /// </summary>
    public void OnBatteryExploded()
    {
        StartCoroutine(GameOverRoutine());
    }

    /// <summary>
    /// Triggers a game over sound and fades out while playing the sound
    /// Notifies the GameManager to releoad the level
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverRoutine()
    {
        GameManager.Instance.IsGameOver = true;
        AudioSource source = AudioManager.Instance.Play2DSound(AudioClipName.GameOver);
        float time = source.clip.length;
        SceneFader.Instance.FadeOut(time);
        yield return new WaitForSeconds(time); // a little longer to allow time to sink in
        GameManager.Instance.ReloadScene();
    }
}
