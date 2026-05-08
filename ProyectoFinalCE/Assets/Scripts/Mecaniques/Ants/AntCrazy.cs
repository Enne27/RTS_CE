using System;
using Unity.VisualScripting;
using UnityEngine;

public class AntCrazy : Ant
{
    //int[] breedingCost = new int[] { 11, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    private void Awake()
    {
        //hp = 17f;
        //armor = 0.35f;
        //speed = 17f;
        //strength = 2f;
        //reach = 1;
        //vision = 1;
        //linepriority = 6;
        //acidbased = false;
    }

    public override void Attack(Ant target)
    {
        if (target != null)
        {
            target.TakeDamage(target, strength, acidBased);
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
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
