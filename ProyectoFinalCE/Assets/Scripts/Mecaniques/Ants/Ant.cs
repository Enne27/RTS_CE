using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public abstract class Ant : MonoBehaviour 
{
    public static event Action<Ant> OnAnyAntDamaged;
    public float HP; 
    public float armor;
    public float speed; 
    public float strength; 
    public int reach; 
    public int vision; 
    protected int linePriority; 
    public int[] breedingCost = new int[2];
    protected bool acidBased;
    
    public int flowFieldInxex;
    public Vector3 currentVelocity;
    public Vector3 objective;

    public float GetCurrentHP()
    {
        return HP;
    }
    
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }

    public virtual void Die() { }

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

    public float GetSpeed()
    {
        return speed;
    }

    public float GetArmor()
    {
        return armor;
    }
}