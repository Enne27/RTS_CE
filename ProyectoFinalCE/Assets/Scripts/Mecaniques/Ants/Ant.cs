using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    protected bool hasObjective = false;

    public int flowFieldIndex;
    public Vector3 currentVelocity;
    public Vector3 objective;
    Material material;
    Color defaultColor;


    #region COMBAT
    public virtual void Attack(Ant target) { }
    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }
    #endregion

    //Para el guardado al cargar:
    public void SetHP(float value)
    {
        HP = value;
    }
    
    #region GETTERS
    public float GetCurrentHP()
    {
        return HP;
    }

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

    #region MOVEMENT LOGIC
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
    #endregion
}