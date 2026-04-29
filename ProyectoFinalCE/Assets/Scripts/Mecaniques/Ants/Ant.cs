using System;
using UnityEngine;
public abstract class Ant : MonoBehaviour 
{
    public static event Action<Ant> OnAnyAntDamaged;
    protected float HP; 
    protected float armor;
    protected float speed; 
    protected float strength; 
    protected int reach; 
    protected int vision; 
    protected int linePriority; 
    public int[] breedingCost = new int[2];
    protected bool acidBased;

    //protected virtual void Move() { }
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }
    //protected virtual void SpawnAnt() { }


    public float GetCurrentHP()
    {
        return HP;
    }
    protected virtual void Die() { }

    public float GetStrength()
    {
        return strength;
    }

    public bool GetAcidBased()
    {
        return acidBased;
    }

    public int[] GetBreedingCost()
    {
        return breedingCost;
    }
    public int GetVision()
    { 
        return vision;
    }

    private bool hasObjective = false;
    private Vector3 objective;

    private void FixedUpdate()
    {
        if (hasObjective)
        {
            Vector3 direction = (objective - transform.position).normalized;
            Vector3 newPos = transform.position + direction * speed * Time.fixedDeltaTime;
            transform.position = newPos;
        }
    }

    public void MoveTo(Vector3 _objective)
    {
        hasObjective = true;
        objective = _objective;
    }

    public void StopMove()
    {
        hasObjective = false;
    }
}