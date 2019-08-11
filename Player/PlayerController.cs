using System.Collections;
using UnityEngine;

/// <summary>
/// Player controller handles controlling the currently powered-on robot
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    /// <summary>
    /// The PlayerRobot currently being controlled
    /// </summary>
    public PlayerRobot CurrentPlayerRobot { get; private set; }

    /// <summary>
    /// Find the robot that starts powered ON
    /// Uses the first robot it finds ON
    /// Turns all other robots OFF
    /// </summary>
    void Start()
    {
        foreach (var robot in FindObjectsOfType<PlayerRobot>()) {
            if (robot.StartPoweredOn) {
                if(CurrentPlayerRobot == null) {
                    CurrentPlayerRobot = robot;
                } else {
                    robot.PoweredOn = false;
                }
            }
        }
    }

    /// <summary>
    /// A suggestion I make here to split player input from movement
    /// </summary>
    void Update()
    {
        if (GameManager.Instance.DisableActions) {
            return;
        }

        // On LMB Pressed
        if (Input.GetMouseButtonDown(0) && CurrentPlayerRobot != null) {            
            CurrentPlayerRobot.FireBattery();
        }
    }

    /// <summary>
    /// Called once the level has been initialize to grant player control
    /// </summary>
    public void LevelLoaded()
    {
        if(CurrentPlayerRobot != null) {
            CurrentPlayerRobot.PoweredOn = true;
        } else {
            Debug.Log("A robot has not been defaulted to Powered On");
        }
    }

    /// <summary>
    /// Changes the robot being controlled
    /// </summary>
    /// <param name="robot"></param>
    public void SwitchRobot(PlayerRobot robot)
    {
        if (robot != null) {
            // Should always be done but it does not hurt
            CurrentPlayerRobot.PoweredOn = false;
            robot.PoweredOn = true;
            CurrentPlayerRobot = robot;
        }
    }
}
