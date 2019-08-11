using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// The level selection button for each level
/// </summary>
[RequireComponent(typeof(Image), typeof(Button))]
public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /// <summary>
    /// The level number this is
    /// </summary>
    [SerializeField]
    int m_level;

    /// <summary>
    /// The button component
    /// </summary>
    Button m_button;

    /// <summary>
    /// THe text component in the button
    /// </summary>
    Text m_buttonText;

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_button = GetComponent<Button>();
        m_buttonText = m_button.GetComponentInChildren<Text>();
        m_buttonText.text = m_level.ToString();
    }

    /// <summary>
    /// Makes itself interactible if the level is unlocked
    /// </summary>
    void Start()
    {
        m_button.interactable = GameManager.Instance.IsLevelUnlocked(m_level);
        if (!m_button.interactable) {
            m_buttonText.text = "";
        }
    }

    /// <summary>
    /// Triggers this level to be played
    /// </summary>
    public void OnButtonPressed()
    {
        GameManager.Instance.PlayLevel(m_level);
    }

    /// <summary>
    /// Plays hover sound effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_button.interactable) {
            AudioManager.Instance.Play2DSound(AudioClipName.MenuHover);
        }
    }

    /// <summary>
    /// Plays click sound effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_button.interactable) {
            AudioManager.Instance.Play2DSound(AudioClipName.MenuSelect);
        }
    }
}
