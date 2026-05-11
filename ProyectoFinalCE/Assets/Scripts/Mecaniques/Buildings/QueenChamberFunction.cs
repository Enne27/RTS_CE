using UnityEngine;
using UnityEngine.UI;


public class QueenChamberFunction : StructuresPlayer
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Tiempo que tarda en producir huevas (en segundos).")]
    [SerializeField] float timeToProduceEggs = 60f;

    [Tooltip("Scriptable construction info")]
    [SerializeField] BuildingData queenBuildingScriptable;


    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 0, 25, 35, 50, 80 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 5, 15, 25, 30, 45 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 10, 75, 90, 90, 120 };

    [Tooltip("Cantidad de huevas que produce por cada burst de producción.")]
    int[] quantityProduction = { 20, 40, 60, 80, 100 };

    // El límite de huevas está en playerConstants

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 2, 4, 5};

    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;

    public override int[] costsUpgradeMC => costsUpgradeMC_;

    public override int[] timeUpgrade => timeUpgrade_;

    public override int[] maxLevelByEra => maxLevelByEra_;

    [Header("Visual player")]
    GameHUDView hudView;

    #endregion


    #region METHODS_STRUCTURES
    private void Awake()
    {
        hudView = FindFirstObjectByType<GameHUDView>();
    }


    public override void OnConstructionFinished()
    {
        TimeManager.Instance.Register(timeToProduceEggs, ProduceEggs);

        GameManager.instance.player.inventory.RemoveEggs(queenBuildingScriptable.costHV);
        GameManager.instance.player.inventory.RemoveMC(queenBuildingScriptable.costMC);

        if (hudView != null)
        {
            hudView.UpdateMCText();
            hudView.UpdateEggsText();
        }
    }

    #endregion

    /*private void Start()  // OnEnable realmente, pero a veces decide ejecutar en otro orden
    {
        TimeManager.Instance.Register(timeToProduceEggs, ProduceEggs);
    }*/

    private void OnDisable()
    {
        TimeManager.Instance.Unregister(timeToProduceEggs, ProduceEggs);
    }

    /// <summary>
    /// FALTARÍA FEEDBACK, PERO SE GENERAN CADA 60 SEGUNDOS
    /// </summary>
    public void ProduceEggs()
    {
       int currentEggs = GameManager.instance.player.inventory.eggs;
       int currentEggCapacity = GameManager.instance.player.inventory.eggCapacity;
       int eggsToAdd = quantityProduction[currentLevel-1];

        if (currentEggs + eggsToAdd < currentEggCapacity)
            GameManager.instance.player.inventory.AddEggs(eggsToAdd); 
            
       else {
            GameManager.instance.player.inventory.eggs = currentEggCapacity; 
       }

       if(hudView != null) hudView.UpdateEggsText();
    }

}
