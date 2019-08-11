using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonSoundFx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /// <summary>
    /// Plays hover sound effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play2DSound(AudioClipName.MenuHover);
    }

    /// <summary>
    /// Plays click sound effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Play2DSound(AudioClipName.MenuSelect);
    }
}
