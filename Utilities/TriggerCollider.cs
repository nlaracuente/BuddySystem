using UnityEngine;

/// <summary>
/// Trigger Colliders are designed for parent classes to know which specific 
/// trigger is Active to know what action to invoke
/// 
/// Note:
///     We could've used Events/Delegates to dispatch the events but for now
///     some just need to know they are active while others need to know the
///     object that is activating them as well
///     
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerCollider : MonoBehaviour
{
    /// <summary>
    /// The tag to recognize trigger
    /// </summary>
    [SerializeField, Tooltip("What to detect on trigger enters")]
    string m_expectedTag = "";

    /// <summary>
    /// True while the trigger is active
    /// </summary>
    public bool ActiveTrigger { get; private set; } = false;

    /// <summary>
    /// The current object activating the tirgger
    /// </summary>
    public GameObject ActiveObject { get; private set; }

    /// <summary>
    /// Sets up the collider
    /// </summary>
    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if(col == null) {
            Debug.LogWarning($"TriggerCollider {name} is missing a Collider component.\n" +
                             $"One was created for you");
            gameObject.AddComponent(typeof(Collider));
            col.GetComponent<Collider>();
        }

        col.isTrigger = true;
    }

    /// <summary>
    /// Sets itself as active
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(m_expectedTag)) {
            ActiveTrigger = true;
        }
    }

    /// <summary>
    /// Deactivates itself
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(m_expectedTag)) {
            ActiveTrigger = false;
        }
    }
}
