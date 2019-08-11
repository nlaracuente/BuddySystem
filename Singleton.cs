using UnityEngine;

/// <summary>
/// A generic singleton base class
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// An instance of self
    /// </summary>
    static T m_instance;

    /// <summary>
    /// True: prevents the object from being destroyed.
    /// <note>
    /// Singletons can be auto-instantiated therefore if you want to keep the persistent key
    /// then you must set it within the child class by overriding this flag.
    /// If you setup the class as a prefab then the option is available in the inspector
    /// </note> 
    /// </summary>
    [SerializeField, Tooltip("Enable this to prevent the object from being destroyed")]
    protected bool m_isPersistent = false;

    /// <summary>
    /// The current instance if one exists or creates a new one 
    /// </summary>
    public static T Instance
    {
        get {
            m_instance = m_instance ?? FindObjectOfType<T>();

            // Create
            if (m_instance == null) {
                GameObject go = new GameObject(typeof(T).Name, typeof(T));
                m_instance = go.GetComponent<T>();
                Debug.LogWarning($"No instance of {typeof(T).Name} was found. A new instance was spawned.\n" +
                                 $"If this is not the desired behavior please ensure to add the instance to the hierachy in the inspector");
            }

            return m_instance;
        }
    }

    /// <summary>
    /// Sets this object as the current instance if one does not exist
    /// Destroys this object if it is not the current instance
    /// Prevents this object from being destroyed if the <see cref="m_isPersistent"/> is enabled
    /// </summary>
    public virtual void Awake()
    {
        if (m_instance == null) {
            m_instance = this as T;

            if (m_isPersistent) {
                // Object cannot be a child or else Unity won't let us make it persistent
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

        } else if (m_instance != this) {
            DestroyImmediate(gameObject);
        }
    }
}