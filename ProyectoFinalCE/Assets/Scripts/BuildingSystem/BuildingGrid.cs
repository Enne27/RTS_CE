using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BuildingGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    private BuildingGridCell[,] grid;

    private void Start()
    {
        grid = new BuildingGridCell[width, height];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid [x, y] = new();
            }
        }
    }

    public void SetBuilding(Building building, List<Vector3> allBuildingPositions)
    {
        foreach (var p in allBuildingPositions)
        {
            (int x, int y) = WorldToGridPosition(p);
            grid[x, y].SetBuilding(building);
        }
    }

    public bool CanBuild(List<Vector3> allBuildingPositions)
    {
        foreach (var p in allBuildingPositions)
        {
            (int x, int y) = WorldToGridPosition(p);
            if (x < 0 || x >= width || y < 0 || y >= height) return false;
            if (!grid[x,y].IsEmpty()) return false;
        }
        return true;
    }

    private (int x, int y) WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;

        int x = Mathf.FloorToInt(localPos.x / BuildingManager.CELL_SIZE);
        int y = Mathf.FloorToInt(localPos.y / BuildingManager.CELL_SIZE);

        return (x, y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (BuildingManager.CELL_SIZE <= 0 || width <= 0 || height <= 0) return;
        Vector3 origin = transform.position;
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = origin + new Vector3(0, y * BuildingManager.CELL_SIZE, 0.01f);
            Vector3 end = origin + new Vector3(width * BuildingManager.CELL_SIZE , y * BuildingManager.CELL_SIZE, 0.01f);
            Gizmos.DrawLine(start, end);
        }
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = origin + new Vector3(x * BuildingManager.CELL_SIZE, 0, 0.01f);
            Vector3 end = origin + new Vector3(x * BuildingManager.CELL_SIZE, height * BuildingManager.CELL_SIZE, 0.01f);
            Gizmos.DrawLine(start, end);
        }
    }
}

public class BuildingGridCell
{
    private Building building;

    public void SetBuilding(Building building)
    {
        this.building = building;
    }

    public bool IsEmpty()
    {
        return building == null;
    }
}
