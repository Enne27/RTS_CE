using System;
using UnityEngine;
using static PlayerConstants;

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
        //if (antOwner == Owner.Player)
        //{
        //    gameObject.tag = "PlayerAnt";
        //}
        //else if (antOwner == Owner.AI) 
        //{
        //    gameObject.tag = "EnemyAnt";
        //}
        antType = ANT_TYPES.EXPLORER;
        HP = 15f;
        armor = 0.40f;
        speed = 16f;
        strength = 1f;
        reach = 1;
        vision = 15;
        linePriority = 8;
        acidBased = false;
        anthillContact = false;
        base.Awake();
    }

    public override void IsRange(Ant target) 
    {
        Debug.Log("Comprobando el rango");
        float distance = Vector3.Distance(transform.position, target.transform.position);
        bool inRange = false;
        while (!inRange)
        {
            if (distance <= reach)
            {
                Debug.Log("Estoy en el rango");
                Attack(target);
                inRange = true;
            }
        }
    }
    public override void Attack(Ant target)
    {
        target.TakeDamage(this, GetEffectiveDamage(), acidBased);
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
            Debug.Log("Ouch! me quedan " + GetCurrentHP() + " puntos de vida");
        }
        if(HP <= 0)
        {
            HP = 0;
            Die();
        }
        OnAnyAntDamaged?.Invoke(this);
       
    }

    public void Collect()
    {
        TimeManager.Instance.OneShotTimer(3f, () => 
        {
            GameObject position = null;
            food = UnityEngine.Random.Range(1, 3);
            MC = UnityEngine.Random.Range(1, 2);
            
            if (antOwner == Owner.Player)
            {
                gameObject.tag = "PlayerAnt";
            }
            else if (antOwner == Owner.AI)
            {
                gameObject.tag = "EnemyAnt";
            }
            switch (antOwner)
            {
                case Owner.Player:
                    if (GameManager.instance.player.structures != null && 
                        GameManager.instance.player.structures.Count > 0)
                    {
                        position = GameManager.instance.player.structures[0];
                    }
                    else
                    {
                        Debug.LogError("AntExplorer.Collect: No hay estructura del jugador a la que regresar.");
                        return; // Cancelar el movimiento
                    }
                    break;
                    
                case Owner.AI:
                    if (GameManager.instance.playerIA.structures != null && 
                        GameManager.instance.playerIA.structures.Count > 0 &&
                        GameManager.instance.playerIA.structures[0] != null)
                    {
                        position = GameManager.instance.playerIA.structures[0];
                    }
                    else
                    {
                        Debug.LogError("AntExplorer.Collect: No hay estructura de la IA a la que regresar.");
                        return; // Cancelar el movimiento
                    }
                    break;
            }
            
            UnitController.MoveTo(this, position);
        });
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
        Destroy(gameObject);
    }

    public int GetFood()
    {
        return food;
    }

    public void SetFood(int value)
    {
        food = value;
    }

    public int GetMC()
    {
        return MC;
    }

    public void SetMC(int value)
    {
        MC = value;
    }
}
