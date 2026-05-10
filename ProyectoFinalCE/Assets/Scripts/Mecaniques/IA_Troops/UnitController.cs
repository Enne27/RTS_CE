using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    static private HashSet<Ant> activeAnts;
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject flagPrefab;
    static private GameObject staticFlagPrefab;


    private void Awake()
    {
        activeAnts = new HashSet<Ant>();
        staticFlagPrefab = flagPrefab;
    }

    private void FixedUpdate()
    {
        float separationRadius = 2f;
        float separationStrength = 3f;

        foreach (Ant ant in activeAnts)
        {
            Vector3 separation = Vector3.zero;
            foreach (Ant other in activeAnts)
            {
                if (ant == other) continue;
                float dist = Vector3.Distance(ant.transform.position, other.transform.position);
                if (dist < separationRadius && dist > 0.001f)
                {
                    Vector3 away = (ant.transform.position - other.transform.position).normalized;

                    // Stronger when closer
                    separation += away / dist;
                }
                separation *= separationStrength;

            }

            Vector3 direction = (ant.objective - ant.transform.position).normalized;
            Vector3 finalDir = (direction + separation).normalized;
            Vector3 newPos = ant.transform.position + finalDir * ant.GetSpeed() * Time.fixedDeltaTime;
            newPos.y = terrain.SampleHeight(newPos) + terrain.transform.position.y;
            ant.transform.LookAt(direction);
            ant.transform.position = newPos;

            //Agregar simulacion de flocking con hormigas cercanas.
            //Agregar comparacion de objetivos entre las hormigas cercanas.
            //Agregar evasion de obstaculos
        }
    }

    public static void MoveTo(Ant ant, Vector3 objective)
    {
        ant.objective = objective;
        activeAnts.Add(ant);
        spawnFlag();
    }

    private static void spawnFlag()
    {
        Instantiate(
           staticFlagPrefab,
           objective,
           Quaternion.identity
       );
    }
} 