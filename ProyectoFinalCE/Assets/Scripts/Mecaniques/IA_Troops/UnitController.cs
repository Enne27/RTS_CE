using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    static private HashSet<Ant> activeAnts;
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject flagPrefab;
    static private GameObject staticFlagPrefab;
    static private GameObject currentFlag;




    private Vector3 separationForce;
    private List<Ant> antsToRemove = new List<Ant>();

    [Header("Boids")]
    [SerializeField] private float steeringSmoothness = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float m_SeparationWeight = 2f;
    //[SerializeField] private float m_AlignmentWeight = 1.0f;
    //[SerializeField] private float m_CohesionWeight = 1.0f;
    [SerializeField] float detectionRadius = 3.0f;
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
            UpdateAnt(ant);

            if (ReachedDestination(ant))
            {
                antsToRemove.Add(ant);
                continue;
            }
        }

        foreach (Ant ant in antsToRemove)
        {
            activeAnts.Remove(ant);
        }
    }


    public static void MoveTo(Ant ant, Vector3 objective)
    {
        ant.objective = objective;
        activeAnts.Add(ant);
        SpawnFlag(objective);
    }
    private bool ReachedDestination(Ant ant)
    {
        Vector3 dir = ant.objective - ant.transform.position;
        dir.y = 0;

        return dir.sqrMagnitude < 1.5f * 1.5f;
    }

    private void UpdateAnt(Ant ant)
    {
        separationForce = Vector3.zero;

        Vector3 direction = ant.objective - ant.transform.position;

        direction.y = 0;

        Collider[] neighbours = GetNeighbours(ant.transform.position, ant);

        if (neighbours.Length > 0)
        {
            CalculateSeparationForces(neighbours, ant.transform.position);

            ApplyAllignment(neighbours, ant.transform.position);
        }

        direction.Normalize();

        Vector3 desiredDirection =
            (direction + separationForce).normalized;

        ant.currentVelocity = Vector3.Lerp(
            ant.currentVelocity,
            desiredDirection,
            steeringSmoothness * Time.fixedDeltaTime
        );

        ant.currentVelocity.y = 0;

        if (ant.currentVelocity != Vector3.zero)
        {
            ant.currentVelocity.Normalize();
        }

        Vector3 movement =
            ant.currentVelocity
            * ant.GetSpeed()
            * Time.fixedDeltaTime;

        Vector3 newPos =
            ant.transform.position + movement;

        newPos.y =
            terrain.SampleHeight(newPos)
            + terrain.transform.position.y;

        ant.transform.position = newPos;

        if (ant.currentVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(ant.currentVelocity);

            ant.transform.rotation =
                Quaternion.Slerp(
                    ant.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
        }
    }

    private static void SpawnFlag(Vector3 objective)
    {
        if (currentFlag == null)
        {
            currentFlag = Instantiate(staticFlagPrefab, objective, staticFlagPrefab.transform.rotation);
        }

        currentFlag.transform.position = objective;
    }

    private void CalculateSeparationForces(Collider[] neightbours, Vector3 position)
    {
        foreach (Collider neightbour in neightbours)
        {
            var dir = neightbour.transform.position - position;
            var distance = dir.magnitude;
            var away = -dir.normalized;

            if(distance > 0)
            {
                separationForce += (away / distance) * m_SeparationWeight;
            }
        }
    }
    
    private void ApplyAllignment(Collider[] neightbours, Vector3 position)
    {
        Vector3 neightboursForward = Vector3.zero;
        foreach (Collider neightbour in neightbours)
        {
            neightboursForward += neightbour.transform.forward;
        }

        if(neightboursForward != Vector3.zero)
        {
            neightboursForward.Normalize();
        }

        separationForce += neightboursForward;
    }

    private Collider[] GetNeighbours(Vector3 position, Ant self)
    {
        int mask = LayerMask.GetMask("Ant");

        Collider[] hits =
            Physics.OverlapSphere(position, detectionRadius, mask);

        List<Collider> neighbours = new List<Collider>();

        foreach (Collider hit in hits)
        {
            if (hit.transform != self.transform)
            {
                neighbours.Add(hit);
            }
        }

        return neighbours.ToArray();
    }
} 