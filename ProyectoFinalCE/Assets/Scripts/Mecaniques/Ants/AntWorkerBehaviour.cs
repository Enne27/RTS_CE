using StateMachine.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntWorkerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.75f;
    [SerializeField] private float reachDistance = 0.05f;
    [SerializeField] private float fastMoveSpeed = 2f;
    private float oldMoveSpeed;

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

    public StateMachineComponent stateMachineManager;
    private Animator animationController;

    private TunnelFunction previousTunnel;

    private bool isMoving;

    private List<TunnelFunction> currentPath = new();
    private int currentPathIndex = 0;

    private bool hasStartedConstruction;


    private TunnelFunction targetBuildingTunnel;

    [Header("Transport")]
    [SerializeField] public ForagingChamberFunction foragingChamber;
    [SerializeField] public StorageChamberFunction storageChamber;

    private bool isTransporting; 

    [SerializeField] private int carryAmount = 2;

    private bool carryingResources;

    private int carriedAmount;

    private ResourceType carriedType;

    private enum TransportPhase
    {
        None,
        GoingToForaging,
        GoingToStorage,
        Delivering
    }

    private TransportPhase transportPhase;

    private void Start()
    {
        stateMachineManager = GetComponent<StateMachineComponent>();
        animationController = GetComponent<Animator>();
        foragingChamber = FindFirstObjectByType<ForagingChamberFunction>();
        oldMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        isMoving = targetTunnel != null;
        animationController.SetBool("IsMoving", isMoving);

        if (stateMachineManager.GetCurrentStateName() == "Wander")
            Wander();
        else if (stateMachineManager.GetCurrentStateName() == "Working")
        {
            if (isTransporting)
                Transport();
            else
                Work();
        }
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

        TunnelFunction targetWorkTunnel = GetBestConstructionTunnel();

        if (targetWorkTunnel == null)
            return;

        // recalcular constantemente
        RecalculatePath(targetWorkTunnel);

        // ya llegamos
        if (currentTunnel == targetWorkTunnel)
        {
            targetTunnel = null;

            // mirar hacia la construcción
            LookAtTunnel(targetBuildingTunnel);

            ArriveAtWork();

            return;
        }

        // seguir path
        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    private void ArriveAtWork()
    {
        if (hasStartedConstruction)
            return;

        hasStartedConstruction = true;
        animationController.SetBool("IsWorking", true);
        VFXManager.Instance.PlayConstructionParticles(currentBuilding.transform.position, currentBuilding.data.constructionTime);
        StructuresPlayer chamber;
        TimeManager.Instance.OneShotTimer(
            currentBuilding.data.constructionTime,
            () =>
            {
                 chamber =
                    currentBuilding.
                    GetComponentInChildren<StructuresPlayer>();
                    

                if (chamber != null)
                {
                    chamber.currentStructureState = StructureState.OnConstruction;    
                    chamber.OnConstructionFinished();
                }

                HasFinishedWork();
                animationController.SetBool("IsWorking", false);

                currentBuilding = null;
                hasStartedConstruction = false;
                SeeIfAnyBuildIsWaiting();
            });
    }

    private TunnelFunction GetAccessibleTunnelFromBuilding(Building building)
    {
        TunnelFunction[] buildingTunnels =
            building.GetComponentsInChildren<TunnelFunction>();

        foreach (TunnelFunction tunnel in buildingTunnels)
        {
            if (tunnel == null)
                continue;

            if (tunnel.constructionAccessTunnel != null)
            {
                targetBuildingTunnel = tunnel;

                return tunnel.constructionAccessTunnel;
            }
        }

        return null;
    }

    private TunnelFunction GetBestConstructionTunnel()
    {
        TunnelFunction[] buildingTunnels =
            currentBuilding.GetComponentsInChildren<TunnelFunction>();

        foreach (TunnelFunction tunnel in buildingTunnels)
        {
            if (tunnel == null)
                continue;

            if (tunnel.isConstructingHerePosible &&
                tunnel.constructionAccessTunnel != null)
            {
                // guardamos el túnel del edificio
                targetBuildingTunnel = tunnel;

                // devolvemos el túnel exterior
                return tunnel.constructionAccessTunnel;
            }
        }

        return null;
    }
    private void LookAtTunnel(TunnelFunction tunnel)
    {
        if (tunnel == null)
            return;

        Vector3 dir =
            (tunnel.transform.position - transform.position).normalized;

        if (dir == Vector3.zero)
            return;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion baseRotation =
            Quaternion.Euler(0, 90f, -90f);

        Quaternion lookRotation =
            Quaternion.Euler(0, 0, angle);

        transform.rotation =
            lookRotation * baseRotation;
    }


    public void CallToBuild(Building buildToWork)
    {
        currentBuilding = buildToWork;
        buildToWork.GetComponentInChildren<StructuresPlayer>().workerWhoBuildThis = this;
        stateMachineManager.GetStateContext().workFinished = false;
        stateMachineManager.GetStateContext().hasWork = true;
    }

    public void CallToTransport()
    {
        stateMachineManager.GetStateContext().workFinished = false;
        stateMachineManager.GetStateContext().hasWork = true;

        isTransporting = true;

        transportPhase = TransportPhase.GoingToForaging;
    }

    private void Transport()
    {
        if (currentTunnel == null)
        {
            currentTunnel = FindCurrentTunnel();
            return;
        }

        TunnelFunction destination = null;

        switch (transportPhase)
        {
            case TransportPhase.GoingToForaging:

                destination = GetAccessibleTunnelFromBuilding(foragingChamber.GetComponentInParent<Building>());

                break;

            case TransportPhase.GoingToStorage:
                destination = GetAccessibleTunnelFromBuilding(storageChamber.GetComponentInParent<Building>());

                break;
        }

        // YA LLEGAMOS
        if (currentTunnel == destination)
        {
            targetTunnel = null;

            switch (transportPhase)
            {
                case TransportPhase.GoingToForaging:

                    PickResources();

                    if (!carryingResources)
                    {
                        FinishTransport();
                        return;
                    }

                    transportPhase =
                        TransportPhase.GoingToStorage;

                    break;

                case TransportPhase.GoingToStorage:

                    DeliverResources();

                    FinishTransport();

                    break;
            }

            return;
        }

        // SOLO recalcular si NO hemos llegado
        RecalculatePath(destination);

        if (targetTunnel != null)
        {
            MoveToTunnel();
        }
    }

    private void FinishTransport()
    {
        isTransporting = false;

        transportPhase = TransportPhase.None;

        carryingResources = false;

        targetTunnel = null;

        currentPath.Clear();

        HasFinishedWork();
    }

    private void PickResources()
    {
        carriedAmount = 0;
        carryingResources = false;

        List<ResourceType> availableResources = new();

        // Ver qué recursos existen
        if (foragingChamber.foods > 0)
            availableResources.Add(ResourceType.food);

        if (foragingChamber.materials > 0)
            availableResources.Add(ResourceType.material);

        // No hay nada disponible
        if (availableResources.Count == 0)
            return;

        // Elegir recurso aleatorio
        ResourceType selectedType =
            availableResources[
                Random.Range(0, availableResources.Count)];

        int availableAmount = 0;
        int freeSpace = 0;

        switch (selectedType)
        {
            case ResourceType.food:

                availableAmount = foragingChamber.foods;

                freeSpace =
                    storageChamber.FreeFoodSpace();

                break;

            case ResourceType.material:

                availableAmount = foragingChamber.materials;

                freeSpace =
                    storageChamber.FreeMaterialSpace();

                break;
        }

        // Cantidad REAL que puede llevar
        int amountToCarry =
            Mathf.Min(
                carryAmount,
                availableAmount,
                freeSpace);

        // El storage ya está lleno
        if (amountToCarry <= 0)
            return;

        bool removed =
            foragingChamber.RemoveResource(
                selectedType,
                amountToCarry);

        if (!removed)
            return;

        carriedType = selectedType;
        carriedAmount = amountToCarry;
        carryingResources = true;
    }

    private void DeliverResources()
    {
        if (!carryingResources || carriedAmount <= 0)
            return;

        switch (carriedType)
        {
            case ResourceType.food:

                storageChamber.FoodAcquired(carriedAmount);

                break;

            case ResourceType.material:

                storageChamber.MC_Acquired(carriedAmount);

                break;
        }

        carryingResources = false;
        carriedAmount = 0;
    }

    public void HasFinishedWork()
    {
        stateMachineManager.GetStateContext().hasWork = false;
        stateMachineManager.GetStateContext().workFinished = true;
        
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

    private void SeeIfAnyBuildIsWaiting()
    {
        if (BuildingManager.Instance.waitingToBeBuilt.Count < 1)
            return;

        Building build = BuildingManager.Instance.waitingToBeBuilt[0];
        CallToBuild(build);
        BuildingManager.Instance.waitingToBeBuilt.Remove(build);
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