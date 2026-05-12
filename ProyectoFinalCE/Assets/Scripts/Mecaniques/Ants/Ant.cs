using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static PlayerConstants;

public abstract class Ant : MonoBehaviour
{
    public static event Action<Ant> OnAnyAntDamaged;

    [Header("Save Data")]
    public ANT_TYPES antType;

    public float HP; 
    public float armor;
    public float speed; 
    public float strength; 
    public int reach; 
    public int vision; 
    public int linePriority; 
    public int[] breedingCost = new int[2];

    protected bool acidBased;
    public Owner antOwner;

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

    #region SAVE / LOAD

    // Para el sistema de guardado
    public void SetHP(float value)
    {
        HP = value;
    }

    #endregion

    #region GETTERS

    public float GetCurrentHP()
    {
        return HP;
    }


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

            if (material.HasProperty("_Outline_Color"))
                defaultColor = material.GetColor("_Outline_Color");
        }

        if (material.HasProperty("_Outline_Color"))
            material.SetColor("_Outline_Color", color);
    }

    public void setDefaultOutline()
    {
        if (material == null)
        {
            material = GetComponent<Renderer>().material;

            if (material.HasProperty("_Outline_Color"))
                defaultColor = material.GetColor("_Outline_Color");
        }

        if (material.HasProperty("_Outline_Color"))
            material.SetColor("_Outline_Color", defaultColor);
    }

    #endregion

    #region MOVEMENT LOGIC

    private void FixedUpdate()
    {
        if (!hasObjective)
            return;

        Vector3 direction = (objective - transform.position).normalized;

        Vector3 newPos =
            transform.position +
            direction * speed * Time.fixedDeltaTime;

        transform.position = newPos;
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