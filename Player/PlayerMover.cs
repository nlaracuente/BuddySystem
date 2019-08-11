using UnityEngine;

/// <summary>
/// Handles the movement logic for the player
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    /// <summary>
    /// A reference to the rigidbody component
    /// </summary>
    Rigidbody m_rigidbody;
    Rigidbody RB
    {
        get {
            if (m_rigidbody == null) {
                m_rigidbody = GetComponent<Rigidbody>();
            }
            return m_rigidbody;
        }
    }

    /// <summary>
    /// How far to cast the ray that checks for mouse position
    /// </summary>
    [SerializeField]
    float m_rayCastDistance = 500f;

    /// <summary>
    /// The layer mask where the floor we care for collision is at
    /// </summary>
    [SerializeField]
    LayerMask m_floorLayerMask;

    /// <summary>
    /// Movement speed
    /// </summary>
    [SerializeField, Tooltip("How fast the robot moves")]
    public float m_movementSpeed = 5.0f;

    /// <summary>
    /// Rotation speed
    /// </summary>
    [SerializeField, Tooltip("How fast the robot turns")]
    public float m_rotationSpeed = 0.1f;

    /// <summary>
    /// Stores the player's current input
    /// </summary>
    Vector3 m_inputVector;

    /// <summary>
    /// Keeps track of angle of rotation for a smoother turn
    /// </summary>
    float m_smoothAngle;

    /// <summary>
    /// True when the player robot is powered on
    /// </summary>
    bool m_poweredOn = false;
    public bool PoweredOn
    {
        set {
            m_poweredOn = value;
            RB.isKinematic = !value; // become kinematic on powered down so it cannot be moved
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
            if(m_animator == null) {
                m_animator = GetComponentInChildren<Animator>();
            }

            return m_animator;
        }
    }

    /// <summary>
    /// Store player input
    /// Transform it to match the camera's position
    /// </summary>
    void Update()
    {
        if (GameManager.Instance.IsLevelCompleted || GameManager.Instance.IsGamePaused) {
            Anim.SetBool("IsMoving", false);
            return;
        }

        if (m_poweredOn){
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            m_inputVector = Camera.main.transform.TransformDirection(input);
            m_inputVector.y = 0f;
            m_inputVector.Normalize();
            Anim.SetBool("IsMoving", m_inputVector != Vector3.zero);

        } else {
            m_inputVector = Vector3.zero;
            Anim.SetBool("IsMoving", false);
        }
    }    

    /// <summary>
    /// Moves and rotates the player 
    /// </summary>
    void FixedUpdate()
    {
        if (GameManager.Instance.DisableActions) {
            return;
        }

        if (m_poweredOn) {
            Rotate();
            Move();

            // Play walking loop sound
            //if (m_inputVector != Vector3.zero) {
            //    if(m_walkingSound == null) {
            //        m_walkingSound = AudioManager.Instance.Play2DSound(AudioClipName.RobotWalk);
            //        m_walkingSound.loop = true;
            //    }

            //    if (!m_walkingSound.isPlaying) {
            //        m_walkingSound.Stop();
            //    }

            //// Stop the walking sound
            //} else {
            //    if (m_walkingSound != null && m_walkingSound.isPlaying) {
            //        m_walkingSound.Stop();
            //    }
            //}
        }
    }

    /// <summary>
    /// Rotates the player based on current input
    /// </summary>
    void Rotate()
    {
        if (m_inputVector == Vector3.zero) {
            return;
        }

        float targetAngle = Mathf.Atan2(m_inputVector.x, m_inputVector.z) * Mathf.Rad2Deg;
        m_smoothAngle = Mathf.LerpAngle(m_smoothAngle, targetAngle, m_rotationSpeed * Time.deltaTime);
        RB.MoveRotation(Quaternion.Euler(Vector3.up * m_smoothAngle));
    }

    /// <summary>
    /// Moves the robot in the direction of the player's input
    /// </summary>
    void Move()
    {
        Vector3 targetPosition = RB.position + (m_inputVector.normalized * m_movementSpeed) * Time.deltaTime;
        RB.MovePosition(targetPosition);
    }

    /// <summary>
    /// Moves the look at target to where the mouse is 
    /// </summary>
    public void LookAtMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, m_rayCastDistance, m_floorLayerMask)) {
            Vector3 dir = hit.point - RB.position;
            dir.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            RB.MoveRotation(targetRotation);
        }
    }

    /// <summary>
    /// Pushes the player in the given direction, ignoring y
    /// </summary>
    /// <param name="dir"></param>
    public void MoveInDirection(Vector3 dir, float speed)
    {
        dir.y = 0;
        RB.MovePosition(RB.position + (speed * dir) * Time.deltaTime);
    }
}
