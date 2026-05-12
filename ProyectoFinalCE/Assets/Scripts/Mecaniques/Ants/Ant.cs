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
    public int linePriority; 
    public int[] breedingCost = new int[2];
    protected bool acidBased;
    
    public int flowFieldInxex;
    public Vector3 currentVelocity;
    public Vector3 objective;
    Material material;
    Color defaultColor;

    protected bool hasObjective = false;

    #region COMBAT
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }

    public virtual void Die() { }

    public virtual void AttackMound(GameObject mound) { }
    #endregion

    #region GETTERS
    public float GetStrength()
    {
        return strength;
    }
    public float GetCurrentHP()
    {
        return HP;
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
    #endregion

    #region OUTLINE 
    public void setOutline(Color color)
    {
        if (material == null)
        {
            material = GetComponent<Renderer>().material;
            defaultColor = material.GetColor("_Outline_Color");
        }
        material.SetColor("_Outline_Color", color);
    }
    public void setDefaultOutline()
    {
        if (material == null)
        {
            material = GetComponent<Renderer>().material;
            defaultColor = material.GetColor("_Outline_Color");
        }
        material.SetColor("_Outline_Color", defaultColor);
    }
    #endregion

}