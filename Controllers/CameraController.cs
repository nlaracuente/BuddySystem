using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the level camera
/// </summary>
public class CameraController : Singleton<CameraController>
{
    /// <summary>
    /// Distance to the player
    /// </summary>
    [SerializeField, Tooltip("Distance to the player")]
    Vector3 m_playerOffset;

    /// <summary>
    /// Smooth speed
    /// </summary>
    [SerializeField]
    float m_moveSpeed = 3f;

    /// <summary>
    /// Angle of rotation
    /// </summary>
    [SerializeField]
    float m_rotationAngle = 60f;

    /// <summary>
    /// Keeps track of the direction the player wants to rotate the camera
    /// </summary>
    float m_playerInput;

    /// <summary>
    /// The current target to follow
    /// </summary>
    Transform Target
    {
        get {
            var target = PlayerController.Instance.CurrentPlayerRobot;
            return target ? target.transform : null;
        }
    }

    /// <summary>
    /// Target to follow
    /// </summary>
    [SerializeField]
    Transform m_target;

    /// <summary>
    /// Store player input
    /// </summary>
    void Update()
    {
        int left = Input.GetKey(KeyCode.Q) ? -1 : 0;
        int right = Input.GetKey(KeyCode.E) ? 1 : 0;
        m_playerInput = left + right;
    }

    /// <summary>
    /// Rotate camera based on player input
    /// </summary>
    void LateUpdate()
    {
        if (GameManager.Instance.DisableActions) {
            return;
        }

        if (m_target != null) {
            FollowTarget();
            RotateAroundTarget();
        }
    }

    /// <summary>
    /// Move towards the player's position
    /// </summary>
    void FollowTarget()
    {
        // Move
        var position = m_target.position;
        transform.position = Vector3.MoveTowards(transform.position, position, m_moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotate around the target
    /// </summary>
    void RotateAroundTarget()
    {
        if (m_playerInput != 0f) {
            float degrees = m_playerInput * m_rotationAngle;
            float angle = degrees * Time.deltaTime;
            transform.RotateAround(m_target.position, Vector3.up, angle);
        }
    }
}
