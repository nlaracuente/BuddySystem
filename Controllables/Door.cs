using UnityEngine;

/// <summary>
/// Controls the state of a single door (Opened/Closed)
/// </summary>
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour, IButtonControllable
{
    [SerializeField, Tooltip("True: the door is opened")]
    bool m_isOpened;

    /// <summary>
    /// A reference to the animator component
    /// </summary>
    Animator m_animator;

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetBool("IsOpened", m_isOpened);
    }

    /// <summary>
    /// Toggles the current state of the door
    /// </summary>
    public void OnButtonPressed()
    {
        m_isOpened = !m_isOpened;
        m_animator.SetBool("IsOpened", m_isOpened);
    }

    /// <summary>
    /// Toggels the current state of the door
    /// </summary>
    public void OnButtonReleased()
    {
        m_isOpened = !m_isOpened;
        m_animator.SetBool("IsOpened", m_isOpened);
    }
}
