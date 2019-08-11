using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the state of a single conveyor belt (running/not running)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ConveyorBelt : MonoBehaviour, IButtonControllable
{
    [SerializeField, Tooltip("True: the door is running")]
    bool m_isRunning;
    public bool IsRunning
    {
        get { return m_isRunning; }
        set {
            m_isRunning = value;

            if (m_isRunning) {
                Source.Play();
            } else {
                Source.Stop();
            }
        }
    }

    AudioSource m_audioSource;
    AudioSource Source
    {
        get {
            if (m_audioSource == null) {
                m_audioSource = GetComponent<AudioSource>();
            }
            return m_audioSource;
        }
    }

    /// <summary>
    /// The speed at which to animate the conveyor
    /// </summary>
    [SerializeField, Range(.1f, 1f), Tooltip("How fast to move the material when the conveyor is on")]
    float m_scrollSpeed = .25f;

    /// <summary>
    /// The speed at which to move the player
    /// </summary>
    [SerializeField, Range(.25f, 5f), Tooltip("How fast to player moves when on the conveyor belt")]
    float m_moveSpeed = 2.5f;
    public float MoveSpeed { get { return m_moveSpeed; } }

    /// <summary>
    /// A reference to the mesh renderer
    /// </summary>
    MeshRenderer m_renderer;

    /// <summary>
    /// A collection of robots standing on the conveyor belt
    /// </summary>
    List<PlayerRobot> m_robotsOnBelt;

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_robotsOnBelt = new List<PlayerRobot>();
        m_renderer = GetComponentInChildren<MeshRenderer>();
        if (m_renderer == null) {
            Debug.LogError($"Could not load MeshRenderer for {name}");
        }
    }

    /// <summary>
    /// Triggers the Running functionality when it is running
    /// </summary>
    void Update()
    {
        if (IsRunning) {
            RunConveyorBelt();
        }
    }

    /// <summary>
    /// True when the robot is on the conveyor belt
    /// </summary>
    /// <returns></returns>
    public bool RobotOnConveyor(PlayerRobot robot)
    {
        return m_robotsOnBelt.Contains(robot);
    }

    /// <summary>
    /// Animates the texture
    /// Forces robots to be moved    
    /// </summary>
    void RunConveyorBelt()
    {
        float offset = Time.time * m_scrollSpeed;
        m_renderer.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
                
        m_robotsOnBelt.ForEach( r =>
        {
            if(r != null) {
                r.MoveOnConveyorBelt(transform.forward, m_moveSpeed);
            }
        });

        // Move the battery too if its on this conveyor
        if(Battery.Instance.OnConveyorBelt == this) {
            Battery.Instance.MoveOnConveyorBelt(transform.forward, m_moveSpeed);
        }
    }

    /// <summary>
    /// Toggles the current state of the conveyor belt
    /// </summary>
    public void OnButtonPressed()
    {
        IsRunning = !IsRunning;
    }

    /// <summary>
    /// Toggels the current state of the conveyor belt
    /// </summary>
    public void OnButtonReleased()
    {
        IsRunning = !IsRunning;
    }

    /// <summary>
    /// Adds the robot to the list
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerFeet")) {
            PlayerRobot robot = other.gameObject.GetComponentInParent<PlayerRobot>();
            if (robot != null && !m_robotsOnBelt.Contains(robot)) {
                m_robotsOnBelt.Add(robot);
            }
        }
    }

    /// <summary>
    /// Removes robots from the list
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerFeet")) {
            PlayerRobot robot = other.gameObject.GetComponentInParent<PlayerRobot>();
            if (robot != null && m_robotsOnBelt.Contains(robot)) {
                m_robotsOnBelt.Remove(robot);
            }
        }
    }
}
