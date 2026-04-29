using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

public class FlowField_Manager : MonoBehaviour
{

    public Vector2Int gridSize;
    public float cellRadius = 0.5f;


    [SerializeField] Box bounds;
    [SerializeField] Terrain terrain;
    [SerializeField] float cellsSize = 1; //when value not 1 brakes
    public List<TerrainFlowField> flowFields;
    public TerrainFlowField[] flowFields_Pool;
    //public List<FlowField> flowFields;

    #region Singleton
    public static FlowField_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        flowFields = new List<TerrainFlowField>();
        //flowFields_Pool = new TerrainFlowField[10];

        //for (int i = 0; i < flowFields_Pool.Length; i++)
        //{
        //    flowFields_Pool[i] = new TerrainFlowField(bounds, terrain, cellsSize);
        //    flowFields_Pool[i].CreateGrid();
        //}
        //flowFields = new List<FlowField>();
    }
    #endregion

    
    public int InitializeFlowField(Vector3 worldMousePos)
    {
        TerrainFlowField flowField = new TerrainFlowField(bounds,terrain ,cellsSize);
        flowField.CreateGrid(); //TODO: Make the grid smaller(not all map)
        Cell destinationCell = flowField.GetCellFromWorldPos(worldMousePos);


        for (int i = 0; i < flowFields.Count; i++) 
        {
            if (flowFields[i].destinationCell.worldPos == destinationCell.worldPos)
            {
                return i;
            }
        }

        //flowField.CreateCostField();
        flowField.CreateIntegrationField(destinationCell);
        flowField.CreateFlowField();

        flowFields.Add(flowField);
        return flowFields.Count -1;
    }


    private void OnDrawGizmos()
    {
        DrawBox(bounds);
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



}
