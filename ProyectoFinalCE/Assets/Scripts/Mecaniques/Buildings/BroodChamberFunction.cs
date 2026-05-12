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
    int[] broodingCapacity = { 1, 2, 3, 4, 5, 6 };

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

    public void CreateAnt(ANT_TYPES antType, Transform position)
    {
        if(AntCreation.Instance != null && currentBreedingQuantity < broodingCapacity[currentLevel])
            AntCreation.Instance.PlayerAntCreation(antType, position, timeGeneratingAnt);
       /* //Debug.Log(antType);
        GameObject antInstantiate = workerAnt;

        if (position != null)
        {
            switch (antType)
            {
                case ANT_TYPES.ACID:
                    antInstantiate = acidAnt;
                    break;
                case ANT_TYPES.BERSERKER:
                    antInstantiate = berserkerAnt;
                    break;
                case ANT_TYPES.EXPLORER:
                    antInstantiate = explorerAnt;
                    break;
                case ANT_TYPES.SOLDIER:
                    antInstantiate = soldierAnt;
                    break;
                case ANT_TYPES.CRAZY: 
                    antInstantiate = crazyAnt;
                    break;
                case ANT_TYPES.KAMIKAZE:
                    antInstantiate = kamikazeAnt;
                    break;
                case ANT_TYPES.WORKER:
                    antInstantiate = workerAnt;
                    break;
            }

            Ant antScript = antInstantiate.GetComponent<Ant>();
            int foodCosts = 0;
            int hvCosts = 0;
            if (antScript != null)
            {
                foodCosts = antScript.GetBreedingCost()[0];
                hvCosts = antScript.GetBreedingCost()[1];
            }

            
            // FALTARÍA AÑADIR LO DE QUE SI HAY UNA HORMIGA DE ESE TIPO DESACTIVADA, USARLA, NO CREAR.
            // FALTARÍA AÑADIR EL TIEMPO DE CONSTRUCCIÓN DE ESA HORMIGA, SIMPLEMENTE USAR EL REGISTER DEL TIME MANAGER Y LUEGO UNREGISTER, PERO CUANDO SE TENGA FEEDBACK
            if(SpawnAnt(foodCosts, hvCosts))
            {
                GameObject newAnt = Instantiate(antInstantiate, position.position, Quaternion.identity);
                if(antType != ANT_TYPES.WORKER)
                    GameManager.instance.player.ants.Add(newAnt.GetComponent<Ant>());
                else
                    GameManager.instance.player.inventory.workerAnts++;
                if (antType == ANT_TYPES.EXPLORER)
                    //newAnt.GetComponent<AntExlporer>().antHillPositionOwner = GameManager.instance.player.structures[0].transform.position;
                    newAnt.GetComponent<AntExlporer>().antOwner = Owner.Player;

                // Actualización HUD
                if (gameHUDView == null)
                    gameHUDView = FindFirstObjectByType<GameHUDView>();

                gameHUDView.UpdateAntText(antType, 1);

                GameManager.instance.player.inventory.RemoveFood(foodCosts);
                gameHUDView.UpdateFoodText();
                GameManager.instance.player.inventory.RemoveEggs(hvCosts);
                gameHUDView.UpdateEggsText();
            }
            else
            {
                Debug.Log("Insuficient hv or food");
            }
        }*/
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
