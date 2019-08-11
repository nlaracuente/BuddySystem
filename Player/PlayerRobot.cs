using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a single robot
/// </summary>
[RequireComponent(typeof(PlayerMover), typeof(AudioListener))]
public class PlayerRobot : MonoBehaviour
{
    /// <summary>
    /// The colors to tell the robots apart
    /// </summary>
    public enum RobotColor
    {
        Blue,
        Red,
    }

    /// <summary>
    /// Distinguishes this robot
    /// </summary>
    [SerializeField, Tooltip("Which color this robot is")]
    RobotColor m_robotColor;
    public RobotColor PlayerColor { get { return m_robotColor; } }

    /// <summary>
    /// Sets this robot as the one to start controlling
    /// </summary>
    [SerializeField, Tooltip("True: this robot is being controlled")]
    public bool StartPoweredOn = false;

    /// <summary>
    /// True when the robot has been turned ON and is being controlled
    /// </summary>
    bool m_isPoweredOn = false;

    /// <summary>
    /// How height from the floor to start casting the ray
    /// </summary>
    [SerializeField]
    float m_rayCastHeight = .5f;

    /// <summary>
    /// Distance to case the raycast
    /// </summary>
    [SerializeField]
    float m_rayCastDistance = 1000f;

    /// <summary>
    /// The layer mask to detect collision when raycasting for another robot
    /// </summary>
    [SerializeField]
    LayerMask m_PlayerRobotLayerMask;

    /// <summary>
    /// Battery's location on the model
    /// </summary>
    [SerializeField, Tooltip("Where to place the battery when attached to the robot")]
    Transform m_batterySlot;
    public Transform BatterySlot { get { return m_batterySlot; } }

    /// <summary>
    /// A reference to the attached battery model so that we can enable/disable 
    /// on PowerOn/Down
    /// </summary>
    [SerializeField]
    GameObject m_batteryModel;

    /// <summary>
    /// A reference to the battery component
    /// </summary>
    Battery m_battery;
    
    /// <summary>
    /// True when the robot has power
    /// </summary>
    public bool PoweredOn
    {
        get { return m_isPoweredOn; }
        set {
            m_isPoweredOn = value;
            Mover.PoweredOn = value;

            if (m_isPoweredOn) {                
                AudioManager.Instance.Play2DSound(AudioClipName.RobotPoweredOn);
            }

            Anim.SetBool("IsPoweredOn", value);

            // Enable/Disable the battery pack
            if (m_batteryModel != null) {
                m_batteryModel.SetActive(m_isPoweredOn);
            }
        }
    }

    /// <summary>
    /// The script that moves the player robot
    /// </summary>
    PlayerMover m_mover;
    PlayerMover Mover
    {
        get {
            if(m_mover == null) {
                m_mover = GetComponent<PlayerMover>();
            }
            return m_mover;
        }
    }

    /// <summary>
    /// Stores a copy of the walking sound since it is a loop
    /// </summary>
    AudioSource m_walkingSound;

    [SerializeField]
    Animator m_animator;
    Animator Anim
    {
        get {
            if (m_animator == null) {
                m_animator = GetComponentInChildren<Animator>();
            }

            return m_animator;
        }
    }

    /// <summary>
    /// Ensures the script is enabled/disabled based on whether the robot is powered on/off
    /// </summary>
    void Start()
    {
        PoweredOn = m_isPoweredOn;
        m_battery = FindObjectOfType<Battery>();
    }

    /// <summary>
    /// Fires a raycast directly infront of it and if it collides with 
    /// a another PlayerRobot then it triggers a switch
    /// </summary>
    public void FireBattery()
    {
        // Otherwise we no longer have the battery
        if (PoweredOn && !GameManager.Instance.DisableActions) {
            StartCoroutine(FireBatteryRoutine());
        }
    }

    /// <summary>
    /// First rotates the player towards the mouse position
    /// allowing time for the rotation to register
    /// Then triggers the battery to be fired...otherwise the
    /// RB will be turned off and the robot not move
    /// </summary>
    /// <returns></returns>
    IEnumerator FireBatteryRoutine()
    {
        LookAtMouse();
        yield return new WaitForFixedUpdate();

        AudioManager.Instance.Play2DSound(AudioClipName.ThrowBattery);
        Battery.Instance.FireBattery(this);

        // Wait a bit for the battery to be ejected then power down
        // These are the same waits that happenig in the Battery routine
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        Anim.SetTrigger("FireBattery");
    }

    /// <summary>
    /// Moves the player down the given direction
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="speed"></param>
    public void MoveOnConveyorBelt(Vector3 dir, float speed)
    {
        Mover.MoveInDirection(dir, speed);
    }

    /// <summary>
    /// Forces this robot to look towards the mouse
    /// </summary>
    public void LookAtMouse()
    {
        Mover.LookAtMouse();
    }
}
