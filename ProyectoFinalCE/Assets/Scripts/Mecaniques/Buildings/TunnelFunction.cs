using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum PathType
{
    Isolated,

    End_Up,
    End_Down,
    End_Left,
    End_Right,

    Straight_Horizontal,
    Straight_Vertical,

    Corner_UpRight,
    Corner_UpLeft,
    Corner_DownRight,
    Corner_DownLeft,

    T_Up,
    T_Down,
    T_Left,
    T_Right,

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
    public int pathID;
    public List<TunnelFunction> TunnelConnections;
    public float detectionDistance = 1.1f;
    public LayerMask tunnelLayer;
    public PathType pathType;
    public bool isBuilding;
    private bool up, down, left, right;
    public bool tileConnectedToEntrance;

    private void OnDrawGizmos()
    {
        DetectNeighbors();
        DeterminePathType();

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
        }
        else if (count == 1)
        {
            if (up) pathType = PathType.End_Up;
            else if (down) pathType = PathType.End_Down;
            else if (left) pathType = PathType.End_Left;
            else pathType = PathType.End_Right;
        }
        else if (count == 2)
        {
            if (up && down)
                pathType = PathType.Straight_Vertical;

            else if (left && right)
                pathType = PathType.Straight_Horizontal;

            else if (up && right)
                pathType = PathType.Corner_UpRight;

            else if (up && left)
                pathType = PathType.Corner_UpLeft;

            else if (down && right)
                pathType = PathType.Corner_DownRight;

            else
                pathType = PathType.Corner_DownLeft;
        }
        else if (count == 3)
        {
            if (!down) pathType = PathType.T_Up;
            else if (!up) pathType = PathType.T_Down;
            else if (!left) pathType = PathType.T_Right;
            else pathType = PathType.T_Left;
        }
        else
        {
            pathType = PathType.Cross;
        }
    }

    
    private void Update()
    {
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