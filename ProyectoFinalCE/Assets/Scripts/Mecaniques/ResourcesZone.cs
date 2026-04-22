using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ResourcesZone : MonoBehaviour
{
    #region VARIABLES
        Vector3 location;
    #endregion

    private void Awake()
    {
        location = gameObject.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        AntExlporer antExlporer = other.gameObject.GetComponent<AntExlporer>();
        if (antExlporer != null)
        {
            antExlporer.Collect(location);
        }
        else Debug.Log("Not a explorer."); 
    }
}
