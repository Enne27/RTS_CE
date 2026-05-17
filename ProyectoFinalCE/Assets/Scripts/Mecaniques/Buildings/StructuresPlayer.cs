using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum StructureState
{
    OnConstruction,
    OnUpdate,
    Idle,
}

public abstract class StructuresPlayer : MonoBehaviour
{
    #region VARIABLES

    [Header("Upgrade Visuals")]
    [SerializeField] public Image upgradeButtonBackground;
    [SerializeField] public Button upgradeButton;
    [SerializeField] public Canvas canvas;
    [SerializeField] public TextMeshProUGUI lvl_TMP;

   // public BuildingData buildingData;

    [Header("Upgrade logic")]
    public int currentCostsUpgradeHV;
    public int currentCostsUpgradeMC;
    public int currentTimeUpgrade = 60;
    public int currentMaxLevel;

    [HideInInspector] public Vector2 positionAntHill;
    public abstract int[] costsUpgradeHV { get; }
    public abstract int[] costsUpgradeMC { get; }
    public abstract int[] timeUpgrade { get; }
    public abstract int[] maxLevelByEra { get; }


    [Header("Current Parameters")]
    public StructureState currentStructureState = StructureState.OnConstruction;
    public int currentLevel = 1;

    [Header("Worker construction")]
    public AntWorkerBehaviour workerWhoBuildThis;
    #endregion


    /// <summary>
    /// Mientras se est� actualizando la construcci�n.
    /// </summary>
    public void UpgradeStructure()
    {
        if (currentStructureState != StructureState.Idle)
            return;

        if (!HasEnoughResources())
           return;

        currentStructureState = StructureState.OnUpdate;
        RefreshUpgradeUI();

        VFXManager.Instance.PlayConstructionParticles(gameObject.transform.position, currentTimeUpgrade);

        TimeManager.Instance.OneShotTimer(currentTimeUpgrade, () =>
        {
            OnUpgradeFinished();
        });
    }

    /// <summary>
    /// Cuando termina de construirse al inicio.
    /// </summary>
    public virtual void OnConstructionFinished()
    {
        currentStructureState = StructureState.Idle;
        RefreshUpgradeUI();
    }

    /// <summary>
    /// Cuando termina de actualizarse la construcci�n.
    /// </summary>
    public virtual void OnUpgradeFinished() 
    { 
        currentStructureState = StructureState.Idle;
        currentLevel++;
        currentCostsUpgradeHV = costsUpgradeHV[currentLevel];
        currentCostsUpgradeMC = costsUpgradeMC[currentLevel];
        currentTimeUpgrade = timeUpgrade[currentLevel];

        RefreshUpgradeUI();
    }

    #region UPGRADE_VALIDATIONS

    public bool IsMaxLevelForEra()
    {
        int currentEra = (int)GameManager.instance.player.currentEra;

        int maxLevel = maxLevelByEra[currentEra];

        return currentLevel >= maxLevel;
    }

    public bool HasEnoughResources()
    {
        var inventory = GameManager.instance.player.inventory;

        return inventory.eggs >= currentCostsUpgradeHV &&
               (inventory.materials >= currentCostsUpgradeMC || inventory.materialsInForaging >= currentCostsUpgradeMC);
    }

    public bool CanUpgrade()
    {
        if (currentStructureState != StructureState.Idle)
            return false;

        if (IsMaxLevelForEra())
            return false;

        return true;
    }
    #endregion

    public void RefreshUpgradeUI()
    {
        if (upgradeButton == null || upgradeButtonBackground == null || lvl_TMP == null)
        {
            Debug.LogWarning($"RefreshUpgradeUI: alguno de los elementos UI es null en {gameObject.name}");
            return;
        }

        bool canUpgrade = CanUpgrade();
        upgradeButtonBackground.gameObject.SetActive(true);
        upgradeButton.interactable = canUpgrade;

        Color imgColor;
        if (IsMaxLevelForEra())
            imgColor = Color.black;
        else if (HasEnoughResources())
            imgColor = Color.green;
        else
            imgColor = Color.red;

        upgradeButton.image.color = imgColor;
        lvl_TMP.text = $"LV.{currentLevel}";
    }
}