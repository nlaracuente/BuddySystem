using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The player must reach this in order to win
/// Each robot must match its color tile
/// </summary>
[RequireComponent(typeof(Renderer))]
public class GoalTile : MonoBehaviour
{
    [SerializeField, Tooltip("Which color robot must stand on this tile")]
    PlayerRobot.RobotColor m_robotColor;

    [SerializeField, Tooltip("Assocaited a Tile Material with a Robot Color")]
    List<RobotColorToTileMaterial> m_robotColorToTileMaterial;

    /// <summary>
    /// A reference to the renderer component
    /// </summary>
    Renderer m_renderer;

    /// <summary>
    /// True when the right player robot is on this tile
    /// </summary>
    bool m_isActive = false;
    public bool IsActive
    {
        get { return m_isActive; }
        set {
            m_isActive = value;
            EnableParticle = value;

            if (m_isActive) {
                m_renderer.material = m_tileActiveMaterial;
            } else {
                m_renderer.material = m_tileDisabledMaterial;
            }
        }
    }

    /// <summary>
    /// A reference to the current robot on the button
    /// </summary>
    PlayerRobot m_robotOnButton;

    /// <summary>
    /// The particle effects
    /// </summary>
    ParticleSystem m_particleSystem;
    bool EnableParticle
    {
        set {
            if(m_particleSystem == null) {
                m_particleSystem = GetComponentInChildren<ParticleSystem>();
            }

            if(m_particleSystem != null) {
                if (value) {
                    m_particleSystem.Play();
                } else {
                    m_particleSystem.Stop();
                }
            }
        }
    }

    [SerializeField]
    Material m_tileActiveMaterial;

    [SerializeField]
    Material m_tileDisabledMaterial;

    /// <summary>
    /// Set references and material
    /// </summary>
    void Start()
    {
        m_renderer = GetComponent<Renderer>();

        if (m_robotColorToTileMaterial != null) {
            var rColor = m_robotColorToTileMaterial.Select(r => r).Where(s => s.RobotColor == m_robotColor).FirstOrDefault();
            if (rColor != null) {
                m_renderer.material = rColor.TileMaterial;
            }
        }

        IsActive = false;
    }

    /// <summary>
    /// Player is on the tile
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerFeet")) {
            PlayerRobot robot = other.gameObject.GetComponentInParent<PlayerRobot>();
            if (robot.PlayerColor == m_robotColor) {
                m_robotOnButton = robot;
                IsActive = true;
                AudioManager.Instance.PlaySoundAt(AudioClipName.GoalTile, transform.position);
            }
        }
    }

    /// <summary>
    /// Player left the tile
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerFeet")) {
            PlayerRobot robot = other.gameObject.GetComponentInParent<PlayerRobot>();
            if (robot.PlayerColor == m_robotColor) {
                m_robotOnButton = null;
                IsActive = false;
            }
        }
    }
}

/// <summary>
/// A way to map a robot color with a tile material
/// </summary>
[System.Serializable]
public class RobotColorToTileMaterial
{
    public PlayerRobot.RobotColor RobotColor;
    public Material TileMaterial;
}
