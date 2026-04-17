using UnityEngine;

public class AntExlporer : Ant
{
    private int food;
    private int constructionMaterial;
    private void Awake()
    {
        HP = 15f;
        armor = 0.40f;
        speed = 16f;
        strength = 1f;
        reach = 1;
        vision = 4;
        linePriority = 8;
        breedingCost = new int[] { 7, 12 };
        acidBased = false;
    }

    protected override void Move()
    {

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
    }

    public void Collect()
    {
        /*
         Cooldown
        se termina
        se añaden recursos
        se llama Carry()
         */
    }

    public void Carry()
    {
        /*
         Move objetivo punt
        Collect()
         */
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
