using StateMachine.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class AntWorkerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.05f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Tunnel Detection")]
    [SerializeField] private float tunnelSearchRadius = 0.25f;
    [SerializeField] private LayerMask tunnelLayer;

    [Header("Debug")]
    [SerializeField] private TunnelFunction currentTunnel;
    [SerializeField] private TunnelFunction targetTunnel;

    [Header("Work")]
    [SerializeField] private Building currentBuilding;

    private StateMachineComponent stateMachineManager;
    private Animator animationController;

    private TunnelFunction previousTunnel;

    private bool isMoving;

    private List<TunnelFunction> currentPath = new();
    private int currentPathIndex = 0;

    private void Start()
    {
        stateMachineManager = GetComponent<StateMachineComponent>();
        animationController = GetComponent<Animator>();
    }

    private void Update()
    {
        isMoving = targetTunnel != null;
        animationController.SetBool("IsMoving", isMoving);

        if (stateMachineManager.GetCurrentStateName() == "Wander")
            Wander();
        else if (stateMachineManager.GetCurrentStateName() == "Working")
            Work();
    }

    // =========================
    // WANDER
    // =========================

    private void Wander()
    {
        if (currentTunnel == null)
        {
            currentTunnel = FindCurrentTunnel();
            return;
        }

        if (targetTunnel == null)
        {
            ChooseNextTunnel();
        }

        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    // =========================
    // WORK
    // =========================

    private void Work()
    {
        if (currentBuilding == null)
            return;

        if (currentTunnel == null)
        {
            currentTunnel = FindCurrentTunnel();
            return;
        }

        TunnelFunction targetWorkTunnel = currentBuilding.gameObject.GetComponentInChildren<TunnelFunction>();

        if (targetWorkTunnel == null)
            return;

        // recalcular constantemente
        RecalculatePath(targetWorkTunnel);

        // ya llegamos
        if (currentTunnel == targetWorkTunnel)
        {
            targetTunnel = null;

            // quedarse quieta aquí
            return;
        }

        // seguir path
        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    public void CallToBuild(Building buildToWork)
    {
        currentBuilding = buildToWork;

        stateMachineManager.GetStateContext().hasWork = true;
    }

    // =========================
    // PATHFINDING
    // =========================

    private void RecalculatePath(TunnelFunction destination)
    {
        List<TunnelFunction> path = FindPath(currentTunnel, destination);

        // no existe camino
        if (path == null || path.Count <= 1)
        {
            TunnelFunction closestReachable =
                FindClosestReachableTunnel(destination);

            if (closestReachable == null)
            {
                targetTunnel = null;
                return;
            }

            path = FindPath(currentTunnel, closestReachable);

            if (path == null || path.Count <= 1)
            {
                targetTunnel = null;
                return;
            }
        }

        currentPath = path;

        // siguiente nodo
        currentPathIndex = 1;

        if (currentPathIndex < currentPath.Count)
        {
            targetTunnel = currentPath[currentPathIndex];
        }
    }

    private List<TunnelFunction> FindPath(
        TunnelFunction start,
        TunnelFunction goal)
    {
        Queue<TunnelFunction> queue = new();
        Dictionary<TunnelFunction, TunnelFunction> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            TunnelFunction current = queue.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, goal);
            }

            foreach (TunnelFunction next in current.TunnelConnections)
            {
                if (!cameFrom.ContainsKey(next))
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    private List<TunnelFunction> ReconstructPath(
        Dictionary<TunnelFunction, TunnelFunction> cameFrom,
        TunnelFunction end)
    {
        List<TunnelFunction> path = new();

        TunnelFunction current = end;

        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();

        return path;
    }

    private TunnelFunction FindClosestReachableTunnel(
        TunnelFunction destination)
    {
        Queue<TunnelFunction> queue = new();
        HashSet<TunnelFunction> visited = new();

        queue.Enqueue(currentTunnel);
        visited.Add(currentTunnel);

        TunnelFunction closest = currentTunnel;
        float closestDistance =
            Vector2.Distance(
                currentTunnel.transform.position,
                destination.transform.position);

        while (queue.Count > 0)
        {
            TunnelFunction current = queue.Dequeue();

            float dist =
                Vector2.Distance(
                    current.transform.position,
                    destination.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = current;
            }

            foreach (TunnelFunction next in current.TunnelConnections)
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        return closest;
    }

    // =========================
    // MOVEMENT
    // =========================

    private void MoveToTunnel()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetTunnel.transform.position;

        targetPos.z = currentPos.z;

        transform.position = Vector3.MoveTowards(
            currentPos,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        Vector2 dir = (targetPos - currentPos).normalized;

        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion baseRotation =
                Quaternion.Euler(0, 90f, -90f);

            Quaternion lookRotation =
                Quaternion.Euler(0, 0, angle);

            Quaternion targetRotation =
                lookRotation * baseRotation;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector2.Distance(transform.position, targetPos)
            <= reachDistance)
        {
            previousTunnel = currentTunnel;
            currentTunnel = targetTunnel;
            targetTunnel = null;
        }
    }

    // =========================
    // RANDOM WANDER
    // =========================

    private void ChooseNextTunnel()
    {
        List<TunnelFunction> connections =
            currentTunnel.TunnelConnections;

        if (connections == null || connections.Count == 0)
            return;

        List<TunnelFunction> possibleTunnels = new();

        foreach (TunnelFunction tunnel in connections)
        {
            if (tunnel != previousTunnel)
            {
                possibleTunnels.Add(tunnel);
            }
        }

        if (possibleTunnels.Count == 0)
        {
            possibleTunnels = connections;
        }

        int randomIndex =
            Random.Range(0, possibleTunnels.Count);

        targetTunnel = possibleTunnels[randomIndex];
    }

    // =========================
    // DETECTION
    // =========================

    private TunnelFunction FindCurrentTunnel()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            tunnelSearchRadius,
            tunnelLayer
        );

        foreach (Collider hit in hits)
        {
            TunnelFunction tunnel =
                hit.GetComponentInParent<TunnelFunction>();

            if (tunnel != null)
                return tunnel;
        }

        return null;
    }

    // =========================
    // DEBUG
    // =========================

    private void OnDrawGizmos()
    {
        if (targetTunnel != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                transform.position,
                targetTunnel.transform.position);
        }

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            tunnelSearchRadius);
    }
}