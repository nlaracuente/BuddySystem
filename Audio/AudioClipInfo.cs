using UnityEngine;

/// <summary>
/// Maps the name of a clip with an audio clip
/// </summary>
[System.Serializable]
public class AudioClipInfo
{
    [SerializeField, Tooltip("A name to identify the clip by")]
    protected AudioClipName m_clipName;

    [SerializeField, Tooltip("The aduio clip to play")]
    protected AudioClip m_audioClip;

    [SerializeField, Tooltip("Settings to use when playing this clip")]
    protected AudioSourceSettings m_settings;

    public AudioClipName Name { get { return m_clipName; } }
    public AudioClip Clip { get { return m_audioClip; } }
    public AudioSourceSettings Settings { get { return m_settings; } }
}