using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class AntKamikaze : Ant
{
    private float maxHP;
    private Ant target;
    private void Awake()
    {
        HP = 11f;
        armor = 0.3f;
        speed = 13f;
        strength = 2f;
        reach = 1;
        vision = 1;
        linePriority = 2;
        breedingCost = new int[] { 7, 18 };
        acidBased = false;
        maxHP = HP;
}

    public void Update()
    {
        if (HP < maxHP * 0.2f && target != null)
        {
            Explode(target);
        }
    }


    protected override void Move()
    {

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
    }

    public void Explode(Ant target)
    {
        strength = 10;
        acidBased = true;
        target.TakeDamage(target,strength,acidBased);
    }
}
