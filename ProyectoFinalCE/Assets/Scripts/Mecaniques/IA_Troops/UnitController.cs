using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    static private HashSet<Ant> activeAnts;
    [SerializeField] private Terrain terrain;


    private void Awake()
    {
        activeAnts = new HashSet<Ant>();
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

            }
            separation *= separationStrength;

            //float avoidDistance = 10f;
            //float avoidStrength = 15f;

            //Vector3 avoidance = Vector3.zero;

            //int obstacleMask = LayerMask.GetMask("Obstacle");
            //RaycastHit hit;
            //if (Physics.Raycast(ant.transform.position, ant.transform.forward, out hit, avoidDistance, obstacleMask))
            //{
            //    Debug.Log(hit.transform.gameObject);
            //    Vector3 reflect = Vector3.Reflect(ant.transform.forward, hit.normal);

            //    avoidance = reflect.normalized * avoidStrength;
            //}

            Vector3 direction = (ant.objective - ant.transform.position).normalized;
            Vector3 finalDir = (direction + separation/* + avoidance*/).normalized;
            Vector3 newPos = ant.transform.position + finalDir * ant.GetSpeed() * Time.fixedDeltaTime;
            newPos.y = terrain.SampleHeight(newPos) + terrain.transform.position.y;
            ant.transform.LookAt(ant.transform.position + finalDir);
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
    }
} 