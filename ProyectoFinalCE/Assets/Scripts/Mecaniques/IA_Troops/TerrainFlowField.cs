using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainFlowField : MonoBehaviour
{

    Terrain terrain;
    Vector3 terrainPos;
    Vector3 terrainSize;
    float stepX;
    float stepZ;

    public int gridResolution = 20;
    //public List<Cell> grid;
    public Quadtree<Cell> quadtreeGrid;
    Vector3[] lineSegments = new Vector3[4];

    public Cell destinationCell;

    #region Debug
    public bool showSingleCell;
    public bool showQuadTree;
    public bool showGrid;

    public int selectedCell;
    public int depth;

    [SerializeField] float nodeStepDelay = 1f;

    private List<int> debugPath = new();
    private Coroutine debugCoroutine;
    private System.Random rng = new System.Random();

    private void Start()
    {
        debugCoroutine = StartCoroutine(DebugRandomQuadtreePath());
    }

    private IEnumerator DebugRandomQuadtreePath()
    {
        while (true)
        {
            debugPath.Clear();

            if (quadtreeGrid != null && quadtreeGrid.Root != -1)
            {
                int nodeId = quadtreeGrid.Root;
                debugPath.Add(nodeId);

                while (true)
                {
                    var node = quadtreeGrid.Nodes[nodeId];  
                    List<int> children = new List<int>(4);

                    for (int i = 0; i < 4; i++)
                        if (node.Children[i] != -1)
                            children.Add(node.Children[i]);

                    if (children.Count == 0)
                        break;

                    nodeId = children[rng.Next(children.Count)];
                    debugPath.Add(nodeId);

                    yield return new WaitForSeconds(nodeStepDelay);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
    private void OnDrawGizmos()
    {
        if (quadtreeGrid == null) return;
        float halfX = stepX * 0.5f;
        float halfZ = stepZ * 0.5f;

        if (showSingleCell)
        {
            DrawDebugPath();
            //Vector2 pos;
            //if (quadtreeGrid.Items.Count < selectedCell)
            //    pos = quadtreeGrid.Items[selectedCell].gridIndex;
            //quadtreeGrid.
            
        }
        if (showQuadTree)
        {
            drawTree(quadtreeGrid.Nodes[quadtreeGrid.Root]);
        }
        if (showGrid)
        {
            foreach (Cell cell in quadtreeGrid.Items)
            {
                Gizmos.color = cell.cost == byte.MaxValue ? Color.red : Color.yellow;

                Vector3 p0 = cell.worldPos + new Vector3(-halfX, 0, -halfZ);
                Vector3 p1 = cell.worldPos + new Vector3(halfX, 0, -halfZ);
                Vector3 p2 = cell.worldPos + new Vector3(halfX, 0, halfZ);
                Vector3 p3 = cell.worldPos + new Vector3(-halfX, 0, halfZ);

                Gizmos.DrawLine(p0, p1);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p0);
            }
        }
    }

    private void drawTree(Node node)
    {
        foreach (var item in node.Children)
        {
            DrawBox(node.Bounds);
            if (item == -1) return;
            drawTree(quadtreeGrid.Nodes[item]);
        }
    }

    private void DrawDebugPath()
    {
        if (debugPath == null || debugPath.Count == 0) return;

        for (int i = 0; i < debugPath.Count; i++)
        {
            int nodeId = debugPath[i];
            var b = quadtreeGrid.Nodes[nodeId].Bounds;

            Gizmos.color = i == debugPath.Count - 1 ? Color.cyan : Color.green;
            DrawBox(b);
        }
    }

    private void DrawBox(Box b)
    {
        Vector3 a = new Vector3(b.Min.x, 0, b.Min.y);
        Vector3 b1 = new Vector3(b.Max.x, 0, b.Min.y);
        Vector3 c = new Vector3(b.Max.x, 0, b.Max.y);
        Vector3 d = new Vector3(b.Min.x, 0, b.Max.y);

        Gizmos.DrawLine(a, b1);
        Gizmos.DrawLine(b1, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
    #endregion

    private void Awake()
    {
        createGrid();
    }

    public void createGrid()
    {
        List<Cell> grid = new List<Cell>();
        terrain = GetComponent<Terrain>();
        terrainPos = terrain.transform.position;
        terrainSize = terrain.terrainData.size;
        stepX = terrainSize.x / gridResolution;
        stepZ = terrainSize.z / gridResolution;
        Vector3 gridOrigin = this.transform.position;

        for (int x = 0; x < gridResolution; x++)
        {
            for (int z = 0; z < gridResolution; z++)
            {
                float worldX = terrainPos.x + (x + 0.5f) * stepX;
                float worldZ = terrainPos.z + (z + 0.5f) * stepZ;

                Vector3 samplePos = new Vector3(worldX, 0, worldZ);

                float height = terrain.SampleHeight(samplePos);
                Vector3 worldPos = gridOrigin + new Vector3(worldX, height + terrainPos.y, worldZ);

                int layerMask = LayerMask.GetMask("WalkableTerrain");

                RaycastHit hit;
                Vector3 startPos = new Vector3(worldX, height + terrainPos.y + 300, worldZ);

                if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    Cell cell = new Cell(worldPos, new Vector2(x, z));
                    cell.cost = 1;
                    grid.Add(cell);
                    Debug.Log("Did hit");
                }
                else
                {
                    Debug.Log("Did not hit");
                }
            }
        }
        quadtreeGrid = Quadtree<Cell>.Build(grid, c => (Vector3)c.worldPos);
    }

    //public void CreateIntegrationField(Cell _destinationCell)
    //{
    //    destinationCell = _destinationCell;

    //    destinationCell.cost = 0;
    //    destinationCell.bestCost = 0;

    //    Queue<Cell> cellsToCheck = new Queue<Cell>();

    //    cellsToCheck.Enqueue(destinationCell);

    //    while (cellsToCheck.Count > 0)
    //    {
    //        Cell curCell = cellsToCheck.Dequeue();
    //        List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
    //        foreach (Cell curNeighbor in curNeighbors)
    //        {
    //            if (curNeighbor.cost == byte.MaxValue) { continue; }
    //            if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
    //            {
    //                curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
    //                cellsToCheck.Enqueue(curNeighbor);
    //            }
    //        }
    //    }
    //}

    //public void CreateFlowField()
    //{
    //    foreach (Cell curCell in grid)
    //    {
    //        List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

    //        int bestCost = curCell.bestCost;

    //        foreach (Cell curNeighbor in curNeighbors)
    //        {
    //            if (curNeighbor.bestCost < bestCost)
    //            {
    //                bestCost = curNeighbor.bestCost;
    //                curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
    //            }
    //        }
    //    }
    //}


    //private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
    //{
    //    List<Cell> neighborCells = new List<Cell>();

    //    foreach (Vector2Int curDirection in directions)
    //    {
    //        Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
    //        if (newNeighbor != null)
    //        {
    //            neighborCells.Add(newNeighbor);
    //        }
    //    }
    //    return neighborCells;
    //}

    //private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
    //{
    //    Vector2Int finalPos = orignPos + relativePos;

    //    if (finalPos.x < 0 || finalPos.x >= gridResolution || finalPos.y < 0 || finalPos.y >= gridResolution)
    //    {
    //        return null;
    //    }

    //    else { return grid[finalPos.x, finalPos.y]; }
    //}

}