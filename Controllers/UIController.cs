using UnityEngine;

/// <summary>
/// Functions as the middle-man between a UI component and the GameManager
/// This is so that in the Editor we can quickly assigned functions to things
/// like button presses or custom UnityEvents without worring about the GM not existing
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// Notifies the GM that the play button has been pressed
    /// </summary>
    public void MainMenuPlayButton()
    {
        GameManager.Instance.OnMainMenuPlayButtonPressed();
    }

    /// <summary>
    /// Notifies the GM to transition to the main menu
    /// </summary>
    public void CreditsSceneMainMenuButton()
    {
        GameManager.Instance.OnCreditsSceneMainMenuButtonPressed();
    }

    /// <summary>
    /// Triggers the GameManager to transition to the given level
    /// </summary>
    /// <param name="level"></param>
    public void GotToLevel(int level)
    {
        GameManager.Instance.PlayLevel(level);
    }
}
