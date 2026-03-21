using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

public class FlowField_Manager : MonoBehaviour
{
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
    }
    #endregion

    public Vector2Int gridSize;
    public float cellRadius = 0.5f;

    public List<FlowField> flowFields;


    public void InitializeFlowField(Vector3 worldMousePos)
    {
        FlowField flowField = new FlowField(cellRadius, gridSize, transform);
        flowField.CreateGrid();
        flowField.CreateCostField();
        Cell destinationCell = flowField.GetCellFromWorldPos(worldMousePos);
        flowField.CreateIntegrationField(destinationCell);
        flowField.CreateFlowField();

        flowFields.Add(flowField);
    }


    #region Debug
    private FlowField debugFlowField;
    public FlowFieldDisplayType curDisplayType;
    public bool displayGrid;

    private void OnValidate()
    {
        debugFlowField = new FlowField(cellRadius, gridSize, transform);
        debugFlowField.CreateGrid();
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        if (displayGrid)
        {
            DrawGrid(Color.yellow);
        }

        if (debugFlowField == null) { return; }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch (curDisplayType)
        {
            case FlowFieldDisplayType.CostField:

                foreach (Cell curCell in debugFlowField.grid)
                {
                    Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
                }
                break;

            case FlowFieldDisplayType.IntegrationField:

                foreach (Cell curCell in debugFlowField.grid)
                {
                    Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
                }
                break;

            case FlowFieldDisplayType.DestinationIcon:

                Gizmos.color = Color.yellow;
                foreach (Cell curCell in debugFlowField.grid)
                {
                    if (curCell.cost == 0)
                    {
                        Handles.DrawWireDisc(curCell.worldPos, Vector3.up, debugFlowField.cellRadius);
                    }
                    else if (curCell.cost == byte.MaxValue)
                    {
                        Vector3 start1 = curCell.worldPos + new Vector3(debugFlowField.cellRadius, 0, -debugFlowField.cellRadius);
                        Vector3 end1 = curCell.worldPos + new Vector3(debugFlowField.cellRadius, 0, debugFlowField.cellRadius);

                        Vector3 start2 = curCell.worldPos + new Vector3(-debugFlowField.cellRadius, 0, debugFlowField.cellRadius);
                        Vector3 end2 = curCell.worldPos + new Vector3(debugFlowField.cellRadius, 0, -debugFlowField.cellRadius);

                        Gizmos.DrawLine(start1, end1);
                        Gizmos.DrawLine(start2, end2);
                    }
                    else if (curCell.bestDirection != null)
                    {
                        Vector2 dir2D = curCell.bestDirection.Vector;

                        Vector3 dir3D = new Vector3(dir2D.x, 0f, dir2D.y);

                        DrawArrow(curCell.worldPos, dir3D, debugFlowField.cellRadius * 2, Color.yellow);
                    }
                }
                break;

            default:
                break;
        }

    }

    public void SetFlowField(FlowField newFlowField)
    {
        debugFlowField = newFlowField;
        cellRadius = newFlowField.cellRadius;
        gridSize = newFlowField.gridSize;
    }

    public void ClearCellDisplay()
    {
        foreach (Transform t in transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    void DrawArrow(Vector3 center, Vector3 direction, float length, Color color)
    {
        Vector3 dir = direction.normalized;

        Vector3 start = center - dir * (length * 0.5f);
        Vector3 end = center + dir * (length * 0.5f);

        Debug.DrawLine(start, end, color);

        float headSize = 0.25f * length;

        Vector3 right = Quaternion.Euler(0, 30, 0) * -dir;
        Vector3 left = Quaternion.Euler(0, -30, 0) * -dir;

        Debug.DrawLine(end, end + right * headSize, color);
        Debug.DrawLine(end, end + left * headSize, color);
    }

    private void DrawGrid(Color drawColor)
    {
        Gizmos.color = drawColor;
        foreach (Cell cell in debugFlowField.grid)
        {
            Vector3 size = new Vector3(debugFlowField.cellRadius * 2, 0.0f, debugFlowField.cellRadius * 2);
            Gizmos.DrawWireCube(cell.worldPos, size);
        }
    }
    #endregion

}
 