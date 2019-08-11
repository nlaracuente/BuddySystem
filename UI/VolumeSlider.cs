using UnityEngine.UI;

/// <summary>
/// Extends the UI.Slider to allow us to trigger a change in value
/// without trigger the on value change call back
/// </summary>
public class VolumeSlider : Slider
{
    /// <summary>
    /// Changes the value of the slider to the one given
    /// Triggers the OnValueChange callback when the flag is set to true
    /// Flag defaults to false
    /// </summary>
    /// <param name="value"></param>
    /// <param name="callback"></param>
    public void SetValue(float value, bool callback = false)
    {
        Set(value, callback);
    }
}
