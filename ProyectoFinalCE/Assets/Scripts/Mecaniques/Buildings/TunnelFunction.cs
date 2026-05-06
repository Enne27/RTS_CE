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
public class Tunnel
{
    public List<Tunnel> TunnelConnections;
}

public class TunnelFunction : MonoBehaviour
{
    public Tunnel tunnel;
    public float detectionDistance = 1.1f;
    public LayerMask tunnelLayer;
    public PathType pathType;
    public bool isBuilding;
    private bool up, down, left, right;

    private void OnDrawGizmos()
    {
        DetectNeighbors();
        DeterminePathType();

        Vector3 center = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, 0.1f);

        Gizmos.color = Color.green;

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
}