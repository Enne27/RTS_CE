using UnityEngine;

public class AttackMound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("EOOOO");

        Ant ant = other.gameObject.GetComponent<Ant>();
        if (ant != null)
        {
            Debug.Log("Estoy atacando el hormiguero");
            ant.AttackMound();
        }
        else Debug.Log("Not an Ant.");
    }
}
