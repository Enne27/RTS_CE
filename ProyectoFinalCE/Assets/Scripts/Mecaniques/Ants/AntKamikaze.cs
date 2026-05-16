using System;
using UnityEngine;
using static PlayerConstants;

public class AntKamikaze : Ant
{
    public static event Action<Ant> OnAnyAntDamaged;
    private float maxHP;
    private Ant target;
    //int[] breedingCost = new int[] { 7, 18 };
    private void Awake()
    {
        antType = ANT_TYPES.KAMIKAZE;
        HP = 11f;
        armor = 0.3f;
        speed = 13f;
        strength = 2f;
        reach = 1;
        vision = 5;
        linePriority = 2;
        acidBased = false;
        maxHP = HP;
        base.Awake();
    }

    public void Update()
    {
        if (HP < maxHP * 0.2f && target != null)
        {
            Explode(target);
        }
    }

    public override void Attack(Ant target)
    {
        if (target != null)
        {
            target.TakeDamage(this, GetEffectiveDamage(), acidBased);
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
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public void Explode(Ant target)
    {
        float explosionDamage = 15f;
        acidBased = true;
        float effectiveDamage = explosionDamage;
        
        if (SkillManager.Instance != null)
        {
            float damageBonus = SkillManager.Instance.GetTotalDamageBonus();
            effectiveDamage = explosionDamage * (1f + damageBonus);
        }
        
        target.TakeDamage(this, effectiveDamage, acidBased);
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
