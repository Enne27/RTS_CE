using Unity.VisualScripting;
using UnityEngine;

public class AntCrazy : Ant
{
    private void Awake()
    {
        HP = 17f;
        armor = 0.35f;
        speed = 17f;
        strength = 2f;
        reach = 1;
        vision = 1;
        linePriority = 6;
        breedingCost = new int[] { 11, 12 };
        acidBased = false;
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
    }
}
