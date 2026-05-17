using UnityEngine;

public class AttackMound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            //Debug.Log("I am attacking the mound");
            ant.AttackMound(gameObject);
        }
        //else Debug.Log("Not an Ant.");
    }
}
