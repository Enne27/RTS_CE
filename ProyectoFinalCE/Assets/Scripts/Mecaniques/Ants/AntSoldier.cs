using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static PlayerConstants;

internal class AntSoldier : Ant
{
    //int[] breedingCost = { 9, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    //public Owner antOwner;
    private void Awake()
    {
        antType = ANT_TYPES.SOLDIER;
        HP = 25f;
        armor = 0.50f;
        speed = 12f;
        strength = 3f;
        reach = 1;
        vision = 5;
        linePriority = 2;
        acidBased = false;
        base.Awake();
    }

    public override void Attack(Ant target) {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= reach)
            {
                target.TakeDamage(this, GetEffectiveDamage(), acidBased);
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

    public override void AttackMound(GameObject mound)
    {
        MoundFunction target;
        if (/*Owner == Owner.Player &&*/ mound.CompareTag("AI_AntHill")||/*Owner == Owner.AI &&*/ mound.CompareTag("Player_AntHill"))
        {
            target = GetComponent<MoundFunction>();
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= reach)
            {
                target.TakeDamage((int)Math.Round(GetEffectiveDamage()), antOwner);
            }
        }

        else
        {
            Debug.Log("Este objeto no es el hormiguero");
            return;
        }
    }
    public override void Die()
    {
        gameObject.SetActive(false);    
    }   
}
 