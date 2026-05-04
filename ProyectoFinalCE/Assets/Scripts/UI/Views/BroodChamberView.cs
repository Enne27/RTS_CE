using UnityEngine;
using UnityEngine.UI;
using static PlayerConstants;

public class BroodChamberView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button soldierButton;
    [SerializeField] Button berserkerButton;
    [SerializeField] Button workerButton;
    [SerializeField] Button explorerButton;
    [SerializeField] Button acidButton;
    [SerializeField] Button crazyutton;
    [SerializeField] Button kamikazeButton;

    [Header("Functionality")]
    [SerializeField] BroodChamberFunction broodChamberFunction;

    [Header("Transforms")]
    Transform antsSpawnPoint;
    Transform workersSpawnPoint;
    #endregion

    public override void Initialize()
    {
        if (AntCreation.Instance != null)
        {
            antsSpawnPoint = AntCreation.Instance.antsSpawnPoint;
            workersSpawnPoint = AntCreation.Instance.workersSpawnPoint;
        }

        if (soldierButton != null)
            soldierButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.SOLDIER, antsSpawnPoint));

        if (berserkerButton != null)
            berserkerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.BERSERKER, antsSpawnPoint));

        if (workerButton != null)
            workerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.WORKER, workersSpawnPoint));

        if (explorerButton != null)
            explorerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.EXPLORER, antsSpawnPoint));

        if (acidButton != null)
            acidButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.ACID, antsSpawnPoint));

        if (crazyutton != null)
            crazyutton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.CRAZY, antsSpawnPoint));

        if (kamikazeButton != null)
            kamikazeButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.KAMIKAZE, antsSpawnPoint));

    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        soldierButton.onClick.RemoveAllListeners();
        berserkerButton.onClick.RemoveAllListeners();
        workerButton.onClick.RemoveAllListeners();
        explorerButton.onClick.RemoveAllListeners();
        acidButton.onClick.RemoveAllListeners();
        crazyutton.onClick.RemoveAllListeners();
        kamikazeButton.onClick.RemoveAllListeners();
    }
}
