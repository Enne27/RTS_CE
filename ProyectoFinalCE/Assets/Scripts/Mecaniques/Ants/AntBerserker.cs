    using UnityEngine;

public class AntBerserker : Ant
{
    int[] breedingCost = new int[] { 15, 30 };
    public static event Action<Ant> OnAnyAntDamaged;
    private void Awake()
    {
        HP = 80f;
        armor = 0.7f;
        speed = 7f;
        strength = 4f;
        reach = 1;
        vision = 1;
        linePriority = 2;
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
        float acidArmor = -0.2f;
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
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
