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

    public void CreateAnt(ANT_TYPES antType, Transform position)
    {
        if (AntCreation.Instance == null || position == null)
            return;

        int limit = broodingCapacity[currentLevel - 1];
            //currentBreedingQuantity--;

        Debug.Log(currentBreedingQuantity + "   " + limit);
        // LÍMITE REAL (antes de reservar)
        if (currentBreedingQuantity >= limit)
        {
            Debug.Log("Límite");
            return;
        }
        else
        {
            //currentBreedingQuantity--;
            currentBreedingQuantity++;

            AntCreation.Instance.PlayerAntCreation(antType, position, timeGeneratingAnt);
        }

        // RESERVA SLOT (IMPORTANTE: aquí es el único sitio)
    }

    /*private bool SpawnAnt(int foodCosts, int hvCosts)
    {
        return (GameManager.instance.player.inventory.food >= foodCosts) && (GameManager.instance.player.inventory.eggs >= hvCosts);
    }
    */
    public override void OnConstructionFinished()
    {
        GameManager.instance.player.inventory.RemoveEggs(broodBuildingScriptable.costHV);
        GameManager.instance.player.inventory.RemoveMC(broodBuildingScriptable.costMC);

        if (gameHUDView != null)
        {
            gameHUDView.UpdateMCText();
            gameHUDView.UpdateEggsText();
        }
    }

}
