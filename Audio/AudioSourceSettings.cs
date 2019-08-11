using System;
using UnityEngine;

/// <summary>
/// Configuration settings for when playing a 2D sound
/// For more information visit:
/// <see href="https://docs.unity3d.com/Manual/class-AudioSource.html">AudioSource Info Page</see>
/// </summary>
[Serializable]
public class AudioSourceSettings
{
    /// <summary>
    /// How loud the sound is at a distance of one world unit (one meter) from the Audio Listener
    /// Note: 
    ///     The Master Volume in AudioManager affects this 
    /// </summary>
    [Range(0f, 1f), Tooltip("How load the sound is from the audio listener")]
    public float volume = 1f;

    /// <summary>
    /// True: causes the audio to play in a loop
    /// </summary>
    [Tooltip("Enable this to make the clip loop when it reaches the end")]
    public bool loops = false;

    /// <summary>
    /// Determines the priority of this audio source among all the ones that coexist in the scene. 
    /// (Priority: 0 = most important. 256 = least important. Default = 128.)
    /// Use 0 for music tracks to avoid it getting occasionally swapped out
    /// </summary>
    [Range(0, 256), Tooltip("Sound Priorty over other sounds.\n0 = most important. 256 = least important. Default = 128")]
    public int priority = 128;

    /// <summary>
    /// Amount of change in pitch due to slowdown/speed up of the Audio Clip. Value 1 is normal playback speed
    /// </summary>
    [Range(-3f, 3f), Tooltip("How slow/fast to play the clip. 1 is normal speed")]
    public float pitch = 1f;

    /// <summary>
    /// Sets the position in the stereo field of 2D sounds
    /// </summary>
    [Range(-1f, 1f), Tooltip("-1 Left speaker. 1 Right speaker")]
    public float stereoPan = 0f;

    /// <summary>
    /// Sets the amount of the output signal that gets routed to the reverb zones. 
    /// The amount is linear in the (0 - 1) range, but allows for a 10 dB amplification in the (1 - 1.1) range 
    /// which can be useful to achieve the effect of near-field and distant sounds
    /// </summary>
    [Range(-0f, 1.1f), Tooltip("Amount of the output signal that gets routed to the reverb zones")]
    public float reverbZoneMix = 1f;

    /// <summary>
    /// Determines how much doppler effect will be applied to this audio source
    /// (if is set to 0, then no effect is applied)
    /// </summary>
    [Range(0f, 5f), Tooltip("How much doppler effect to apply. 0 = no effect")]
    public float dopplerLevel = 1f;

    /// <summary>
    /// Determines how much 2D effect the sound will play as.
    /// When using custom custom rolloff this option is controlled by the curved
    /// </summary>
    [Range(0, 1f), Tooltip("Make the sound 2D, 3D, or a mixture. 0 = 2d, 1 = 3D")]
    public float spatialBlend = 0;

    /// <summary>
    /// Sets the spread angle to 3D stereo or multichannel sound in speaker space.
    /// </summary>
    [Range(0f, 360f), Tooltip("Sets the spread angle to 3D stereo or multichannel sound in speaker space")]
    public float spread = 0f;

    /// <summary>
    /// Within the MinDistance, the sound will stay at loudest possible. 
    /// Outside MinDistance it will begin to attenuate. 
    /// Increase the MinDistance of a sound to make it ‘louder’ in a 3d world, 
    /// and decrease it to make it ‘quieter’ in a 3d world.
    /// </summary>
    [Tooltip("Minimum distance the audio listerner needs to be to hear the sound the loudest")]
    public float minDistance = 1f;

    /// <summary>
    /// The distance where the sound stops attenuating at. 
    /// Beyond this point it will stay at the volume it would be at MaxDistance 
    /// units from the listener and will not attenuate any more.
    /// </summary>
    [Tooltip("Maximum distance from the audio listener before the sound level remains the same")]
    public float maxDistance = 500f;

    /// <summary>
    /// How fast the sound fades. 
    /// The higher the value, the closer the Listener has to be before hearing the sound. 
    /// (This is determined by a Graph)
    /// 
    ///     - Logarithmic Rolloff: The sound is loud when you are close to the audio source, 
    ///                            but when you get away from the object it decreases significantly fast.
    ///     - Linear Rolloff:      The further away from the audio source you go, the less you can hear it.
    ///     - Custom Rolloff:      The sound from the audio source behaves accordingly to how you set the graph of roll offs.
    ///     
    /// Note:
    ///     For "custom rolloff" it is best to create your own audiosource to manipulate the curve and use that
    ///     to trigger sounds
    /// </summary>
    [Tooltip("How fast the sounds fades")]
    public AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;
}