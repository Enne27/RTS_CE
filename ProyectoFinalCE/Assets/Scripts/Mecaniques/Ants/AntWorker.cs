using UnityEngine;

public class AntWorker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*
public class AntWorker : Ant
{
    public static event System.Action<Ant> OnAnyAntDamaged;

    private void Awake()
    {
        antType = ANT_TYPES.WORKER;
        HP = 12f;
        armor = 0.25f;
        speed = 8f;
        strength = 1f;
        reach = 1;
        vision = 2;
        linePriority = 1;
        acidBased = false;
    }

    public override void Attack(Ant target)
    {
        // Workers don't attack
    }

    public override void TakeDamage(Ant other, float strength, bool acidBased)
    {
        float damageTaken;
        if (other.GetAcidBased() == true)
        {
            damageTaken = other.GetStrength() - (0.5f * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        else if (other.GetAcidBased() == false)
        {
            damageTaken = other.GetStrength() - (armor * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
*/