using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryTarget : MonoBehaviour
{
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

    [SerializeField]
    LayerMask m_wallLayerMask;

    /// <summary>
    /// Moves the look at target to where the mouse is 
    /// </summary>
    void LateUpdate()
    {
        if (GameManager.Instance.DisableActions) {
            return;
        }

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, m_rayCastDistance, m_floorLayerMask)) {

            var PlayerRobot = PlayerController.Instance.CurrentPlayerRobot;
            Vector3 playerPos = PlayerRobot.transform.position;
            var dir = playerPos - hit.point;
            dir.y = PlayerRobot.BatterySlot.position.y;
            Ray ray = new Ray(playerPos, dir);

            if (Physics.Raycast(camRay, out hit, m_rayCastDistance, m_wallLayerMask)) {
                Vector3 position = hit.point;
                position.y = PlayerRobot.BatterySlot.position.y;
                transform.position = position;
                transform.rotation = Quaternion.LookRotation(hit.normal);
            }
           
        }
    }
}
