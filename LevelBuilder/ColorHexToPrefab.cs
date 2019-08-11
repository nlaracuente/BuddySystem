using UnityEngine;

/// <summary>
/// Maps a pixle color to a prefab
/// </summary>
[System.Serializable]
public struct ColorHexToPrefab
{
    /// <summary>
    /// The color that represent the prefab
    /// </summary>
    [Tooltip("The color that represents the prefab. Note: the default alpha is 0. Make sure to update to 255")]
    public Color32 color;

    /// <summary>
    /// The prefab itself
    /// </summary>
    [Tooltip("The prefab to instantiate when this color is recognized")]
    public GameObject prefab;
}
