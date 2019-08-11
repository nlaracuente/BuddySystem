using UnityEngine;

/// <summary>
/// A SingleShotAudio behave very similar to <see href="https://docs.unity3d.com/ScriptReference/AudioSource.PlayClipAtPoint.html">AudioSource.PlayClipAtPoint</see> 
/// with the added behavior that you can get the AudioSource generated, something that the AudioSource does not do, and you can request
/// that the sound be played as 2D for when a position is not required such as UI button presses etc.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SingleShotAudio : MonoBehaviour
{
    /// <summary>
    /// A reference to the audio source
    /// </summary>
    AudioSource m_source;
    public AudioSource Source
    {
        get {
            if (m_source == null) {
                m_source = GetComponent<AudioSource>();
            }

            return m_source;
        }
    }

    /// <summary>
    /// True while the audio source is playing
    /// </summary>
    public bool IsPlaying { get { return Source.isPlaying; } }

    /// <summary>
    /// True once the sound has been triggered to play
    /// </summary>
    bool m_soundPlayed = false;

    /// <summary>
    /// Ensures the audio is destroyed even when Time.timeScale is 0
    /// We could have used clip.length to destroy later but that is affected by Time.timeScale
    /// which means that the audio source will continue to exist in the hierachy and take up resources
    /// until the Time.timescale is no longer 0
    /// </summary>
    void Update()
    {
        if (m_soundPlayed && !Source.isPlaying) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays the given clip at the given position by moving the AudioSource parent object to that location
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="settings"></param>
    public void PlaySoundAt(AudioClip clip, Vector3 position, AudioSourceSettings settings)
    {
        transform.position = position;
        PlaySound(clip, settings);

    }

    /// <summary>
    /// Forces the given clip to have a spatial blend of 0 to make it 2D
    /// This overrides which ever values the given settings has
    /// Note:
    ///     You could set the settings to spatialBlend 0 by default and use the PlaySoundAt,
    ///     be it will always play as 2D, but what this allows you to do is play any sound 
    ///     as a 2D sound even if it was configured to be 3D
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="settings"></param>
    public void Play2DSound(AudioClip clip, AudioSourceSettings settings)
    {
        PlaySound(clip, settings);
    }

    /// <summary>
    /// Plays the given clip as a 2D sound with default audio settings
    /// Use this when you need to quickly play a 2D sound without the need of custom settings
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch">change the pitch level</param>
    public void Play2DSound(AudioClip clip, float volume, float pitch = 1f)
    {
        AudioSourceSettings settings = new AudioSourceSettings()
        {
            volume = volume,
            pitch = pitch
        };
        PlaySound(clip, settings);
    }

    /// <summary>
    /// Setups the audio source to play the given sound at the given volume/spatial blend 
    /// A spatial blen higher than 0 makes the sound 3D with the highest value being 1
    /// </summary>
    /// <param name="clip">Clip to play</param>        
    /// <param name="volume">Volume to play at</param>
    /// <param name="spatialBlend">1f = 3D, 0f = 2D</param>
    /// <param name="loops">Loop sound playback</param>
    void PlaySound(AudioClip clip, AudioSourceSettings settings)
    {
        if (clip == null) {
            Destroy(gameObject);
            return;
        }

        m_soundPlayed = true;

        // Keeps the Hierarchy a little cleaner
        gameObject.name = clip.name + "_AudioSource";

        // Confiure the AudioSource
        Source.clip = clip;
        Source.volume = Mathf.Clamp01(settings.volume);
        Source.loop = settings.loops;
        Source.priority = settings.priority;
        Source.pitch = settings.pitch;
        Source.panStereo = settings.stereoPan;
        Source.reverbZoneMix = settings.reverbZoneMix;
        Source.dopplerLevel = settings.dopplerLevel;
        Source.spatialBlend = Mathf.Clamp01(settings.spatialBlend);
        Source.spread = Mathf.Clamp(settings.spread, 0, 360);
        Source.minDistance = settings.minDistance;
        Source.maxDistance = settings.maxDistance;
        Source.rolloffMode = settings.volumeRolloff;

        Source.Play();
    }
}
