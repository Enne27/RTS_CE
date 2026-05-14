using System;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerConstants;

public class AntCrazy : Ant
{
    //int[] breedingCost = new int[] { 11, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    private void Awake()
    {
        antType = ANT_TYPES.CRAZY;
        HP = 17f;
        armor = 0.35f;
        speed = 17f;
        strength = 2f;
        reach = 1;
        vision = 5;
        linePriority = 6;
        acidBased = false;
        base.Awake();
    }

    public override void Attack(Ant target)
    {
        if (target != null)
        {
            target.TakeDamage(target, GetEffectiveDamage(), acidBased);
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
        Destroy(gameObject);
    }
}
