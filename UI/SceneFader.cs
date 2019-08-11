using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles fading the entire screen in/out
/// </summary>
[RequireComponent(typeof(Image))]
public class SceneFader : Singleton<SceneFader>
{
    /// <summary>
    /// The UI image that sits on top of the entire scene to hide/reveal it
    /// </summary>
    [SerializeField]
    Image m_faderImage;
    Image FaderImage
    {
        get {
            if(m_faderImage == null) {
                m_faderImage = GetComponent<Image>();
            }
            return m_faderImage;
        }
    }

    /// <summary>
    /// Fades the screen to reveal scene
    /// </summary>
    /// <param name="time"></param>
    public void FadeOut(float time = 1f)
    {
        StartCoroutine(FadeRoutine(0f, 1f, time));
    }

    /// <summary>
    /// Fades the screen to hide scene
    /// </summary>
    /// <param name="time"></param>
    public void FadeIn(float time = 1f)
    {
        StartCoroutine(FadeRoutine(1f, 0f, time));
    }

    /// <summary>
    /// Sets the fader's image's alpha to the starting value
    /// Triggers a cross fade alpha
    /// Waits until the alpha is done.
    /// </summary>
    /// <param name="start">starting alpha</param>
    /// <param name="end">target alpha</param>
    /// <returns></returns>
    public IEnumerator FadeRoutine(float start, float end, float speed = 1f)
    {
        Color cColor = FaderImage.color;
        Color nColor = new Color(cColor.r, cColor.g, cColor.b, start);
        FaderImage.color = nColor;

        // Wait before we start fading to show the start alpha
        yield return new WaitForSeconds(.1f);

        // Not working correctly with fade ins (switching to old school way)
        // FaderImage.CrossFadeAlpha(end, time, false);

        float increment = (end - start / speed) * Time.deltaTime;
        float current = FaderImage.color.a;

        while (!Mathf.Approximately(current, end)) {
            yield return new WaitForEndOfFrame();
            current = Mathf.Clamp01(current + increment);
            Color newColor = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, current);
            FaderImage.color = newColor;
        }

        Color finalColor = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, end);
        FaderImage.color = finalColor;

        yield return new WaitForSeconds(speed);
    }
}
