using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum PathType
{
    //mesh1
    Isolated,

    //mesh2
    End_Up,
    End_Down,
    End_Left,
    End_Right,

    //mesh3
    Straight_Horizontal,
    Straight_Vertical,

    //mesh4
    Corner_UpRight,
    Corner_UpLeft,
    Corner_DownRight,
    Corner_DownLeft,

    //mesh5
    T_Up,
    T_Down,
    T_Left,
    T_Right,

    //mesh6
    Cross
}


[System.Serializable]
public class TunnelPath
{
    public int pathID;
    public List<TunnelFunction> TunnelPieces;

    public TunnelPath(int id, TunnelFunction tunnelPiece)
    {
        pathID = id;
        TunnelPieces = new();
        TunnelPieces.Add(tunnelPiece);
    }
}

public class TunnelFunction : MonoBehaviour
{
    [Header("Path Information")]
    public int pathID;
    public List<TunnelFunction> TunnelConnections;
    public float detectionDistance = 1.1f;
    public LayerMask tunnelLayer;
    public bool isBuilding;
    public bool tileConnectedToEntrance;
    public PathType pathType;

    [Header("TunnelMeshes")]
    [SerializeField] private Mesh isolatedMesh;
    [SerializeField] private Mesh endMesh;
    [SerializeField] private Mesh straightMesh;
    [SerializeField] private Mesh cornerMesh;
    [SerializeField] private Mesh T_CornerMesh;
    [SerializeField] private Mesh crossMesh;

    private bool up, down, left, right;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnDrawGizmos()
    {
        DetectNeighbors();

        bool connected = tileConnectedToEntrance;

        Vector3 center = transform.position;

        Gizmos.color = connected ? Color.green : Color.red;
        Gizmos.DrawSphere(center, 0.15f);

        Gizmos.color = connected ? Color.green : Color.red;

        if (up) Gizmos.DrawLine(center, center + Vector3.up * detectionDistance);
        if (down) Gizmos.DrawLine(center, center + Vector3.down * detectionDistance);
        if (left) Gizmos.DrawLine(center, center + Vector3.left * detectionDistance);
        if (right) Gizmos.DrawLine(center, center + Vector3.right * detectionDistance);
    }

    void DetectNeighbors()
    {
        Vector3 center = transform.position;

        up = Check(center + Vector3.up * detectionDistance);
        down = Check(center + Vector3.down * detectionDistance);
        left = Check(center + Vector3.left * detectionDistance);
        right = Check(center + Vector3.right * detectionDistance);
    }

    bool Check(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 0.2f, tunnelLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject)
                return true;
        }

        return false;
    }

    void DeterminePathType()
    {
        int count = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        if (count == 0)
        {
            pathType = PathType.Isolated;
            meshFilter.mesh = isolatedMesh;
        }
        else if (count == 1)
        {
            meshFilter.mesh = endMesh;
            if (up)
            {
                pathType = PathType.End_Up;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (down)
            {
                pathType = PathType.End_Down;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else if (left)
            {
                pathType = PathType.End_Left;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else 
            {
                pathType = PathType.End_Right;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            } 

        }
        else if (count == 2)
        {
            if (up && down)
            {
                pathType = PathType.Straight_Vertical;
                meshFilter.mesh = straightMesh;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            else if (left && right)
            {
                pathType = PathType.Straight_Horizontal;
                meshFilter.mesh = straightMesh;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            else if (up && right)
            {
                pathType = PathType.Corner_UpRight;
                transform.rotation = Quaternion.Euler(0, 0, 270);
                meshFilter.mesh = cornerMesh;
            }

            else if (up && left)
            {
                pathType = PathType.Corner_UpLeft;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                meshFilter.mesh = cornerMesh;
            }

            else if (down && right)
            {
                pathType = PathType.Corner_DownRight;
                transform.rotation = Quaternion.Euler(0, 0, 180);
                meshFilter.mesh = cornerMesh;
            }

            else
            {
                pathType = PathType.Corner_DownLeft;
                transform.rotation = Quaternion.Euler(0, 0, 90);
                meshFilter.mesh = cornerMesh;

            }
        }
        else if (count == 3)
        {
            meshFilter.mesh = T_CornerMesh;

            if (!down) 
            { 
                pathType = PathType.T_Up;
                transform.rotation = Quaternion.Euler(0, 0, 270);
                Debug.Log("arriba");

            }
            else if (!up)
            {
                pathType = PathType.T_Down;
                transform.rotation = Quaternion.Euler(0, 0, 90);
                Debug.Log("abajo");

            }
            else if (!left)
            {
                pathType = PathType.T_Right;
                transform.rotation = Quaternion.Euler(0, 0, 180);
                Debug.Log("derecha");
            }
            else
            {
                pathType = PathType.T_Left;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                Debug.Log("izquierda");
            } 
        }
        else
        {
            pathType = PathType.Cross;
            meshFilter.mesh = crossMesh;
        }
    }

    
    private void Update()
    {
        DetectNeighbors();
        if(meshFilter != null && meshRenderer != null)
        {
            DeterminePathType();
        }
        RefreshTunnelState();
    }
    

    public void RefreshTunnelState()
    {
        DetectTunnels();
        tileConnectedToEntrance = IsConnectedToEntrance();
    }

    public void DetectTunnels()
    {
        TunnelConnections.Clear();

        Vector3 center = transform.position;

        TunnelFunction upNeighbor = CheckTunnel(center + Vector3.up * detectionDistance);
        TunnelFunction downNeighbor = CheckTunnel(center + Vector3.down * detectionDistance);
        TunnelFunction leftNeighbor = CheckTunnel(center + Vector3.left * detectionDistance);
        TunnelFunction rightNeighbor = CheckTunnel(center + Vector3.right * detectionDistance);

        up = upNeighbor != null;
        down = downNeighbor != null;
        left = leftNeighbor != null;
        right = rightNeighbor != null;

        if (upNeighbor) TunnelConnections.Add(upNeighbor);
        if (downNeighbor) TunnelConnections.Add(downNeighbor);
        if (leftNeighbor) TunnelConnections.Add(leftNeighbor);
        if (rightNeighbor) TunnelConnections.Add(rightNeighbor);
    }

    public TunnelFunction CheckTunnel(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 0.2f, tunnelLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                TunnelFunction tf = hit.GetComponentInParent<TunnelFunction>();
                if (tf != null)
                    return tf;
            }
        }

        return null;
    }

    public bool IsConnectedToEntranceDirect()
    {
        if (!isBuilding) return false;

        Building b = GetComponentInParent<Building>();
        if (b == null) return false;

        return b.data.buildingType == BuildingType.Entrance;
    }

    public bool IsConnectedToEntrance()
    {
        HashSet<TunnelFunction> visited = new();
        Queue<TunnelFunction> queue = new();

        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            TunnelFunction current = queue.Dequeue();

            if (current.IsConnectedToEntranceDirect())
                return true;

            foreach (TunnelFunction neighbor in current.TunnelConnections)
            {
                if (neighbor == null) continue;

                if (visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }
        }
        return false;
    }
}