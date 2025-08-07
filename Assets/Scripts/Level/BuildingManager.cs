using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building
{
    public GameObject buildingPrefab;
    public GameObject ghostBuildingPrefab;
}

public class BuildingManager : MonoBehaviour
{
    public bool inBuildingMode = false;

    public float cellSize = 5f;
    public float platformHeight = 5f;
    public int selectedBuildingIndex;
    public List<Building> buildings;
    private BuildingCell selectedBuilding;

    [SerializeField] private float placingRange = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask placingLayer;

    private Transform mainCamera;
    private GameObject ghostBuilding;
    private Dictionary<Vector2Int, BuildingCell> occupiedCells = new Dictionary<Vector2Int, BuildingCell>();

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectBuilding(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectBuilding(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectBuilding(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectBuilding(3);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (inBuildingMode)
                ExitBuildingMode();
            else inBuildingMode = true;
        }

        if (!inBuildingMode) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) PlaceBuilding();
        if (Input.GetKeyDown(KeyCode.Q)) RotateBuilding(-1);
        if (Input.GetKeyDown(KeyCode.E)) RotateBuilding();
    }

    private void FixedUpdate()
    {
        if (!inBuildingMode) return;
        
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, placingRange, placingLayer))
        {
            Vector3 snapped = SnapPosition(hit.point);
            Vector2Int cell = WorldToCell(snapped);
            if (ghostBuilding)
            {
                ghostBuilding.transform.position = snapped;
            }
            else ghostBuilding = Instantiate(buildings[selectedBuildingIndex].ghostBuildingPrefab, snapped, Quaternion.identity);
        }
        else if(ghostBuilding)
        {
            Destroy(ghostBuilding.gameObject);
        }
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int z = Mathf.RoundToInt(worldPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    public bool CanPlace(Vector2Int origin, Vector2Int size)
    {
        Vector2Int offset = new Vector2Int(size.x / 2, size.y / 2);
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var check = origin + offset + new Vector2Int(x, y);
                if (occupiedCells.ContainsKey(check))
                    return false;
            }
        }
        return true;
    }

    public void RegisterCell(Vector2Int origin, BuildingCell build)
    {
        for (int x = 0; x < build.footprint.x; x++)
        {
            for (int y = 0; y < build.footprint.y; y++)
            {
                var cell = origin + new Vector2Int(x, y);
                occupiedCells[cell] = build;
            }
        }

        UpdateConnections(origin, build);
    }

    Vector3 SnapPosition(Vector3 worldPos)
    {
        float x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        float z = Mathf.Round(worldPos.z / cellSize) * cellSize;
        return new Vector3(x, platformHeight, z);
    }
    private void UpdateConnections(Vector2Int origin, BuildingCell build)
    {
        Vector2Int size = build.footprint;

        List<(Vector2Int cell, int direction)> edges = new();

        for (int x = 0; x < size.x; x++)
        {
            edges.Add((origin + new Vector2Int(x, size.y), 0)); // North
            edges.Add((origin + new Vector2Int(x, -1), 2));     // South
        }
        for (int y = 0; y < size.y; y++)
        {
            edges.Add((origin + new Vector2Int(size.x, y), 1)); // East
            edges.Add((origin + new Vector2Int(-1, y), 3));     // West
        }

        foreach (var (neighborCell, dir) in edges)
        {
            if (occupiedCells.TryGetValue(neighborCell, out var neighbor))
            {
                build.OpenDoor((Direction)dir);
                neighbor.OpenDoor((Direction)((dir + 2) % 4));
            }
        }
    }
    public void SelectBuilding(int newBuildingIndex)
    {
        selectedBuildingIndex = Mathf.Clamp(newBuildingIndex, 0, buildings.Count - 1);
    }

    private void RotateBuilding(int direction = 1)
    {
        if (!ghostBuilding) return;
        ghostBuilding.transform.Rotate(0, direction * rotationSpeed, 0);
    }

    private Vector3 AlignToFootprint(Vector2Int cellPos, Vector2Int footprint)
    {
        Vector3 world = new Vector3(cellPos.x, 0, cellPos.y) * cellSize;
        world += new Vector3(footprint.x, 0, footprint.y) * 0.5f * cellSize;
        return world;
    }

    private void PlaceBuilding()
    {
        if (!ghostBuilding) return;
        selectedBuilding = buildings[selectedBuildingIndex].buildingPrefab.GetComponent<BuildingCell>();
        Vector3 snapped = SnapPosition(ghostBuilding.transform.position);
        Vector2Int cell = WorldToCell(snapped);
        //Vector3 worldPos = AlignToFootprint(cell, selectedBuilding.footprint);
        if (CanPlace(cell, selectedBuilding.footprint))
        {
            GameObject obj = Instantiate(buildings[selectedBuildingIndex].buildingPrefab.gameObject, snapped, ghostBuilding.transform.rotation);
            var buildCell = obj.GetComponent<BuildingCell>();
            RegisterCell(cell, buildCell);
        }
        ExitBuildingMode();
    }

    private void ExitBuildingMode()
    {
        if (ghostBuilding) Destroy(ghostBuilding.gameObject);
        inBuildingMode = false;
    }
}
