//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

//public class GridDebug : MonoBehaviour
//{
//    public FlowField_Test flowField_Test;
//    public bool displayGrid;

//    public FlowFieldDisplayType curDisplayType;

//    private Vector2Int gridSize;
//    private float cellRadius;
//    private FlowField curFlowField;

//    public void SetFlowField(FlowField newFlowField)
//    {
//        curFlowField = newFlowField;
//        cellRadius = newFlowField.cellRadius;
//        gridSize = newFlowField.gridSize;
//    }

//    public void ClearCellDisplay()
//    {
//        foreach (Transform t in transform)
//        {
//            GameObject.Destroy(t.gameObject);
//        }
//    }


//    void DrawArrow(Vector3 center, Vector3 direction, float length, Color color)
//    {
//        Vector3 dir = direction.normalized;

//        Vector3 start = center - dir * (length * 0.5f);
//        Vector3 end = center + dir * (length * 0.5f);

//        Debug.DrawLine(start, end, color);

//        float headSize = 0.25f * length;

//        Vector3 right = Quaternion.Euler(0, 30, 0) * -dir;
//        Vector3 left = Quaternion.Euler(0, -30, 0) * -dir;

//        Debug.DrawLine(end, end + right * headSize, color);
//        Debug.DrawLine(end, end + left * headSize, color);
//    }

//    private void OnDrawGizmos()
//    {
//        if (displayGrid)
//        {
//            if (curFlowField == null)
//            {
//                DrawGrid(flowField_Test.gridSize, Color.yellow, flowField_Test.cellRadius);
//            }
//            else
//            {
//                DrawGrid(gridSize, Color.green, cellRadius);
//            }
//        }

//        if (curFlowField == null) { return; }

//        GUIStyle style = new GUIStyle(GUI.skin.label);
//        style.alignment = TextAnchor.MiddleCenter;

//        switch (curDisplayType)
//        {
//            case FlowFieldDisplayType.CostField:

//                foreach (Cell curCell in curFlowField.grid)
//                {
//                    Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
//                }
//                break;

//            case FlowFieldDisplayType.IntegrationField:

//                foreach (Cell curCell in curFlowField.grid)
//                {
//                    Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
//                }
//                break;

//            case FlowFieldDisplayType.DestinationIcon:

//                Gizmos.color = Color.yellow;
//                foreach (Cell curCell in curFlowField.grid)
//                {
//                    if(curCell.cost == 0)
//                    {
//                        Handles.DrawWireDisc(curCell.worldPos, Vector3.up, .5f);
//                    }
//                    else if (curCell.cost == byte.MaxValue)
//                    {
//                        Vector3 start1 = curCell.worldPos + new Vector3(-0.5f, 0, -0.5f);
//                        Vector3 end1 = curCell.worldPos + new Vector3(0.5f, 0, 0.5f);

//                        Vector3 start2 = curCell.worldPos + new Vector3(-0.5f, 0, 0.5f);
//                        Vector3 end2 = curCell.worldPos + new Vector3(0.5f, 0, -0.5f);

//                        Gizmos.DrawLine(start1, end1);
//                        Gizmos.DrawLine(start2, end2);
//                    }
//                    else if (curCell.bestDirection != null)
//                    {
//                        Vector2 dir2D = curCell.bestDirection.Vector;

//                        Vector3 dir3D = new Vector3(dir2D.x, 0f, dir2D.y);

//                        DrawArrow(curCell.worldPos, dir3D, 1f, Color.yellow);
//                    }
//                }
//                break;

//            default:
//                break;
//        }

//    }

//    private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
//    {
//        Gizmos.color = drawColor;
//        for (int x = 0; x < drawGridSize.x; x++)
//        {
//            for (int y = 0; y < drawGridSize.y; y++)
//            {
//                Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
//                Vector3 size = Vector3.one * drawCellRadius * 2;
//                Gizmos.DrawWireCube(center, size);
//            }
//        }
//    }
//}