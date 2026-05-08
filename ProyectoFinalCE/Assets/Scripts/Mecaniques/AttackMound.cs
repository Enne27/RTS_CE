using UnityEngine;

public class AttackMound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            ant.AttackMound();
        }
        else Debug.Log("Not an Ant.");
    }
}
