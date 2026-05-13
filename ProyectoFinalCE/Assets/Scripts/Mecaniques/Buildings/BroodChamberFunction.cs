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

    [Header("Limits")]
    [HideInInspector] public int currentBreedingQuantity = 0;
    #endregion

    private void Awake()
    {
        gameHUDView = FindFirstObjectByType<GameHUDView>().GetComponent<GameHUDView>();

    }
    private void OnDestroy()
    {
        currentBreedingQuantity = 0;
    }

    #region BUILDING_METHODS
    public override void OnConstructionFinished()
    {
        GameManager.instance.player.inventory.RemoveEggs(broodBuildingScriptable.costHV);
        GameManager.instance.player.inventory.RemoveMC(broodBuildingScriptable.costMC);

        if (gameHUDView != null)
        {
            gameHUDView.UpdateMCText();
            gameHUDView.UpdateEggsText();
        }

        currentStructureState = StructureState.Idle;
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
            Debug.Log("Límite");
            return;
        }

        // Obtener costes ANTES de reservar slot
        AntCreation.Instance.ChangeAntTypeToInstantiate(antType);

        Ant antScript = AntCreation.Instance.antToInstantiate.GetComponent<Ant>();

        int foodCosts = antScript.GetBreedingCost()[0];
        int hvCosts = antScript.GetBreedingCost()[1];

        if (!AntCreation.Instance.CanSpawnAnt(foodCosts, hvCosts))
        {
            Debug.Log("Insuficient hv or food");
            return;
        }

        currentBreedingQuantity++;

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
            currentBreedingQuantity--;
        });

        VFXManager.Instance?.PlayBroodingChamberParticles(transform.position, time);

        gameHUDView?.UpdateAntText(antType, 1);

        GameManager.instance.player.inventory.RemoveFood(foodCosts);
        GameManager.instance.player.inventory.RemoveEggs(hvCosts);

        gameHUDView?.UpdateFoodText();
        gameHUDView?.UpdateEggsText();

    }
}
