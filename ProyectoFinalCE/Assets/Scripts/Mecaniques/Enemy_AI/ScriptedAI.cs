using UnityEngine;

public class ScriptedAI : MonoBehaviour
{
    [SerializeField]private GameObject zonaRecursos;

    private void Start()
    {
        TimeManager.Instance.OneShotTimer(3, sendStarterAnts);   
    }

    private void Update()
    {
        //GameManager.instance.playerIA.ants;
        //GameManager.instance.playerIA.inventory;
        //GameManager.instance.playerIA.structures;
        //GameManager.instance.playerIA.currentEra;
    }

    private void sendStarterAnts()
    {
        foreach (var ant in GameManager.instance.playerIA.ants)
        {
            if(ant is AntExlporer antExplorer)
            {
                antExplorer.asignedResourceZone = zonaRecursos;
                UnitController.MoveTo(antExplorer, zonaRecursos);
            }
        }
    }
}
