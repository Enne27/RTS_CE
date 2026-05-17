using System;
using UnityEngine;
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

    private float baseHP;
    private float baseArmor;
    private float baseSpeed;
    private float baseStrength;
    private int baseReach;
    private int baseVision;

    protected bool acidBased;
    public Owner antOwner;

    protected bool hasObjective = false;

    public int flowFieldIndex;
    public Vector3 currentVelocity;
    public Vector3 objective;

    private Material material;
    private Color defaultColor;

    #region COMBAT

    public virtual void Attack(Ant target) { }

    public virtual void TakeDamage(Ant other, float strength, bool acidBased) { }

    public virtual void AttackMound(GameObject mound)
    {
        MoundFunction target;
        //La trucada del AttackMound no pasa el if
        if (antOwner == Owner.Player && mound.CompareTag("AI_AntHill") || antOwner == Owner.AI && gameObject.CompareTag("Player_AntHill"))
        {
            target = mound.GetComponent<MoundFunction>();
            target.TakeDamage((int)Math.Round(strength), antOwner);
        }
    }
    public virtual void Die() { }

    #endregion

    #region SAVE LOAD

    public void SetHP(float value)
    {
        HP = value;
    }

    public void SetArmor(float value)
    {
        armor = value;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    public void SetStrength(float value)
    {
        strength = value;
    }

    public void SetReach(int value)
    {
        reach = value;
    }

    public void SetVision(int value)
    {
        vision = value;
    }

    public void SetLinePriority(int value)
    {
        linePriority = value;
    }

    public void SetBreedingCost(int[] value)
    {
        breedingCost = value;
    }

    public void SetAcidBased(bool value)
    {
        acidBased = value;
    }

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

    public int GetReach()
    {
        return reach;
    }

    public int GetLinePriority()
    {
        return linePriority;
    }

    public float GetEffectiveDamage()
    {
        float baseDamage = strength;
        
        if (SkillManager.Instance != null)
        {
            float damageBonus = SkillManager.Instance.GetTotalDamageBonus();
            return baseDamage * (1f + damageBonus);
        }
        
        return baseDamage;
    }

    protected virtual void Awake()
    {
        CacheBaseStats();
    }

    private void Start()
    {
        ApplySkillModifiers();
    }

    public void CacheBaseStats()
    {
        baseHP = HP;
        baseArmor = armor;
        baseSpeed = speed;
        baseStrength = strength;
        baseReach = reach;
        baseVision = vision;
    }

    public void ResetToBaseStats()
    {
        HP = baseHP;
        armor = baseArmor;
        speed = baseSpeed;
        strength = baseStrength;
        reach = baseReach;
        vision = baseVision;
    }

    public void ApplySkillModifiers()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.ApplyModifiersToAnt(this);
    }

    #endregion

    #region OUTLINE

    public void setOutline(Color color)
    {
        if (material == null)
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null)
            {
                material = renderer.material;

                if (material.HasProperty("_Outline_Color"))
                    defaultColor = material.GetColor("_Outline_Color");
            }
        }

        if (material != null && material.HasProperty("_Outline_Color"))
            material.SetColor("_Outline_Color", color);
    }

    public void setDefaultOutline()
    {
        if (material == null)
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null)
            {
                material = renderer.material;

                if (material.HasProperty("_Outline_Color"))
                    defaultColor = material.GetColor("_Outline_Color");
            }
        }

        if (material != null && material.HasProperty("_Outline_Color"))
            material.SetColor("_Outline_Color", defaultColor);
    }

    #endregion

    #region MOVEMENT

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