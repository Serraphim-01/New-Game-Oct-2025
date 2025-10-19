using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FloorGenerator : MonoBehaviour
{
    [Header("Floor Settings")]
    [Tooltip("Prefab of the floor to generate")]
    public GameObject floorPrefab;

    [Tooltip("Size of each floor prefab (width, height, length) used for spacing")]
    public Vector3 floorSize = new Vector3(1, 0.1f, 1);

    [Tooltip("Scale to apply to each floor prefab")]
    public Vector3 floorScale = Vector3.one;

    [Tooltip("Starting position for floor generation")]
    public Vector3 startPosition = Vector3.zero;

    [Tooltip("Width of the room in number of floor tiles")]
    public int roomWidth = 1;

    [Tooltip("Length of the room in number of floor tiles")]
    public int roomLength = 1;

    [Header("Parent Object")]
    [Tooltip("Parent object to hold generated floors")]
    public GameObject floorParent;

    [Tooltip("Name for the sub-parent object")]
    public string parentName = "Parent name";

    private GameObject subParent;

    // Method to generate floors in a grid layout
    public void GenerateFloors()
    {
        if (floorPrefab == null)
        {
            Debug.LogWarning("Floor prefab is not assigned.");
            return;
        }

        if (roomWidth < 1 || roomLength < 1)
        {
            Debug.LogWarning("Room width and length must be at least 1.");
            return;
        }

        // If no parent assigned, create one
        if (floorParent == null)
        {
            floorParent = new GameObject("FloorParent");
            floorParent.transform.position = Vector3.zero;
        }

        // Create or find the sub-parent under floorParent
        subParent = floorParent.transform.Find(parentName)?.gameObject;
        if (subParent == null)
        {
            subParent = new GameObject(parentName);
            subParent.transform.SetParent(floorParent.transform);
            subParent.transform.position = startPosition;
            subParent.AddComponent<Zone>();
            subParent.AddComponent<BoxCollider>();
            subParent.AddComponent<AlwaysShowZoneCollider>();
        }

        // Clear existing children under subParent
        ClearFloors();

        // Generate floors in a grid (roomWidth x roomLength)
        for (int x = 0; x < roomWidth; x++)
        {
            for (int z = 0; z < roomLength; z++)
            {
                Vector3 position = new Vector3(x * floorSize.x, 0, z * floorSize.z);
                GameObject floorInstance = (GameObject)PrefabUtility.InstantiatePrefab(floorPrefab);
                floorInstance.transform.position = position;
                floorInstance.transform.localScale = floorScale;
                floorInstance.transform.SetParent(subParent.transform);
            }
        }

        // Calculate and set BoxCollider to cover all floors
        Vector3 totalSize = new Vector3(roomWidth * floorSize.x, floorSize.y, roomLength * floorSize.z);
        Vector3 center = new Vector3((roomWidth - 1) * floorSize.x / 2, 0, (roomLength - 1) * floorSize.z / 2);
        BoxCollider box = subParent.GetComponent<BoxCollider>();
        box.center = center;
        box.size = totalSize;
        box.isTrigger = true; // Assuming zones are triggers
    }

    // Method to clear generated floors
    public void ClearFloors()
    {
        if (subParent == null) return;

        int childCount = subParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            DestroyImmediate(subParent.transform.GetChild(i).gameObject);
            #else
            Destroy(subParent.transform.GetChild(i).gameObject);
            #endif
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FloorGenerator))]
public class FloorGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FloorGenerator generator = (FloorGenerator)target;
        if (GUILayout.Button("Generate Floors"))
        {
            generator.GenerateFloors();
        }

        if (GUILayout.Button("Clear Floors"))
        {
            generator.ClearFloors();
        }
    }
}
#endif
