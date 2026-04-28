using System;
using UnityEngine;

public class AntExlporer : Ant
{
    public int food;
    public int MC;
    //int[] breedingCost = { 7, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    public Transform targetTransform;   
    private Vector3 targetPosition;
    public Vector3 antHillPosition;
    private bool useTransformTarget;

    public GameObject asignedResourceZone;
    private void Awake()
    {
        HP = 15f;
        armor = 0.40f;
        speed = 16f;
        strength = 1f;
        reach = 1;
        vision = 4;
        linePriority = 8;
        acidBased = false;
        antHillPosition = GameManager.instance.player.structures[0].transform.position;
    }

    public override void Attack(Ant target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= reach)
        {
            target.TakeDamage(this, strength, acidBased);
        }
    }
    public override void TakeDamage(Ant other, float strenght, bool acidBased)
    {
        float damageTaken;
        float acidArmor = 0.6f;
        if (other.GetAcidBased() == true)
        {
            damageTaken = other.GetStrength() - (acidArmor * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        else if (other.GetAcidBased() == false)
        {
            damageTaken = other.GetStrength() - (armor * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        if(HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public void Collect()
    {
        TimeManager.Instance.OneShotTimer(3f, () => 
        {
            food = UnityEngine.Random.Range(5, 11);
            MC = UnityEngine.Random.Range(1, 5);
            UnitController.MoveTo(this, antHillPosition);
            Debug.Log("Finished Collecting");
        });
    }
    public void Deposit()
    {
        TimeManager.Instance.OneShotTimer(3f, () => 
        {
            //Anthill.resources.food.add(food);
            //Anthill.resources.MC.add(MC);
            food = 0;
            MC = 0;
            UnitController.MoveTo(this, asignedResourceZone.transform.position);
        });
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
