using System;
using TMPro;
using UnityEngine;

public class AntExlporer : Ant
{
    public int food;
    public int MC;
    //int[] breedingCost = { 7, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    public Transform targetTransform;   
    private Vector3 targetPosition;
    public Vector3 antHillPositionOwner;
    private bool useTransformTarget;   
    private void Awake()
    {
        HP = 15f;
        armor = 0.40f;
        speed = 16f;
        strength = 1f;
        reach = 1;
        vision = 4;
        linePriority = 8;
        acidBased = false;
        //antHillPositionOwner = GameManager.instance.player.structures[0].transform.position;
    }

    protected override void Move()
    {
        Vector3 target;

        if (useTransformTarget)
        {
            if (targetTransform == null) return;
            target = targetTransform.position;
        }
        else
        {
            target = targetPosition;
        }

        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
    public override void Attack(Ant target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= reach)
        {
            target.TakeDamage(this, strength, acidBased);
        }
        else
        {
            useTransformTarget = true;
            targetTransform = target.transform;
            Move();
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
        OnAnyAntDamaged?.Invoke(this);
    }

    public void Collect(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        if (distance > reach)
        {
        targetPosition = target;
        useTransformTarget = false;
        Move();
        }
        else
        {
        TimeManager.Instance.Register(3f, () => Collect(target));
        food = UnityEngine.Random.Range(5, 11);
        MC = UnityEngine.Random.Range(1,5);
        TimeManager.Instance.Unregister(3f, () => Collect(target));
        Carry();
        }
    }

    public void Carry()
    {
        useTransformTarget = false;
        targetPosition = antHillPositionOwner;
        Move();
        /*
            Dejar comida y materiales en la zona de forrajeo 
         */
        food = 0;
        MC = 0;
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
