using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The AudioManager is responsible for the playing/pausing/and resuming of music and sounds
/// Though it is possible to control individual clips as the AudioManager returns them after requesting them to play
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField, Tooltip("Enables/Disables the playing on music by default")]
    bool m_musicEnabled = true;

    /// <summary>
    /// Turns the music aduio source ON/OFF 
    /// </summary>
    public bool MusicEnabled
    {
        get { return m_musicEnabled; }
        set {
            m_musicEnabled = value;
            MusicAudioSource.enabled = value;
        }
    }

    [SerializeField, Tooltip("Enables/Disables the playing on sound fxs by default")]
    bool m_soundFxsEnabled = true;

    /// <summary>
    /// Turns the playing of sounds effects ON/OFF  
    /// When OFF, all currently playing sounds will be stopped
    /// </summary>
    public bool SoundFxsEnabled
    {
        get { return m_soundFxsEnabled; }

        set {
            m_soundFxsEnabled = value;
            m_soundFxSources.ForEach(source => {
                if (source != null && !value) {
                    source.Stop();
                }
            });
        }
    }

    [SerializeField, Range(0f, 1f), Tooltip("Master music volume level")]
    float m_musicVolume = 1f;

    /// <summary>
    /// Returns or sets the current music volume setting
    /// </summary>
    public float MusicVolume
    {
        get { return m_musicVolume; }
        set {

            m_musicVolume = Mathf.Clamp01(value);
            MusicAudioSource.volume = m_musicVolume;
        }
    }

    [SerializeField, Range(0f, 1f), Tooltip("Master sound fxs volume level")]
    float m_soundFxsVolume = 1f;

    /// <summary>
    /// Get/Sets the current sound fx volume
    /// If a sound fx sample clip is available then it plays it as a 2D Sound
    /// </summary>
    public float SoundFxVolume
    {
        get { return m_soundFxsVolume; }
        set {
            m_soundFxsVolume = Mathf.Clamp01(value);

            // A sample clip is available to play
            if (m_soundFxSampleClip != null) {

                // Only play when the previous sample clip is done playing
                if (m_sampleSoundFxClip == null || !m_sampleSoundFxClip.IsPlaying) {
                    GameObject clipGO = new GameObject("SampleFxClip", typeof(SingleShotAudio));
                    m_sampleSoundFxClip = clipGO.GetComponent<SingleShotAudio>();


                    m_sampleSoundFxClip.Play2DSound(m_soundFxSampleClip, m_soundFxsVolume);
                }
            }
        }
    }

    /// <summary>
    /// Current game music clip
    /// If one is assigned when the AudioManager sets up then it autoplays it
    /// </summary>
    [SerializeField, Tooltip("Current music play. Autoplays on setup")]
    AudioClip m_musicClip;

    /// <summary>
    /// A reference to the attached audio source for playing music
    /// </summary>
    AudioSource m_musicAudioSource;
    AudioSource MusicAudioSource
    {
        get {
            if (m_musicAudioSource == null && this != null) {
                m_musicAudioSource = GetComponent<AudioSource>();
                m_musicAudioSource.loop = true;
                m_musicAudioSource.volume = m_musicVolume;
            }

            return m_musicAudioSource;
        }
    }

    /// <summary>
    /// The audio clip to play to show sound fx volume change
    /// </summary>
    [SerializeField, Tooltip("The audio clip to play when changing the sound fx volume")]
    AudioClip m_soundFxSampleClip;

    /// <summary>
    /// A reference to the current fx sample clip playing
    /// This is to ensure we don't playing it too many times
    /// </summary>
    SingleShotAudio m_sampleSoundFxClip;

    /// <summary>
    /// A collection of AudioSources of all currently playing audio clips
    /// </summary>
    List<AudioSource> m_soundFxSources = new List<AudioSource>();

    /// <summary>
    /// A collection of the clips library the AudioManager has access to
    /// </summary>
    [SerializeField, Tooltip("A collection of all the different audio clips that cab be played. Both music and sounds")]
    List<AudioClipInfo> m_clipsLibrary = new List<AudioClipInfo>();

    /// <summary>
    /// A maps audio clip names with the clip info for easy access
    /// </summary>
    Dictionary<AudioClipName, AudioClipInfo> m_clipMapping;

    /// <summary>
    /// Triggers contiguration after the manager is instantiated
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        if(this != null && Instance == this) {
            Configure();
        }
    }

    /// <summary>
    /// Configures the AudioManager 
    /// Creates the audio clip mapping
    /// Sets up the music audio source to play startup music 
    /// </summary>
    void Configure()
    {
        m_clipMapping = m_clipsLibrary.GroupBy(c => c.Name).ToDictionary(k => k.Key, v => v.First());
        SetMusic(m_musicClip);
    }

    /// <summary>
    /// Does not play the sample clip
    /// </summary>
    /// <param name="volume"></param>
    public void SetFxVolumeWithoutDemoClip(float volume)
    {
        m_soundFxsVolume = volume;
    }

    /// <summary>
    /// Sets the current music to play
    /// </summary>
    /// <param name="clipName"></param>
    public void PlayMusic(AudioClipName clipName)
    {
        AudioClip clip = GetClipInfo(clipName).Clip;
        SetMusic(clip);
    }

    /// <summary>
    /// Updates the music audio source to play the given clip
    /// </summary>
    /// <param name="clip"></param>
    void SetMusic(AudioClip clip)
    {
        if (clip != null) {
            MusicAudioSource.Stop();
            MusicAudioSource.clip = clip;
            MusicAudioSource.Play();
        }
    }

    /// <summary>
    /// Returns the AudipClipInfo assocaited with the given clip name
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public AudioClipInfo GetClipInfo(AudioClipName clipName)
    {
        AudioClipInfo info = m_clipMapping.ContainsKey(clipName) ? m_clipMapping[clipName] : null;

        if (info == null) {
            Debug.LogError($"Clip: '{clipName.ToString()}' has not been assigned in the clips library");
        } else if (info.Clip == null) {
            Debug.LogError($"'{clipName.ToString()}' has no AudioClip assigned to it");
        }

        return info;
    }

    /// <summary>
    /// Plays the given clip as 2D sound which means it will be heard equally from all speakers
    /// If audio is set to loop it will only stop when either:
    /// <see cref="PauseSounds"/> or <see cref="StopAll"/> are called 
    /// or when you stop it through the returned AudioSource
    /// </summary>
    /// <param name="clipName">The name of the clip to play</param>
    /// <param name="volume">Modify the default volume of the clip</param>
    /// <param name="pitch">Modify the pitch of the sound</param>
    /// <returns></returns>
    public AudioSource Play2DSound(AudioClipName clipName, float volume = 1f, float pitch = 1f)
    {
        AudioClipInfo info = GetClipInfo(clipName);
        AudioSourceSettings settings = info.Settings;

        // Override settings
        settings.volume = volume * SoundFxVolume;
        settings.pitch = pitch;

        SingleShotAudio fx = CreateNewSoundSource();
        fx.Play2DSound(info.Clip, settings);

        return fx.Source;
    }

    /// <summary>
    /// Plays the given clip as a 3D sound by making the sound originate from the given position
    /// If audio is set to loop it will only stop when either:
    /// <see cref="PauseSounds"/> or <see cref="StopAll"/> are called 
    /// or when you stop it through the returned AudioSource
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="position"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public AudioSource PlaySoundAt(AudioClipName clipName, Vector3 position)
    {
        AudioClipInfo info = GetClipInfo(clipName);
        AudioClip clip = info.Clip;
        SingleShotAudio fx = CreateNewSoundSource();

        info.Settings.volume = SoundFxVolume;
        fx.PlaySoundAt(clip, position, info.Settings);
        AudioSource source = fx.Source;

        return source;
    }

    /// <summary>
    /// Returns a new instance of a SingleShotAudio
    /// AudioSources created are stored in <see cref="m_soundFxSources"/>
    /// </summary>
    /// <returns></returns>
    SingleShotAudio CreateNewSoundSource()
    {
        SingleShotAudio audio = new GameObject("SingleShotAudio", typeof(SingleShotAudio)).GetComponent<SingleShotAudio>();

        // Keeps the hierarchy a little cleaner by putting all spawned audio under the manager
        audio.gameObject.transform.SetParent(transform);

        m_soundFxSources.Add(audio.Source);
        return audio;
    }

    /// <summary>
    /// Pauses the currently playing music
    /// </summary>
    public void PauseMusic()
    {
        ToggleMusic(false);
    }

    /// <summary>
    /// Resume playing music
    /// </summary>
    public void ResumeMusic()
    {
        ToggleMusic(true);
    }

    /// <summary>
    /// Triggers the music to play or pause 
    /// </summary>
    /// <param name="play"></param>
    void ToggleMusic(bool play)
    {
        if (!play) {
            MusicAudioSource.Pause();
        } else {
            MusicAudioSource.UnPause();
        }
    }

    /// <summary>
    /// Triggers all currently playing sounds to stop
    /// </summary>
    public void PauseSounds()
    {
        ToggleSounds(false);
    }

    /// <summary>
    /// Triggers all sounds to resume playing
    /// </summary>
    public void ResumeSounds()
    {
        ToggleSounds(true);
    }

    /// <summary>
    /// Toggles the pausing and playing of sound effect given the state of the "play" boolean
    /// </summary>
    /// <param name="play"></param>
    void ToggleSounds(bool play)
    {
        m_soundFxSources.ForEach(s =>
        {
            if (s != null) {
                if (!play) {
                    s.Pause();
                } else {
                    s.UnPause();
                }
            }
        });
    }

    /// <summary>
    /// Triggers all sounds and music to pause
    /// </summary>
    public void PauseAll()
    {
        PauseMusic();
        PauseSounds();
    }

    /// <summary>
    /// Triggers all soudns and music to resume
    /// </summary>
    public void ResumeAll()
    {
        ResumeMusic();
        ResumeSounds();
    }

    /// <summary>
    /// Forces all sounds and music to stop
    /// Note: 
    ///     Resume will not restart sounds as they are destroyed when they have stopped playing
    ///     If you want to stop the music for a short time and resume it later then use the
    ///     <see cref="PauseAll"/> and <see cref="ResumeAll"/> methods instead
    /// </summary>
    public void StopAll()
    {
        m_soundFxSources.ForEach(s =>
        {
            if (s != null) {
                s.Stop();
            }
        });

        MusicAudioSource.Stop();
    }
}
