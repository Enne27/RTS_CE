using UnityEngine;

public class ResourcesZone : MonoBehaviour
{
    #region VARIABLES
    [Header("MC")]
    [SerializeField, /*Range(0,1)*/] int randomMC_min;
    [SerializeField, /*Range(0,1)*/] int randomMC_max;

    [Header("Food")]
    [SerializeField] int randomFood_min;
    [SerializeField] int randomFood_max;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        int randomFoodFound = Random.Range(randomFood_min, randomFood_max+1);
        int randomMCFound = Random.Range(randomMC_min, randomMC_max+1);

        AntExlporer antExlporer = other.gameObject.GetComponent<AntExlporer>();
        if (antExlporer != null)
        {
            Debug.Log("Ant collecting MC: " + randomMCFound + " and food: " + randomFoodFound);
            antExlporer.Collect();
        }
        else Debug.Log("Not a explorer."); 
    }
}
