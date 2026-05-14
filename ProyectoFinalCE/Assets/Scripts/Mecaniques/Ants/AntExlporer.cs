using System;
using UnityEngine;


public enum Owner
{
    Player,
    AI
}
public class AntExlporer : Ant
{
    public int food;
    public int MC;
    //int[] breedingCost = { 7, 12 };
    public static event Action<Ant> OnAnyAntDamaged;
    public Transform targetTransform;   
    private Vector3 targetPosition;
    [Obsolete("Use antOwner instead")]
    public Vector3 antHillPositionOwner;
    private bool useTransformTarget;

    public GameObject asignedResourceZone;
    private void Awake()
    {
        //HP = 15f;
        //armor = 0.40f;
        //speed = 16f;
        //strength = 1f;
        //reach = 1;
        //vision = 4;
        //linePriority = 8;
        //acidBased = false;

        //if (antOwner == Owner.Player)
        //{
        //    gameObject.tag = "PlayerAnt";
        //}
        //else if (antOwner == Owner.AI) 
        //{
        //    gameObject.tag = "EnemyAnt";
        //}
        anthillContact = false;
    }

    public override void IsRange(Ant target) 
    {
        Debug.Log("Comprobando el rango");
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= reach)
        {
            Debug.Log("Estoy en el rango");
            Attack(target);
        }
    }
    public override void Attack(Ant target)
    {
        target.TakeDamage(this, strength, acidBased);
    }
    public override void TakeDamage(Ant other, float strenght, bool acidBased)
    {
        float damageTaken;
        float acidArmor = 0.6f;
        if (other.GetAcidBased() == true)
        {
            damageTaken = other.GetStrength() - (acidArmor * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        else if (other.GetAcidBased() == false)
        {
            damageTaken = other.GetStrength() - (armor * other.GetStrength());
            damageTaken = Mathf.Max(0, damageTaken);
            HP -= damageTaken;
        }
        if(HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
        Debug.Log("Ouch! me quedan " + GetCurrentHP() + " puntos de vida");
    }

    public void Collect()
    {
        TimeManager.Instance.OneShotTimer(3f, () => 
        {
            GameObject position = null;
            food = UnityEngine.Random.Range(1, 3);
            MC = UnityEngine.Random.Range(1, 2);
            switch (antOwner)
            {
                case (Owner.Player):
                    position = GameManager.instance.player.structures[0];
                    break;
                case (Owner.AI):
                    if(GameManager.instance.playerIA.structures[0] != null)
                        position = GameManager.instance.playerIA.structures[0];
                    break;
            }
            if( position != null )
                UnitController.MoveTo(this, position);
        }); 
        if (antOwner == Owner.Player)
        {
            gameObject.tag = "PlayerAnt";
        }
        else if (antOwner == Owner.AI)
        {
            gameObject.tag = "EnemyAnt";
        }
    }
    public void Deposit()
    {
        Inventory inventory = null;
        ForagingChamberFunction foragingChamber = ForagingChamberFunction.Instance;
        switch (antOwner)
        {
            case (Owner.Player):
                inventory = GameManager.instance.player.inventory;
                if (foragingChamber.AddResource(ResourceType.food, food))
                    inventory.AddFoodInForaging(food);
                else
                    inventory.foodInForaging = foragingChamber.foods;
                if (foragingChamber.AddResource(ResourceType.material, MC))
                    inventory.AddMCInForaging(MC);
                else
                    inventory.materialsInForaging = foragingChamber.materials;
                break;
            case (Owner.AI):
                inventory = GameManager.instance.playerIA.inventory;
                inventory.AddFood(food);
                inventory.AddMC(MC);
                break;
        }
        food = 0;
        MC = 0;
        if (asignedResourceZone != null)
        UnitController.MoveTo(this, asignedResourceZone);
    }

    public override void AttackMound(GameObject mound)
    {
        MoundFunction target;
        //La trucada del AttackMound no pasa el if
        if (antOwner == Owner.Player && mound.CompareTag("AI_AntHill") ||antOwner == Owner.AI && gameObject.CompareTag("Player_AntHill"))
        {
            target = mound.GetComponent<MoundFunction>();
            target.TakeDamage((int)Math.Round(strength), antOwner);
            CheckMoundTrigger(mound);
        }

        else
        {   
            return;
        }
    }
    public void CheckMoundTrigger(GameObject mound)
    {
        if (anthillContact == true)
        {
            TimeManager.Instance.OneShotTimer(3f,()=> AttackMound(mound));
        }
        else
            return;
    }
    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
