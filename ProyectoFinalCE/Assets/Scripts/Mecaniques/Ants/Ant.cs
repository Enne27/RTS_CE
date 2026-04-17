using UnityEngine;
public abstract class Ant : MonoBehaviour 
{ 
    protected float HP; 
    protected float armor;
    protected float speed; 
    protected float strength; 
    protected int reach; 
    protected int vision; 
    protected int linePriority; 
    protected int[] breedingCost = new int[2];
    protected bool acidBased;
    
    protected virtual void Move() { }
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }
    protected virtual void Carry() { }
    protected virtual void SpawnAnt() { }

    protected virtual void Die() { }

    public float GetStrength()
    {
        return strength;
    }

    public bool GetAcidBased()
    {
        return acidBased;
    }
}