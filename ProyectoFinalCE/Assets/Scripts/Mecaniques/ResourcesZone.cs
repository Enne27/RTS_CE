using UnityEngine;

public class ResourcesZone : MonoBehaviour
{
    #region VARIABLES
        Vector3 location;
        GameHUDView hudView;


    #endregion

    private void Awake()
    {
        location = gameObject.transform.position;
        hudView = FindFirstObjectByType<GameHUDView>();
    }
    private void OnTriggerEnter(Collider other)
    {
        AntExlporer antExlporer = other.gameObject.GetComponent<AntExlporer>();
        if (antExlporer != null)
        {
            antExlporer.Collect();
        }
        //else Debug.Log("Not a explorer."); 
    }
}
