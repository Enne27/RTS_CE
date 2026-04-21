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
        int randomFoodFound = Random.Range(randomFood_min+1, randomFood_max+1);
        int randomMCFound = Random.Range(randomMC_min+1, randomMC_max+1);

        if (other.gameObject.GetComponent<AntExlporer>())
        {
            Debug.Log("Ant collecting MC: " + randomMCFound + " and food: " + randomFoodFound);
        }
        else Debug.Log("Not a explorer."); 
    }
}
