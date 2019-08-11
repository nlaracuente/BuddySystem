using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A floor button changes the state of all the IButtonControllable it handles ON or OFF
/// </summary>
public abstract class FloorButton : MonoBehaviour
{
    /// <summary>
    /// Returns a collection of all the IButtonControllables this floor button affects
    /// </summary>
    public abstract List<IButtonControllable> Controllables { get; }

    /// <summary>
    /// True while a player is standing on the button
    /// </summary>
    public bool IsPressed { get; private set; } = false;

    /// <summary>
    /// Keeps a collection of object pressing the button
    /// </summary>
    List<GameObject> m_objectedPressingTheButton = new List<GameObject>();

    /// <summary>
    /// Triggers on button pressed
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerFeet")) {

            // Trigger press if no other one has been pressed
            if (!IsPressed) {
                AudioManager.Instance.PlaySoundAt(AudioClipName.ButtonOn, transform.position);
                IsPressed = true;
                Controllables.ForEach(c => { if (c != null) c.OnButtonPressed(); });
            }

            // Add the new object on the button
            if (!m_objectedPressingTheButton.Contains(other.gameObject)) {
                m_objectedPressingTheButton.Add(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Triggers on button released
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerFeet")) {

            // Remove them from the list
            if (m_objectedPressingTheButton.Contains(other.gameObject)) {
                m_objectedPressingTheButton.Remove(other.gameObject);
            }

            if (IsPressed) {
                // We will only trigger release when there are no other objects
                // pressing the button (or another of the same type)
                if (m_objectedPressingTheButton.Count > 0) {
                    return;
                }

                AudioManager.Instance.PlaySoundAt(AudioClipName.ButtonOff, transform.position);
                IsPressed = false;
                Controllables.ForEach(c => { if (c != null) c.OnButtonReleased(); });
            }
        }
    }
}