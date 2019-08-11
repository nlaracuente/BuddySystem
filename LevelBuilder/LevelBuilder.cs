using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Level Builder reads a Texture2D (A.K.A. an image) and loops through each pixle
/// spawning the prefab that matches that pixle.
/// 
/// This operates on a grid which means if your objects are larger than a single cell in the 
/// grid they will overlap. This can easily be overcomed by making the pixels around the large 
/// object be of empty spaces but keep this in mind if you are also using a tile map to detect
/// whether a space if available or not.
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    /// <summary>
    /// A reference to a transform where we will parent all level objects
    /// </summary>
    [SerializeField, Tooltip("Where to parent the spawned prefabs to")]
    Transform m_levelTransform;
    public Transform LevelTransform
    {
        get {
            if (!m_levelTransform) {
                m_levelTransform = new GameObject("_Level").transform;
            }
            return m_levelTransform;
        }
    }

    /// <summary>
    /// The image containing the level layout
    /// </summary>
    [SerializeField, Tooltip("This is the image that represents the level")]
    Texture2D m_levelTexture;
    public Texture2D LevelTexture { get { return m_levelTexture; } }

    /// <summary>
    /// The scale of the original tile to know how much to offset the tiles by
    /// </summary>
    [SerializeField, Tooltip("Controls the spacing between each tile. Change the if the prefabs are not aligned corretly")]
    Vector2 m_tileScale = new Vector2(1f, 1f);
    public Vector2 TileScale { get { return m_tileScale; } }

    /// <summary>
    /// A collection of all the color to prefab definitions
    /// </summary>
    [SerializeField]
    List<ColorHexToPrefab> m_colorHexToPrefabs;
    
    /// <summary>
    /// Convers the list of <see cref="m_colorHexToPrefabs"/> to a hash table
    /// </summary>
    public Dictionary<Color32, GameObject> PrefabMapping { get; internal set; }

    /// <summary>
    /// Triggers the mapping of the prefabs
    /// </summary>
    private void Awake()
    {
        CreatePrefabMapping();
    }

    /// <summary>
    /// Creates the color to prefab mapping
    /// </summary>
    public void CreatePrefabMapping()
    {
        PrefabMapping = new Dictionary<Color32, GameObject>();

        foreach (ColorHexToPrefab definition in m_colorHexToPrefabs) {
            if (!PrefabMapping.ContainsKey(definition.color)) {
                PrefabMapping.Add(definition.color, definition.prefab);
            }
        }
    }

    /// <summary>
    /// Builds a level using the given texture
    /// Note:
    ///     This is available if you ever want to build a level at run time
    ///     rather than using the editor script.
    /// </summary>
    /// <param name="levelTexture"></param>
    public void BuildLevel(Texture2D levelTexture)
    {
        if (!levelTexture) {
            Debug.LogError("Level texture cannot be null!");
            return;
        }

        ClearLevel();

        for (int x = 0; x < levelTexture.width; x++) {
            for (int z = 0; z < levelTexture.height; z++) {
                Color32 colorId = levelTexture.GetPixel(x, z);
                GameObject prefab = GetPrefabByColorId(colorId);

                if (!prefab) {
                    Debug.LogWarningFormat(
                        "Color Id {0} at position {1}, {2} is not associated with a prefab",
                        colorId, x, z
                    );
                    continue;
                }

                Vector3 position = new Vector3(x, prefab.transform.position.y, z);
                GameObject go = Instantiate(prefab, position, Quaternion.identity, LevelTransform);
                go.name = string.Format("{0}_{1}_{2}", prefab.name, x, z);
            }
        }

    }

    /// <summary>
    /// Returns the prefab associated with the given color if it knows about it
    /// </summary>
    /// <param name="colorId"></param>
    /// <returns></returns>
    public GameObject GetPrefabByColorId(Color32 colorId)
    {
        GameObject prefab = null;

        if (PrefabMapping.ContainsKey(colorId)) {
            prefab = PrefabMapping[colorId];
        }

        return prefab;
    }

    /// <summary>
    /// Creates the level object container if it does not exist
    /// Deletes all the level objects
    /// </summary>
    public void ClearLevel()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in LevelTransform) {
            children.Add(child.gameObject);
        }

        children.ForEach(child => {
            if (child != null) {
                DestroyImmediate(child);
            }
        });
    }
}
