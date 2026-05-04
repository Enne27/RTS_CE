using System.Collections.Generic;
using UnityEngine;

public class TerrainFlowField : MonoBehaviour
{
    Box bounds;
    Terrain terrain;
    public float cellsSize = 1;
    Vector3 terrainPos;

    public Cell[,] grid;
    public int gridX_Amount => (int)(bounds.size.x / cellsSize);
    public int gridY_Amount => (int)(bounds.size.y / cellsSize);

    public Cell destinationCell;

    public TerrainFlowField(Box bounds, Terrain terrain, float cellsSize)
    {
        this.bounds = bounds;
        this.terrain = terrain;
        this.cellsSize = cellsSize;
    }

    public void CreateGrid()
    {
        grid = new Cell[gridX_Amount, gridY_Amount];
        Vector3 gridOrigin = new Vector3(bounds.Center.x, 0, bounds.Center.y);

        for (int x = 0; x < gridX_Amount; x++)
        {
            for (int y = 0; y < gridY_Amount; y++)
            {

                float worldX = bounds.Min.x + (x + 0.5f) * cellsSize;
                float worldZ = bounds.Min.y + (y + 0.5f) * cellsSize;

                Vector3 samplePos = new Vector3(worldX, 0, worldZ);

                float height = terrain.SampleHeight(samplePos);
                Vector3 worldPos = new Vector3(worldX, height /*+ terrainPos.y*/, worldZ);

                int layerMask = LayerMask.GetMask("WalkableTerrain");

                RaycastHit hit;
                Vector3 startPos = new Vector3(worldX, height + terrainPos.y + 300, worldZ);

                Cell cell = new Cell(worldPos, new Vector2Int(x, y));

                grid[x, y] = cell;
                if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    cell.cost = 1;
                }
                else
                {
                    cell.cost = byte.MaxValue;
                }

            }
        }
    }

    public void CreateIntegrationField(Cell _destinationCell)
    {
        destinationCell = _destinationCell;

        destinationCell.cost = 0;
        destinationCell.bestCost = 0;

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destinationCell);

        while (cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
            foreach (Cell curNeighbor in curNeighbors)
            {
                if (curNeighbor.cost == byte.MaxValue) { continue; }
                if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach (Cell curCell in grid)
        {
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

            int bestCost = curCell.bestCost;

            foreach (Cell curNeighbor in curNeighbors)
            {
                if (curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
                }
            }
        }
    }


    private List<Cell> GetNeighborCells(Vector2 nodeIndex, List<GridDirection> directions)
    {
        List<Cell> neighborCells = new List<Cell>();

        foreach (Vector2Int curDirection in directions)
        {
            Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
            if (newNeighbor != null)
            {
                neighborCells.Add(newNeighbor);
            }
        }
        return neighborCells;
    }

    private Cell GetCellAtRelativePos(Vector2 orignPos, Vector2 relativePos)
    {
        Vector2 finalPos = orignPos + relativePos;

        if (finalPos.x < 0 || finalPos.x >= bounds.size.x / cellsSize || finalPos.y < 0 || finalPos.y >= bounds.size.y / cellsSize)
        {
            return null;
        }

        else { return grid[(int)finalPos.x, (int)finalPos.y]; }
    }

    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        //Vector3 gridOrigin =
        //    origin.position
        //    - new Vector3(gridSize.x * cellDiameter, 0f, gridSize.y * cellDiameter) * 0.5f;

        Vector3 local = worldPos - new Vector3(bounds.Center.x, 0, bounds.Center.y);

        int x = Mathf.FloorToInt(local.x / cellsSize);
        int y = Mathf.FloorToInt(local.z / cellsSize);

        x = Mathf.Clamp(x, 0, gridX_Amount - 1);
        y = Mathf.Clamp(y, 0, gridY_Amount - 1);

        return grid[x, y];
    }
}