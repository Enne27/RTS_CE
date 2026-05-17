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
                int Randomplace = Random.Range(0, zonaRecursos.Length);
                antExplorer.asignedResourceZone = zonaRecursos[Randomplace];
                UnitController.MoveToAi(antExplorer, zonaRecursos[Randomplace].transform.position);
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
                    UnitController.MoveToAi(ant, structure.transform.position);
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
