using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CurrentLevel : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        GetComponent<Text>().text = $"Level {GameManager.Instance.CurrentLevel}";
    }
}
