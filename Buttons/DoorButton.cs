using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controls all the doors in the room
/// </summary>
public class DoorButton : FloorButton
{
    /// <summary>
    /// A list of the controllable this button affects
    /// </summary>
    List<IButtonControllable> m_controllables;
    public override List<IButtonControllable> Controllables
    {
        get {
            return m_controllables;
        }
    }

    /// <summary>
    /// Store all available controllables
    /// </summary>
    void Start()
    {
        m_controllables = FindObjectsOfType<Door>().Cast<IButtonControllable>().ToList();
    }
}
