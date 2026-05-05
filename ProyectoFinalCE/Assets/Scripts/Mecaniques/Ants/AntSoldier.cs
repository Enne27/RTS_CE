using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.GraphicsBuffer;

internal class AntSoldier : Ant
{
    //int[] breedingCost = { 9, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    private void Awake()
    {
        HP = 25f;
        armor = 0.50f;
        speed = 12f;
        strength = 3f;
        reach = 1;
        vision = 1;
        linePriority = 2;
        acidBased = false;
    }

    public override void Attack(Ant target) {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= reach)
            {
                target.TakeDamage(this, strength, acidBased);
            }
        }
    }
    public override void TakeDamage(Ant other, float strenght, bool acidBased)
    {
        float damageTaken;
        float acidArmor = 0.65f;
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
        if (HP <= 0) {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public void AttackMound()
    {
        MoundFunction target;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= reach)
        {
            target.TakeDamage((int)Math.Round(strength));
        }
    }
    public override void Die()
    {
        gameObject.SetActive(false);    
    }   
}
 