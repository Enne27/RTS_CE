using UnityEngine;

public class AttackMound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            ant.AttackMound(gameObject);
            ant.anthillContact = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            ant.anthillContact = false;
        }
    }
}
