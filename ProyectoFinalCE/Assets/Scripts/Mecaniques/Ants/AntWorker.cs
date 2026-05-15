using UnityEngine;
using static PlayerConstants;

public class AntWorker : Ant
{
    private void Awake()
    {
        antType = ANT_TYPES.WORKER;
        HP = 12f;
        armor = 0f;
        speed = 8f;
        strength = 0f;
        reach = 1;
        vision = 2;
        linePriority = 1;
        acidBased = false;
        breedingCost = new int[] { 1, 1 };  

        CacheBaseStats();
    }

    public override void Attack(Ant target)
    {
        // Las obreras no atacan
    }

    public override void TakeDamage(Ant other, float strength, bool acidBased)
    {
        // Las obreras no reciben daño (están bajo tierra)
    }

    public override void AttackMound(GameObject mound)
    {
        // No atacan montículos
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}