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
    
    public int flowFieldInxex;
    public Vector3 currentVelocity;
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }

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
}