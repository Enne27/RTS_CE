using System.Collections.Generic;
using UnityEngine;

//Temporal class ant
//Remove it when class ant exists
public class Ant 
{
    public void Gather()
    {

    }
}

public class ZonaRecursos : MonoBehaviour
{
    private List<Ant> farmingAnts = new List<Ant>();
    private const int gatherTime = 5;
    private void Update()
    {
        foreach (var ant in farmingAnts)
        {
            TimeManager.Instance.Register(gatherTime, ant.Gather);
        }
    }

    public void AntStartFarming(Ant ant)
    {
        farmingAnts.Add(ant);
    }
    public void AntStopFarming(Ant ant)
    {
        farmingAnts.Remove(ant);
    }
}
public class ZonasRecursos : MonoBehaviour
{

    #region Singleton
    public static ZonasRecursos Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    private Vector2Int[] locations = new Vector2Int[/*GameManager.instance.startingZR*/5];
    private const int gatherTime = 5;
    //private List<Ant> farmingAnts = new List<Ant>();

    public void AntStartFarming(Ant ant)
    {
        TimeManager.Instance.Register(gatherTime, ant.Gather);
        //farmingAnts.Add(ant);
    }
    public void AntStopFarming(Ant ant)
    {
        TimeManager.Instance.Unregister(gatherTime, ant.Gather);
        //farmingAnts.Remove(ant);
    }
}
