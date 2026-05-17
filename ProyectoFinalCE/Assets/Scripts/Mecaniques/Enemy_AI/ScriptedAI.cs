
/*
 *     private void sendStarterAnts()
    {
        foreach (var ant in GameManager.instance.playerIA.ants)
        {
            if(ant is AntExlporer antExplorer)
            {
                antExplorer.asignedResourceZone = zonaRecursos;
                UnitController.MoveTo(antExplorer, zonaRecursos.transform.position);
            }
        }
    }
 */
using UnityEngine;

using static PlayerConstants;

public class ScriptedAI : MonoBehaviour
{
    [SerializeField] private GameObject[] zonaRecursos;

    [SerializeField] Transform spawnPoint;




    private void Start()
    {
        TimeManager.Instance.OneShotTimer(45, () => TimeManager.Instance.Register(1, createAnts));
    }

    private void Update()
    {
        AntsActions();
    }

    private void AntsActions()
    {
        foreach (var ant in GameManager.instance.playerIA.ants)
        {
            if (UnitController.activeAnts.Contains(ant)) continue;
            //if (ant.objective != null) continue;

            if (ant is AntExlporer antExplorer)
            {
                antExplorer.asignedResourceZone = zonaRecursos[0];
                UnitController.MoveTo(antExplorer, zonaRecursos[Random.Range(0, zonaRecursos.Length)].transform.position);
                continue;
            }

            int paDonde = Random.Range(0, 4);
            if (paDonde > 1)
            {
                //select random ant
                var playerAnts = GameManager.instance.player.ants;
                if (playerAnts.Count > 0)
                {
                    //Atack random ant selected
                    var randomAnt = playerAnts[Random.Range(0, playerAnts.Count)];
                    continue;
                }
            }

            foreach (var structure in GameManager.instance.player.structures)
            {
                if (structure.GetComponent<Anthill>() != null)
                {
                    //attack anthill
                    UnitController.MoveTo(ant, structure.transform.position);
                }
            }
        }
    }

    ANT_TYPES antType = ANT_TYPES.ACID;
    int amountTypes = 6;
    private void createAnts()
    {
        antType = (ANT_TYPES)((int)++antType % amountTypes);
        AntCreation.Instance.AIAntCreation(antType, spawnPoint, 1);
    }
}
