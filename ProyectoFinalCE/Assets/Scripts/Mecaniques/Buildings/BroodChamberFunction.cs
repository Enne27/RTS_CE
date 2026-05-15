using UnityEngine;
using static PlayerConstants;

public class BroodChamberFunction : StructuresPlayer
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Scriptable construction info")]
    [SerializeField] BuildingData broodBuildingScriptable;

    [Header("Ants")]
    [SerializeField] GameObject soldierAnt;
    [SerializeField] GameObject explorerAnt;
    [SerializeField] GameObject workerAnt;
    [SerializeField] GameObject berserkerAnt;
    [SerializeField] GameObject acidAnt;
    [SerializeField] GameObject crazyAnt;
    [SerializeField] GameObject kamikazeAnt;


    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 50, 50, 70, 90, 110, 170 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 5, 20, 30, 35, 55, 60 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 30, 60, 60, 70, 90, 120 };

    [Tooltip("Cantidad de hormigas que puede generar por nivel.")]
    [HideInInspector] public int[] broodingCapacity = { 1, 2, 3, 4, 5, 6 };

    [Tooltip("Tiempo que tarda en crear una hormiga base.")]
    int timeGeneratingAnt = 60;

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 3, 5, 6 };

    [Tooltip("Cantidad máxima que se puede construir por cada era.")]
    int[] maxQuantityByEra_ = { 1, 3, 4, 6 };


    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;
    public override int[] costsUpgradeMC => costsUpgradeMC_;
    public override int[] timeUpgrade => timeUpgrade_;
    public override int[] maxLevelByEra => maxLevelByEra_;

    public GameHUDView gameHUDView;
    [SerializeField] public BroodChamberView broodView;

    [Header("Limits")]
    /*[HideInInspector] */public int currentBreedingQuantity = 0;
    #endregion

    private void Awake()
    {
        gameHUDView = FindFirstObjectByType<GameHUDView>().GetComponent<GameHUDView>();

        if (broodView == null)
            broodView = FindFirstObjectByType<BroodChamberView>();
    }

    private void OnEnable()
    {
        currentBreedingQuantity = 0;
    }
    private void OnDestroy()
    {
        currentBreedingQuantity = 0;
    }

    #region BUILDING_METHODS
    public override void OnConstructionFinished()
    {
        GetComponentInChildren<Renderer>().material = BuildingManager.Instance.BroodChamberMaterial;
        currentStructureState = StructureState.Idle;
        workerWhoBuildThis.HasFinishedWork();
        workerWhoBuildThis = null;
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="antType"></param>
    /// <param name="position"></param>
    public void CreateAnt(ANT_TYPES antType, Transform position)
    {
        if (AntCreation.Instance == null || position == null)
            return;

        int limit = broodingCapacity[currentLevel - 1];

        if (currentBreedingQuantity >= limit)
        {
            return;
        }

        // Obtener costes ANTES de reservar slot
        AntCreation.Instance.ChangeAntTypeToInstantiate(antType);
        int foodCosts = 0;
        int hvCosts = 0;

        if (antType != ANT_TYPES.WORKER)
        {
            Ant antScript = AntCreation.Instance.antToInstantiate.GetComponent<Ant>();

            foodCosts = antScript.GetBreedingCost()[0];
            hvCosts = antScript.GetBreedingCost()[1];   
        }
        else
        {
            AntWorkerBehaviour antWorker = AntCreation.Instance.antToInstantiate.GetComponentInChildren<AntWorkerBehaviour>();
            foodCosts = antWorker.foodCost;
            hvCosts =  antWorker.hvCost;
        }

        if (!AntCreation.Instance.CanSpawnAnt(foodCosts, hvCosts))
        {
            Debug.Log("Insuficient hv or food");
            return;
        }


        currentBreedingQuantity++;

        if (broodView == null)
            broodView = FindFirstObjectByType<BroodChamberView>();

        PlayerAntCreation(antType, position, timeGeneratingAnt, foodCosts, hvCosts);
    }


    /// <summary>
    /// Creación de hormigas del jugador mediante el uso de recursos y actualización de la interfaz.
    /// </summary>
    /// <param name="antType">Tipo de hormiga a instanciar.</param>
    /// <param name="position">Transform de la posición donde instanciar.</param>
    public void PlayerAntCreation(ANT_TYPES antType, Transform position, float time, int foodCosts, int hvCosts)
    {
        if (position == null) return;

        AntCreation.Instance.ChangeAntTypeToInstantiate(antType);

        AntCreation.Instance.positionInstantiate = position;

        TimeManager.Instance?.OneShotTimer(time, () =>
        {
            AntCreation.Instance.SystemAntCreation(1, antType, position, true, true);
            gameHUDView?.UpdateAntText(antType, 1);

            currentBreedingQuantity--;

            ProgressManager.instance.RegisterAntCreation(antType);
        });

        VFXManager.Instance?.PlayBroodingChamberParticles(GetTransformToSpawnTimer(antType), time);


        GameManager.instance.player.inventory.RemoveFood(foodCosts);
        GameManager.instance.player.inventory.RemoveEggs(hvCosts);

        gameHUDView?.UpdateFoodText();
        gameHUDView?.UpdateEggsText();
    }

    private Vector3 GetTransformToSpawnTimer(ANT_TYPES antType)
    {
        Vector3 transform = new Vector3(0, 0, 0);
        switch (antType)
        {
            case ANT_TYPES.ACID:
                transform = broodView.acidButton.transform.position;
                break;

            case ANT_TYPES.BERSERKER:
                transform = broodView.berserkerButton.transform.position;
                break;

            case ANT_TYPES.EXPLORER:
                transform = broodView.explorerButton.transform.position;
                break;

            case ANT_TYPES.SOLDIER:
                transform = broodView.soldierButton.transform.position;
                break;

            case ANT_TYPES.CRAZY:
                transform = broodView.crazyButton.transform.position;
                break;

            case ANT_TYPES.KAMIKAZE:
                transform = broodView.kamikazeButton.transform.position;
                break;

            case ANT_TYPES.WORKER:
                transform = broodView.workerButton.transform.position;
                break;
        }

        return transform;
    }
}
