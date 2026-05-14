using System;
using UnityEngine;

public class AntAcidSpewer : Ant
{
    //int[] breedingCost = new int[] { 10, 18 };
    public static event Action<Ant> OnAnyAntDamaged;

    private void Awake()
    {
        /*HP = 15f;
        armor = 0.30f;
        speed = 10f;
        strength = 5f;
        reach = 9;
        vision = 1;
        linePriority = 10;
        acidBased = true;*/
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
        damageTaken = other.GetStrength() - (armor * other.GetStrength());
        damageTaken = Mathf.Max(0, damageTaken);
        HP -= damageTaken;
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public override void AttackMound(GameObject mound)
    {
        MoundFunction target;
        //La trucada del AttackMound no pasa el if
        if (antOwner == Owner.Player && mound.CompareTag("AI_AntHill") || antOwner == Owner.AI && gameObject.CompareTag("Player_AntHill"))
        {
            target = mound.GetComponent<MoundFunction>();
            target.TakeDamage((int)Math.Round(strength), antOwner);
            CheckMoundTrigger(mound);
        }

        else
        {
            return;
        }
    }
    public void CheckMoundTrigger(GameObject mound)
    {
        if (anthillContact == true)
        {
            TimeManager.Instance.OneShotTimer(3f, () => AttackMound(mound));
        }
        else
            return;
    }
    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
