using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssignAntHill : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] GameObject[] antHills; 
    #endregion

    private void Start()
    {
        AssignHill();
    }

    private void AssignHill()
    {
        if(antHills != null)
        {
            List<GameObject> antHillsNotOwned = new List<GameObject>(antHills);
            int i = Random.Range(0, antHillsNotOwned.Count);

            GameManager.instance.player.structures.Add(antHillsNotOwned[i]);
            GameManager.instance.player.structuresCount.Add(typeof(BuildingData), 1);
            antHillsNotOwned.RemoveAt(i);

            i = Random.Range(0, antHillsNotOwned.Count);
            GameManager.instance.playerIA.structures.Add(antHillsNotOwned[i]);
            GameManager.instance.playerIA.structuresCount.Add(typeof(BuildingData), 1);
            antHillsNotOwned.RemoveAt(i);
        }

        Debug.Log("Player Hill Position: " + GameManager.instance.player.structures[0].transform.position);
        Debug.Log("IA Hill Position: " + GameManager.instance.playerIA.structures[0].transform.position); ;
    }
}
