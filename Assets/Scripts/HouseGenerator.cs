using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class HouseGenerator : MonoBehaviour
{
    [Header("Floor Settings")]
    [Tooltip("Prefab of the floor to generate")]
    public GameObject floorPrefab;

    [Tooltip("Size of each floor prefab (width, height, length) used for spacing")]
    public Vector3 floorSize = new Vector3(1, 0.1f, 1);

    [Tooltip("Scale to apply to each floor prefab")]
    public Vector3 floorScale = Vector3.one;

    [Tooltip("Starting position for house generation")]
    public Vector3 startPosition = Vector3.zero;

    [Header("Wall Settings")]
    [Tooltip("Prefab of the wall to surround rooms")]
    public GameObject wallPrefab;

    [Tooltip("Size of each wall prefab (width, height, length) used for spacing")]
    public Vector3 wallSize = new Vector3(1, 1, 0.1f);

    [Tooltip("Scale to apply to each wall prefab")]
    public Vector3 wallScale = Vector3.one;

    [Tooltip("Offset to apply to wall positions for alignment")]
    public Vector3 wallOffset = Vector3.zero;

    [Header("Door Settings")]
    [Tooltip("Prefab of the door to place at intersections")]
    public GameObject doorPrefab;

    [Header("Parent Object")]
    [Tooltip("Parent object to hold generated house")]
    public GameObject houseParent;

    [Header("Room Configurations")]
    [Tooltip("Width of Living Room in tiles")]
    public int livingRoomWidth = 6;

    [Tooltip("Length of Living Room in tiles")]
    public int livingRoomLength = 6;

    [Tooltip("Width of Bedroom in tiles")]
    public int bedroomWidth = 4;

    [Tooltip("Length of Bedroom in tiles")]
    public int bedroomLength = 4;

    [Tooltip("Width of Bathroom in tiles")]
    public int bathroomWidth = 3;

    [Tooltip("Length of Bathroom in tiles")]
    public int bathroomLength = 3;

    [Tooltip("Width of Kitchen in tiles")]
    public int kitchenWidth = 4;

    [Tooltip("Length of Kitchen in tiles")]
    public int kitchenLength = 4;

    [Tooltip("Width of Passage in tiles")]
    public int passageWidth = 2;

    [Tooltip("Length of Passage in tiles")]
    public int passageLength = 6;

    [Tooltip("Include Passage in the house")]
    public bool includePassage = true;



    // Method to generate the house
    public void GenerateHouse()
    {
        if (floorPrefab == null)
        {
            Debug.LogWarning("Floor prefab is not assigned.");
            return;
        }

        // If no parent assigned, create one
        if (houseParent == null)
        {
            houseParent = new GameObject("HouseParent");
            houseParent.transform.position = Vector3.zero;
        }

        // Clear existing house
        ClearHouse();

        // Define room positions for a quad-like house layout, accounting for scaled floor size
        float scaledFloorSizeX = floorSize.x * floorScale.x;
        float scaledFloorSizeZ = floorSize.z * floorScale.z;
        Vector3 livingRoomPos = startPosition;
        Vector3 bedroomPos = livingRoomPos + new Vector3(livingRoomWidth * scaledFloorSizeX, 0, 0); // Right of living room
        Vector3 kitchenPos = livingRoomPos + new Vector3(0, 0, livingRoomLength * scaledFloorSizeZ); // Above living room
        Vector3 bathroomPos = bedroomPos + new Vector3(0, 0, bedroomLength * scaledFloorSizeZ); // Above bedroom
        Vector3 passagePos = livingRoomPos + new Vector3(-passageWidth * scaledFloorSizeX, 0, 0); // Left of living room

        // Generate rooms
        GenerateRoom("LivingRoom", livingRoomPos, livingRoomWidth, livingRoomLength);
        GenerateRoom("Bedroom", bedroomPos, bedroomWidth, bedroomLength);
        GenerateRoom("Bathroom", bathroomPos, bathroomWidth, bathroomLength);
        GenerateRoom("Kitchen", kitchenPos, kitchenWidth, kitchenLength);
        if (includePassage)
        {
            GenerateRoom("Passage", passagePos, passageWidth, passageLength);
        }
    }

    private void GenerateRoom(string roomName, Vector3 position, int width, int length)
    {
        GameObject subParent = new GameObject(roomName);
        subParent.transform.SetParent(houseParent.transform);
        subParent.transform.position = position;
        subParent.AddComponent<Zone>();
        subParent.AddComponent<BoxCollider>();
        subParent.AddComponent<AlwaysShowZoneCollider>();

        // Create floors parent
        GameObject floorsParent = new GameObject("Floors");
        floorsParent.transform.SetParent(subParent.transform);

        // Generate floors
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 floorPos = position + new Vector3(x * floorSize.x * floorScale.x, 0, z * floorSize.z * floorScale.z);
                GameObject floorInstance = (GameObject)PrefabUtility.InstantiatePrefab(floorPrefab);
                floorInstance.transform.position = floorPos;
                floorInstance.transform.localScale = floorScale;
                floorInstance.transform.SetParent(floorsParent.transform);
            }
        }

        // Set BoxCollider
        Vector3 totalSize = new Vector3(width * floorSize.x * floorScale.x, floorSize.y * floorScale.y, length * floorSize.z * floorScale.z);
        Vector3 center = new Vector3((width - 1) * floorSize.x * floorScale.x / 2, 0, (length - 1) * floorSize.z * floorScale.z / 2);
        BoxCollider box = subParent.GetComponent<BoxCollider>();
        box.center = center;
        box.size = totalSize;
        box.isTrigger = true;

        // Generate walls around the room
        GenerateWallsForRoom(subParent, position, width, length);
    }

    private void GenerateWallsForRoom(GameObject subParent, Vector3 position, int width, int length)
    {
        if (wallPrefab == null) return;

        // Create walls parent
        GameObject wallsParent = new GameObject("Walls");
        wallsParent.transform.SetParent(subParent.transform);

        float scaledFloorSizeX = floorSize.x * floorScale.x;
        float scaledFloorSizeZ = floorSize.z * floorScale.z;

        // Left wall (along z) - face positive x (inside room), at left edge
        for (int z = 0; z < length; z++)
        {
            Vector3 wallPos = position + new Vector3(-scaledFloorSizeX / 2, 0, (z + 0.5f) * scaledFloorSizeZ) + wallOffset;
            GameObject wallInstance = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
            wallInstance.transform.position = wallPos;
            wallInstance.transform.localScale = wallScale;
            wallInstance.transform.rotation = Quaternion.Euler(0, 90, 0); // Face positive x
            wallInstance.transform.SetParent(wallsParent.transform);
        }

        // Right wall (along z) - face negative x (inside room), at right edge
        for (int z = 0; z < length; z++)
        {
            Vector3 wallPos = position + new Vector3(width * scaledFloorSizeX + scaledFloorSizeX / 2, 0, (z + 0.5f) * scaledFloorSizeZ) + wallOffset;
            GameObject wallInstance = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
            wallInstance.transform.position = wallPos;
            wallInstance.transform.localScale = wallScale;
            wallInstance.transform.rotation = Quaternion.Euler(0, -90, 0); // Face negative x
            wallInstance.transform.SetParent(wallsParent.transform);
        }

        // Bottom wall (along x) - face positive z (inside room), at bottom edge
        for (int x = 0; x < width; x++)
        {
            Vector3 wallPos = position + new Vector3((x + 0.5f) * scaledFloorSizeX, 0, -scaledFloorSizeX / 2) + wallOffset;
            GameObject wallInstance = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
            wallInstance.transform.position = wallPos;
            wallInstance.transform.localScale = wallScale;
            wallInstance.transform.rotation = Quaternion.Euler(0, 0, 0); // Face positive z
            wallInstance.transform.SetParent(wallsParent.transform);
        }

        // Top wall (along x) - face negative z (inside room), at top edge
        for (int x = 0; x < width; x++)
        {
            Vector3 wallPos = position + new Vector3((x + 0.5f) * scaledFloorSizeX, 0, length * scaledFloorSizeZ + scaledFloorSizeX / 2) + wallOffset;
            GameObject wallInstance = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
            wallInstance.transform.position = wallPos;
            wallInstance.transform.localScale = wallScale;
            wallInstance.transform.rotation = Quaternion.Euler(0, 180, 0); // Face negative z
            wallInstance.transform.SetParent(wallsParent.transform);
        }
    }

    // Method to generate doors at room intersections
    public void GenerateDoors()
    {
        if (doorPrefab == null || houseParent == null) return;

        // Calculate scaled sizes
        float scaledFloorSizeX = floorSize.x * floorScale.x;
        float scaledFloorSizeZ = floorSize.z * floorScale.z;

        Vector3 livingRoomPos = startPosition;
        Vector3 bedroomPos = livingRoomPos + new Vector3(livingRoomWidth * scaledFloorSizeX, 0, 0);
        Vector3 kitchenPos = livingRoomPos + new Vector3(0, 0, livingRoomLength * scaledFloorSizeZ);
        Vector3 bathroomPos = bedroomPos + new Vector3(0, 0, bedroomLength * scaledFloorSizeZ);
        Vector3 passagePos = livingRoomPos + new Vector3(-passageWidth * scaledFloorSizeX, 0, 0);

        // Door between Passage and Living Room
        if (includePassage)
        {
            Transform livingRoomTransform = houseParent.transform.Find("LivingRoom");
            if (livingRoomTransform != null)
            {
                GameObject doorsParent = livingRoomTransform.Find("Doors")?.gameObject ?? new GameObject("Doors");
                doorsParent.transform.SetParent(livingRoomTransform);

                float centerZ = (livingRoomLength / 2.0f + 0.5f) * scaledFloorSizeZ;
                Vector3 doorPos = livingRoomPos + new Vector3(-scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                GameObject doorInstance = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorInstance.transform.position = doorPos;
                doorInstance.transform.localScale = wallScale;
                doorInstance.transform.rotation = Quaternion.Euler(0, 90, 0); // Face positive x
                doorInstance.transform.SetParent(doorsParent.transform);

                Vector3 passageWallPos = passagePos + new Vector3(passageWidth * scaledFloorSizeX + scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                Vector3 livingRoomWallPos = livingRoomPos + new Vector3(-scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                DisableWallAtPosition(passageWallPos, "Passage");
                DisableWallAtPosition(livingRoomWallPos, "LivingRoom");
            }
        }

        // Door between Kitchen and Living Room
        {
            Transform livingRoomTransform = houseParent.transform.Find("LivingRoom");
            if (livingRoomTransform != null)
            {
                GameObject doorsParent = livingRoomTransform.Find("Doors")?.gameObject ?? new GameObject("Doors");
                doorsParent.transform.SetParent(livingRoomTransform);

                float centerX = (livingRoomWidth / 2.0f + 0.5f) * scaledFloorSizeX;
                Vector3 doorPos = livingRoomPos + new Vector3(centerX, 0, livingRoomLength * scaledFloorSizeZ + scaledFloorSizeX / 2) + wallOffset;
                GameObject doorInstance = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorInstance.transform.position = doorPos;
                doorInstance.transform.localScale = wallScale;
                doorInstance.transform.rotation = Quaternion.Euler(0, 180, 0); // Face negative z
                doorInstance.transform.SetParent(doorsParent.transform);

                Vector3 kitchenWallPos = kitchenPos + new Vector3(centerX, 0, -scaledFloorSizeX / 2) + wallOffset;
                Vector3 livingRoomWallPos = livingRoomPos + new Vector3(centerX, 0, livingRoomLength * scaledFloorSizeZ + scaledFloorSizeX / 2) + wallOffset;
                DisableWallAtPosition(kitchenWallPos, "Kitchen");
                DisableWallAtPosition(livingRoomWallPos, "LivingRoom");
            }
        }

        // Door between Living Room and Bedroom
        {
            Transform livingRoomTransform = houseParent.transform.Find("LivingRoom");
            if (livingRoomTransform != null)
            {
                GameObject doorsParent = livingRoomTransform.Find("Doors")?.gameObject ?? new GameObject("Doors");
                doorsParent.transform.SetParent(livingRoomTransform);

                float centerZ = (livingRoomLength / 2.0f + 0.5f) * scaledFloorSizeZ;
                Vector3 doorPos = livingRoomPos + new Vector3(livingRoomWidth * scaledFloorSizeX + scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                GameObject doorInstance = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorInstance.transform.position = doorPos;
                doorInstance.transform.localScale = wallScale;
                doorInstance.transform.rotation = Quaternion.Euler(0, -90, 0); // Face negative x
                doorInstance.transform.SetParent(doorsParent.transform);

                Vector3 bedroomWallPos = bedroomPos + new Vector3(-scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                Vector3 livingRoomWallPos = livingRoomPos + new Vector3(livingRoomWidth * scaledFloorSizeX + scaledFloorSizeX / 2, 0, centerZ) + wallOffset;
                DisableWallAtPosition(bedroomWallPos, "Bedroom");
                DisableWallAtPosition(livingRoomWallPos, "LivingRoom");
            }
        }

        // Door between Bedroom and Bathroom
        {
            Transform bedroomTransform = houseParent.transform.Find("Bedroom");
            if (bedroomTransform != null)
            {
                GameObject doorsParent = bedroomTransform.Find("Doors")?.gameObject ?? new GameObject("Doors");
                doorsParent.transform.SetParent(bedroomTransform);

                float centerX = (bedroomWidth / 2.0f + 0.5f) * scaledFloorSizeX;
                Vector3 doorPos = bedroomPos + new Vector3(centerX, 0, bedroomLength * scaledFloorSizeZ + scaledFloorSizeX / 2) + wallOffset;
                GameObject doorInstance = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                doorInstance.transform.position = doorPos;
                doorInstance.transform.localScale = wallScale;
                doorInstance.transform.rotation = Quaternion.Euler(0, 180, 0); // Face negative z
                doorInstance.transform.SetParent(doorsParent.transform);

                Vector3 bathroomWallPos = bathroomPos + new Vector3(centerX, 0, -scaledFloorSizeX / 2) + wallOffset;
                Vector3 bedroomWallPos = bedroomPos + new Vector3(centerX, 0, bedroomLength * scaledFloorSizeZ + scaledFloorSizeX / 2) + wallOffset;
                DisableWallAtPosition(bathroomWallPos, "Bathroom");
                DisableWallAtPosition(bedroomWallPos, "Bedroom");
            }
        }
    }

    private void DisableWallAtPosition(Vector3 pos, string roomName)
    {
        Transform roomTransform = houseParent.transform.Find(roomName);
        if (roomTransform == null) return;
        Transform wallsParentTransform = roomTransform.Find("Walls");
        if (wallsParentTransform == null) return;
        foreach (Transform wall in wallsParentTransform)
        {
            if (Vector3.Distance(wall.position, pos) < 0.1f)
            {
                wall.gameObject.SetActive(false);
            }
        }
    }

    // Method to clear generated house
    public void ClearHouse()
    {
        if (houseParent == null) return;

        int childCount = houseParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            DestroyImmediate(houseParent.transform.GetChild(i).gameObject);
            #else
            Destroy(houseParent.transform.GetChild(i).gameObject);
            #endif
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HouseGenerator))]
public class HouseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HouseGenerator generator = (HouseGenerator)target;
        if (GUILayout.Button("Generate House"))
        {
            generator.GenerateHouse();
        }

        if (GUILayout.Button("Generate Doors"))
        {
            generator.GenerateDoors();
        }

        if (GUILayout.Button("Clear House"))
        {
            generator.ClearHouse();
        }
    }
}
#endif
