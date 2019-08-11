using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The battery is the life source of the robots and without it
/// the player cannot control them.
/// 
/// This is our main theme interpretation and as such makes this 
/// clas a Singleton :)
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(TrailRenderer))]
public class Battery : Singleton<Battery>
{
    /// <summary>
    /// How fast the battery moves
    /// </summary>
    [SerializeField, Tooltip("Speed at which the batter moves")]
    float m_speed = 5f;

    /// <summary>
    /// Total bounces before the battery explodes
    /// </summary>
    [SerializeField, Range(0, 10), Tooltip("Total bounces before it destroys itself")]
    int m_maxBounces = 1;

    /// <summary>
    /// How long, in seconds, to continue to move after existing a conveyor
    /// </summary>
    [SerializeField, Tooltip("How long, in seconds, to continue to move after existing a conveyor")]
    float m_travelOffConveyorTime = 1f;

    /// <summary>
    /// A reference to the child renderer to enable/disable it visually
    /// </summary>
    [SerializeField]
    Renderer m_renderer;
    Renderer BatteryRenderer
    {
        get {
            if(m_renderer == null) {
                m_renderer = GetComponentInChildren<Renderer>();
            }
            return m_renderer;
        }
    }

    /// <summary>
    /// A reference to the particle system
    /// </summary>
    [SerializeField, Tooltip("Particle effects")]
    ParticleSystem m_particleSystem;

    /// <summary>
    /// How many times the battery has currently bounced
    /// </summary>
    int m_totalBounces = 0;

    /// <summary>
    /// A reference to the rb component
    /// </summary>
    Rigidbody m_rigidbody;
    Rigidbody RB {
        get {
            if (m_rigidbody == null) {
                m_rigidbody = GetComponent<Rigidbody>();
            }
            return m_rigidbody;
        }
    }

    /// <summary>
    /// A reference to the robot that fired the battery
    /// </summary>
    PlayerRobot m_firedFromRobot;

    /// <summary>
    /// Keeps track of the rigidbody's velocity from the last frame
    /// </summary>
    Vector3 m_lastFrameVelocity;

    /// <summary>
    /// Used to allow the battery to re-attach itself to the robot
    /// that fired it off only after bouncing off another wall first
    /// </summary>
    bool m_colliderWithWall = false;

    /// <summary>
    /// True while the battery was fired
    /// </summary>
    // public bool Fired { get; private set; } = false;

    /// <summary>
    /// Disables/Enables the renderer based on wether the battery is attached or not
    /// </summary>
    bool Attached
    {
        set {
            BatteryRenderer.enabled = !value;
            ShowParticleEffect = !value;
            EnableTrailRenderer = !value;
            m_totalBounces = 0;
        }

        get {
            // If the renderer is OFF then it is attached
            return !BatteryRenderer.enabled;
        }
    }

    /// <summary>
    /// Stops/Plays the particle effects
    /// </summary>
    bool ShowParticleEffect
    {
        set {
            if (m_particleSystem != null) {
                if (value) {
                    m_particleSystem.Play();
                } else {
                    m_particleSystem.Stop();
                }
            }
        }
    }

    /// <summary>
    /// True while attaching itself to a robot
    /// </summary>
    bool m_isAttaching = false;

    /// <summary>
    /// True after falling off a conveyor belt
    /// </summary>
    bool m_isFalling = false;

    /// <summary>
    /// A reference to the conveyor belt the battery is on
    /// </summary>
    public ConveyorBelt OnConveyorBelt { get; private set; }

    /// <summary>
    /// The player robot battery slot in range
    /// </summary>
    PlayerRobot m_playerBatteryInRange;

    /// <summary>
    /// A reference to the trail renderer
    /// </summary>
    TrailRenderer m_trailRenderer;
    bool EnableTrailRenderer
    {
        set {
            if(m_trailRenderer == null) {
                m_trailRenderer = GetComponent<TrailRenderer>();
            }

            if(m_trailRenderer != null && m_trailRenderer.enabled) {
                m_trailRenderer.enabled = value;
                m_trailRenderer.Clear();
            }
        }
    }


