using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    static private HashSet<Ant> activeAnts;
    static private GameObject staticFlagPrefab;
    static private GameObject currentFlag;

    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject flagPrefab;

    [Header("Flocking")]
    [SerializeField] private float detectionRadius = 4.0f;

    [SerializeField] private float separationWeight = 3.5f;
    [SerializeField] private float alignmentWeight = 0.5f;
    [SerializeField] private float cohesionWeight = 0.3f;

    [SerializeField] private float arrivalRadius = 6.0f;

    [Header("Steering")]
    [SerializeField] private float steeringSmoothness = 6f;
    [SerializeField] private float rotationSpeed = 8f;

    private readonly List<Ant> antsToRemove = new List<Ant>();

    private void Awake()
    {
        activeAnts = new HashSet<Ant>();
        staticFlagPrefab = flagPrefab;
    }

    private void FixedUpdate()
    {
        antsToRemove.Clear();

        foreach (Ant ant in activeAnts)
        {
            if (ReachedDestination(ant))
            {
                antsToRemove.Add(ant);
                continue;
            }

            UpdateAnt(ant);
        }

        foreach (Ant ant in antsToRemove)
            activeAnts.Remove(ant);
    }


    public static void MoveTo(Ant ant, Vector3 objective)
    {
        // Snap the objective onto the terrain surface
        objective.y = 0; // will be resolved by SampleHeight at runtime
        ant.objective = objective;
        activeAnts.Add(ant);
        SpawnFlag(objective);
    }


    private void UpdateAnt(Ant ant)
    {
        Vector3 toGoal = ant.objective - ant.transform.position;
        toGoal.y = 0;

        float distToGoal = toGoal.magnitude;

        float speedScale = Mathf.Clamp01(distToGoal / arrivalRadius);

        Vector3 seekDir = distToGoal > 0.001f ? toGoal / distToGoal : Vector3.zero;

        Collider[] neighbours = GetNeighbours(ant.transform.position, ant);

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        if (neighbours.Length > 0)
        {
            separation = CalculateSeparation(neighbours, ant.transform.position);
            alignment = CalculateAlignment(neighbours);
            cohesion = CalculateCohesion(neighbours, ant.transform.position);
        }

        Vector3 desiredDirection = (
              seekDir
            + separation * separationWeight
            + alignment * alignmentWeight
            + cohesion * cohesionWeight
        );

        desiredDirection.y = 0;

        if (desiredDirection.sqrMagnitude > 0.001f)
            desiredDirection.Normalize();

        ant.currentVelocity = Vector3.Lerp(
            ant.currentVelocity,
            desiredDirection,
            steeringSmoothness * Time.fixedDeltaTime
        );
        ant.currentVelocity.y = 0;

        if (ant.currentVelocity.sqrMagnitude > 0.001f)
            ant.currentVelocity.Normalize();

        float speed = ant.GetSpeed() * speedScale;
        Vector3 newPos = ant.transform.position + ant.currentVelocity * speed * Time.fixedDeltaTime;

        newPos.y = terrain.SampleHeight(newPos) + terrain.transform.position.y;
        ant.transform.position = newPos;

        Vector3 flatVelocity = ant.currentVelocity;
        flatVelocity.y = 0;

        if (flatVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
            ant.transform.rotation = Quaternion.Slerp(
                ant.transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }


    private bool ReachedDestination(Ant ant)
    {
        Vector3 dir = ant.objective - ant.transform.position;
        dir.y = 0;
        return dir.sqrMagnitude < arrivalRadius * arrivalRadius;
    }


    /// <summary>
    /// Push away from neighbours. Uses inverse-distance weighting
    /// so ants that are very close exert a stronger push.
    /// </summary>
    private Vector3 CalculateSeparation(Collider[] neighbours, Vector3 position)
    {
        Vector3 force = Vector3.zero;

        foreach (Collider neighbour in neighbours)
        {
            Vector3 away = position - neighbour.transform.position;
            away.y = 0;

            float distance = away.magnitude;

            if (distance > 0.001f)
            {
                float weight = 1f / Mathf.Max(distance * distance, 0.01f);
                force += away.normalized * weight;
            }
        }

        return force.sqrMagnitude > 0.001f ? force.normalized : Vector3.zero;
    }

    /// <summary>
    /// Steer toward the average heading of neighbours.
    /// </summary>
    private Vector3 CalculateAlignment(Collider[] neighbours)
    {
        Vector3 avgForward = Vector3.zero;

        foreach (Collider neighbour in neighbours)
        {
            Vector3 fwd = neighbour.transform.forward;
            fwd.y = 0;
            avgForward += fwd;
        }

        return avgForward.sqrMagnitude > 0.001f ? avgForward.normalized : Vector3.zero;
    }

    /// <summary>
    /// Steer toward the centre of mass of neighbours.
    /// </summary>
    private Vector3 CalculateCohesion(Collider[] neighbours, Vector3 position)
    {
        Vector3 center = Vector3.zero;

        foreach (Collider neighbour in neighbours)
        {
            Vector3 pos = neighbour.transform.position;
            pos.y = 0;
            center += pos;
        }

        center /= neighbours.Length;

        Vector3 toCenter = center - new Vector3(position.x, 0, position.z);

        return toCenter.sqrMagnitude > 0.001f ? toCenter.normalized : Vector3.zero;
    }


    private Collider[] GetNeighbours(Vector3 position, Ant self)
    {
        int mask = LayerMask.GetMask("Ant");
        Collider[] hits = Physics.OverlapSphere(position, detectionRadius, mask);

        List<Collider> neighbours = new List<Collider>(hits.Length);

        foreach (Collider hit in hits)
        {
            if (hit.transform != self.transform)
                neighbours.Add(hit);
        }

        return neighbours.ToArray();
    }

    private static void SpawnFlag(Vector3 objective)
    {
        if (currentFlag == null)
            currentFlag = Instantiate(staticFlagPrefab, objective, staticFlagPrefab.transform.rotation);

        currentFlag.transform.position = objective;
    }
}