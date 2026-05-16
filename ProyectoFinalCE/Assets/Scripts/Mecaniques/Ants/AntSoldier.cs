using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.GraphicsBuffer;

internal class AntSoldier : Ant
{
    //int[] breedingCost = { 9, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    Ant currentTarget;
    private void Awake()
    {
        //HP = 25f;
        //armor = 0.50f;
        //speed = 12f;
        //strength = 3f;
        //reach = 1;
        //vision = 1;
        //linePriority = 2;
        //acidBased = false;
    }
    private void FixedUpdate()
    {
        if (currentTarget != null)
        {
            IsRange(currentTarget);
        }
    }
    public override void IsRange(Ant target)
    {
        if (target == null)
            return;
        currentTarget = target;
        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        if (distance <= reach)
        {
            Debug.Log("Estoy en el rango");
            Attack(target);
        }
        else
        {
            UnitController.MoveTo(this, target.gameObject);
            Debug.Log("Sigue corriendo, sigue corriendo");
        }
    }
    public override void Attack(Ant target) {
        if (target != null)
        {
                target.TakeDamage(this, strength, acidBased);
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
 