    /// <summary>
    /// Setup
    /// </summary>
    void Start()
    {
        // Should be attached to a robot by default
        Attached = true;

        if (m_particleSystem == null) {
            m_particleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    /// <summary>
    /// Updates velocity cache
    /// </summary>
    void Update()
    {
        m_lastFrameVelocity = RB.velocity;    
    }

    /// <summary>
    /// Attaches to the conveyor belt only when:
    ///     - The player that fired it is not on the same conveyor belt
    ///     - Removes reference to robot that fired it when it attches to the conveyor belt
    ///     
    /// Otherwise
    ///     - If a robot is within range then sets a reference to its battery back
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (OnConveyorBelt == null && !Attached && other.CompareTag("ConveyorBelt")) {
            var conveyor = other.GetComponentInParent<ConveyorBelt>();

            if (conveyor != null && conveyor.IsRunning){
                // Ignore conveyor trigger since the robot who shot the battery is on it
                if (m_firedFromRobot != null && conveyor.RobotOnConveyor(m_firedFromRobot)){
                    return;
                }

                // Good to attach
                m_firedFromRobot = null;
                OnConveyorBelt = conveyor;
                RB.velocity = Vector3.zero;
            }
        } else if (other.CompareTag("PlayerBatterySlot")) {
            m_playerBatteryInRange = other.gameObject.GetComponentInParent<PlayerRobot>();
        }
    }

    /// <summary>
    /// Valdiates that it can attach itself to a robot on tigger enter
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        // Attach to robot
        if (other.CompareTag("PlayerBatterySlot")) {
            PlayerRobot robot = other.gameObject.GetComponentInParent<PlayerRobot>();
            if (robot == null) {
                return;
            }

            // Attach to new robot
            if (robot != m_firedFromRobot) {
                AttachToRobot(robot);

            // Allowed to re-attach to robot that fired it
            } else if (m_colliderWithWall) {
                AttachToRobot(m_firedFromRobot);
            }
        }
    }

