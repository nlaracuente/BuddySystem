using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    /// <summary>
    /// The GO for the pause menu
    /// </summary>
    [SerializeField]
    GameObject m_pauseMenu;

    /// <summary>
    /// Current state of the menu
    /// </summary>
    bool m_isMenuOpened = false;

    /// <summary>
    /// Ensures menu starts closed
    /// </summary>
    void Start()
    {
        CloseMenu();
    }

    /// <summary>
    /// Toggles the menu to open
    /// </summary>
    void LateUpdate()
    {
        // Ignore on level completion sequence
        if (GameManager.Instance.IsGamePaused || GameManager.Instance.IsLevelCompleted) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            m_isMenuOpened = !m_isMenuOpened;
            if (m_isMenuOpened) {
                OpenMenu();
            } else {
                CloseMenu();
            }
        }
    }

    /// <summary>
    /// Closes the menu
    /// </summary>
    void CloseMenu()
    {
        Time.timeScale = 1f;
        if(m_pauseMenu != null) {
            m_pauseMenu.SetActive(false);
        }
        GameManager.Instance.IsGamePaused = false;
    }

    /// <summary>
    /// Opens the menu
    /// </summary>
    void OpenMenu()
    {
        GameManager.Instance.IsGamePaused = true;
        Time.timeScale = 0f;
        if (m_pauseMenu != null) {
            m_pauseMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void OnRestartButtonPressed()
    {
        GameManager.Instance.IsGamePaused = false;
        GameManager.Instance.ReloadScene();
    }

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void OnQuitButtonPressed()
    {
        GameManager.Instance.IsGamePaused = false;
        GameManager.Instance.TransitionToMainMenu();
    }
}
