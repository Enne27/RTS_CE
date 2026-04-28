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
        foreach (Ant ant in activeAnts)
        {
            Vector3 direction = (ant.objective - ant.transform.position).normalized;
            Vector3 newPos = ant.transform.position + direction * ant.GetSpeed() * Time.fixedDeltaTime;
            newPos.y = terrain.SampleHeight(newPos) + terrain.transform.position.y;
            ant.transform.position = newPos;
            if (ant.transform.position == ant.objective) activeAnts.Remove(ant);

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