    /// <summary>
    /// Triggers the actions when the battery is no longer on the belt
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (OnConveyorBelt != null && other.CompareTag("ConveyorBelt")) {
            ConveyorBelt conveyor = other.gameObject.GetComponentInParent<ConveyorBelt>();
            OnConveyorBeltExit(other.transform.forward, conveyor.MoveSpeed);
        } else if (other.CompareTag("PlayerBatterySlot")) {
            m_playerBatteryInRange = null;
        }
    }

    /// <summary>
    /// Triggers a bounce
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with player as those are trigger events
        if (collision.collider.CompareTag("Player")) {
            return;
        }

        if (m_isFalling) {
            Break();
        } else {
            Bounce(collision.contacts[0].normal);
        }
    }

    /// <summary>
    /// Triggers the routine for firing the battery
    /// </summary>
    /// <param name="robot"></param>
    public void FireBattery(PlayerRobot robot)
    {
        StartCoroutine(BatteryFiredRoutine(robot));        
    }

    /// <summary>
    /// Fires the battery in the robot's forward direction
    /// </summary>
    /// <param name="robot"></param>
    /// <returns></returns>
    IEnumerator BatteryFiredRoutine(PlayerRobot robot)
    {
        // Power it down
        robot.PoweredOn = false;

        // Reset values
        m_firedFromRobot = robot;
        m_totalBounces = 0;
        m_colliderWithWall = false;

        // Update position
        RB.position = robot.BatterySlot.position;
        yield return new WaitForFixedUpdate();

        // Detache and show effects
        Attached = false;
        yield return new WaitForEndOfFrame();        

        // Ensure collisions are being detected
        RB.detectCollisions = true;
        RB.velocity = robot.transform.forward * m_speed;
    }

    /// <summary>
    /// Triggers the routine for attaching itself to a robot
    /// </summary>
    /// <param name="robot"></param>
    public void AttachToRobot(PlayerRobot robot)
    {
        OnConveyorBelt = null;
        RB.velocity = Vector3.zero;
        StartCoroutine(AttachToRobotRoutine(robot));
    }

    /// <summary>
    /// Moves into the given robot's battery slot and powers it back on
    /// </summary>
    /// <param name="robot"></param>
    /// <returns></returns>
    IEnumerator AttachToRobotRoutine(PlayerRobot robot)
    {
        m_isAttaching = true;

        // Disable collisions until it is fired again
        m_rigidbody.detectCollisions = false;

        Vector3 destination = robot.BatterySlot.position;
        while (Vector3.Distance(m_rigidbody.position, destination) > .001f) {
            Vector3 towards = Vector3.MoveTowards(RB.position, destination, m_speed * Time.deltaTime);
            RB.MovePosition(towards);            
            yield return new WaitForEndOfFrame();
        }

        RB.position = destination;
        Attached = true;
        PlayerController.Instance.SwitchRobot(robot);

        m_isAttaching = false;
    }

    /// <summary>
    /// Causes the battery to "bounce" off the collision and head in a different direction
    /// </summary>
    /// <param name="collisionNormal"></param>
    void Bounce(Vector3 collisionNormal)
    {
        m_colliderWithWall = true;

        float cSpeed = m_lastFrameVelocity.magnitude;
        Vector3 dir = Vector3.Reflect(m_lastFrameVelocity.normalized, collisionNormal);

        RB.velocity = dir * Mathf.Max(cSpeed, m_speed);

        // AudioManager.Instance.PlaySoundAt(AudioClipName.BatteryBounce, transform.position);
        // Increase the pitch after each bounce
        float pitch = 1 + (m_totalBounces * 0.25f);        

        // Still boncing - play sound
        m_totalBounces++;
        if (m_totalBounces < m_maxBounces) {
            AudioManager.Instance.Play2DSound(AudioClipName.BatteryBounce, 1f, pitch);
        } else {
            Break(); 
        }
    }

    /// <summary>
    /// Moves the battery in the given direction
    /// This is for when the battery is on a conveyor belt
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="speed"></param>
    public void MoveOnConveyorBelt(Vector3 dir, float speed)
    {
        // Stop moving since it is no longer fired
        RB.velocity = Vector3.zero;
        dir.y = 0;
        RB.MovePosition(RB.position + (speed * dir) * Time.deltaTime);
    }

    /// <summary>
    /// Prevent the conveyor from moving the battery any further
    /// Give it one more push in the direction it was headed
    /// Determines whether to attach to the player or fall to the ground
    /// </summary>
    void OnConveyorBeltExit(Vector3 forward, float speed)
    {
        OnConveyorBelt = null;

        // Not game over
        if (m_playerBatteryInRange != null) {
            AttachToRobot(m_playerBatteryInRange);
            return;
        }

        StartCoroutine(OnConveyorBeltExitRoutine(forward, speed));
    }

    /// <summary>
    /// Keeps moving the 
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator OnConveyorBeltExitRoutine(Vector3 forward, float speed)
    {
        float tTime = Time.time + m_travelOffConveyorTime;

        while (Time.time < tTime) {
            MoveOnConveyorBelt(forward, speed);
            yield return new WaitForEndOfFrame();
        }

        // The extra push might have made the Battery be close enough 
        // a player and save them
        if (m_playerBatteryInRange != null) {
            AttachToRobot(m_playerBatteryInRange);
        } else {
            // Game Over man!        
            m_isFalling = true;
            RB.isKinematic = false;
            RB.detectCollisions = true;
            RB.useGravity = true;
            RB.constraints = RigidbodyConstraints.None;
        }        
    }

    /// <summary>
    /// Triggers the break routine
    /// </summary>
    void Break()
    {
        RB.velocity = Vector3.zero;
        StartCoroutine(BreakRoutine());
    }

    /// <summary>
    /// Runs the battery explosion sequence
    /// Notifies the game controller the battery exploded
    /// </summary>
    /// <returns></returns>
    IEnumerator BreakRoutine()
    {
        var source = AudioManager.Instance.Play2DSound(AudioClipName.BatteryBreak);
        BatteryRenderer.enabled = false;
        ShowParticleEffect = false;
        yield return new WaitForSeconds(source.clip.length);
        LevelController.Instance.OnBatteryExploded();
    }
}
