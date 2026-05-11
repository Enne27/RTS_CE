using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static PlayerConstants;

public abstract class Ant : MonoBehaviour
{
    public static event Action<Ant> OnAnyAntDamaged;

    [Header("Save Data")]
    public ANT_TYPES antType;

    protected float HP;
    protected float armor;
    protected float speed;
    protected float strength;
    protected int reach;
    protected int vision;
    protected int linePriority;

    public int[] breedingCost = new int[2];

    protected bool acidBased;

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