using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using static PlayerConstants;
using static ConstantsAndKeys;

public class EraManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] Sprite broteSprite;
    [SerializeField] Sprite nidoSprite;
    [SerializeField] Sprite coloniaSprite;
    [SerializeField] Sprite imperioSprite;

    private Dictionary<HIVE_ERAS, List<EraRequirement>> eraRequirements;

    [HideInInspector] public Dictionary<HIVE_ERAS, Sprite> eraSprites;
    private Dictionary<HIVE_ERAS, LocalizedString> eraNames;

    [Header("UI")]
    private GeneralInfoView generalInfoView;

    [Header("Sound")]
    [SerializeField] private EventReference advanceEraSoundEventReference;
    private StudioEventEmitter advanceEraSound;

    [Header("Localization")]
    [SerializeField] LocalizedString brote_ls;
    [SerializeField] LocalizedString nido_ls;
    [SerializeField] LocalizedString colonia_ls;
    [SerializeField] LocalizedString imperio_ls;

    [Header("Era requirements control")]
    public int requirementsCurrentEra;
    #endregion

    #region SINGLETON
    public static EraManager instance { get; private set; }

    #endregion

    #region GETTERS
    public Sprite GetEraSprite(HIVE_ERAS era)
    {
        return eraSprites[era];
    }

    public LocalizedString GetEraName(HIVE_ERAS era)
    {
        return eraNames[era];
    }

    public List<EraRequirement> GetRequirements(HIVE_ERAS era)
    {
        return eraRequirements[era];
    }

    public LocalizedString GetLocalizedRequirement(RequirementID id)
    {
        // Esto lo sacas del slot o lo centralizas luego
        return null;
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitData();
        InitRequirements();

        /*if (generalInfoView == null)
            generalInfoView = FindFirstObjectByType<GeneralInfoView>();

        ForceRecalculateLevels();
        RefreshUI();*/

        if (advanceEraSound == null)
            advanceEraSound = new StudioEventEmitter();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene escena, LoadSceneMode arg1)
    {
        switch (escena.name)
        {
            case SINGLE_PLAYER_GAME_SCENE_NAME:
                if (generalInfoView == null)
                    generalInfoView = FindFirstObjectByType<GeneralInfoView>();

                ForceRecalculateLevels();
                RefreshUI();

                break;

            case CREATIVE_MODE_SCENE_NAME:
                break;
        }
    }

    public void InitData()
    {
        eraRequirements = new();

        eraSprites = new()
        {
            { HIVE_ERAS.BROTE,  broteSprite},
            { HIVE_ERAS.NIDO,  nidoSprite},
            { HIVE_ERAS.COLONIA, coloniaSprite },
            { HIVE_ERAS.IMPERIO,  imperioSprite}
        };

        eraNames = new()
        {
            { HIVE_ERAS.BROTE, brote_ls },
            { HIVE_ERAS.NIDO, nido_ls },
            { HIVE_ERAS.COLONIA, colonia_ls },
            { HIVE_ERAS.IMPERIO, imperio_ls }
        };

    }

    public void InitRequirements()
    {
        eraRequirements = new()
        {
            {
                HIVE_ERAS.BROTE, new List<EraRequirement>()
                {
                    new EraRequirement(RequirementID.QUEEN_CHAMBER, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.ANT, 2, RequirementType.COUNT),
                    new EraRequirement(RequirementID.STORAGE_CHAMBER, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.BROOD_CHAMBER, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.EXPLORATION, 3, RequirementType.COUNT),
                    //new EraRequirement(RequirementID.EXPLORATION, 1, RequirementType.COUNT),
                }
            },
            { 
                HIVE_ERAS.NIDO, new List<EraRequirement>()
                {
                    new EraRequirement(RequirementID.QUEEN_CHAMBER, 1, RequirementType.LEVEL, 2),
                    /*new EraRequirement(RequirementID.WORKER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.EXPLORER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.BERSERKER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.SOLDIER_ANT, 1, RequirementType.COUNT),*/
                    new EraRequirement(RequirementID.STORAGE_CHAMBER, 2, RequirementType.LEVEL, 2),
                    new EraRequirement(RequirementID.BROOD_CHAMBER, 2, RequirementType.LEVEL, 2),
                    new EraRequirement(RequirementID.EXPLORATION, 5, RequirementType.COUNT),
                }
            },
            {
                HIVE_ERAS.COLONIA, new List<EraRequirement>()
                {

                }
            },
            {
                HIVE_ERAS.IMPERIO, new List<EraRequirement>()
                {

                }
            }
        };
    }

    public void AddProgress(RequirementID id, int amount = 1)
    {
        var era = GameManager.instance.player.currentEra;

        if (!eraRequirements.ContainsKey(era))
            return;

        var requirements = GetRequirements(era);
        bool changed = false;

        bool exists = false;
        foreach (var req in requirements)
        {
            if (req.id == id)
            {
                exists = true;
                break;
            }
        }

        if (!exists) // No queremos progreso invalido
            return;

        foreach (var req in requirements)
        {
            if (req.id != id)
                continue;

            if (req.type == RequirementType.COUNT)
            {
                int before = req.currentQuantity;

                req.AddProgress(amount);

                if (req.currentQuantity != before)
                    changed = true;
            }
            else if (req.type == RequirementType.LEVEL)
            {
                int newValue = CountStructuresMeetingRequirement(req);

                if (newValue != req.currentQuantity)
                {
                    req.SetProgress(newValue);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            RefreshUI();
            CheckEraCompletion(era);
        }
    }

    public void ForceRecalculateLevels()
    {
        var era = GameManager.instance.player.currentEra;

        foreach (var req in eraRequirements[era])
        {
            if (req.type == RequirementType.LEVEL)
            {
                int value = CountStructuresMeetingRequirement(req);
                req.SetProgress(value);
            }
        }
    }



    private int CountStructuresMeetingRequirement(EraRequirement req)
    {
        int count = 0;

        foreach (var construction in BuildingManager.Instance.constructionsBuilt)
        {
            var structure = construction.GetComponentInChildren<StructuresPlayer>();
            if (structure == null) continue;

            // Filtrar por tipo
            if (!MatchesID(structure, req.id)) continue;

            if (structure.currentLevel >= req.requiredLevel)
                count++;
        }

        return count;
    }

    private bool MatchesID(StructuresPlayer structure, RequirementID id)
    {
        switch (id)
        {
            case RequirementID.QUEEN_CHAMBER:
                return structure is QueenChamberFunction;

            case RequirementID.BROOD_CHAMBER:
                return structure is BroodChamberFunction;

            case RequirementID.STORAGE_CHAMBER:
                return structure is StorageChamberFunction;
        }

        return false;
    }

    private void CheckEraCompletion(HIVE_ERAS era)
    {
        foreach (var req in eraRequirements[era])
        {
            if (!req.IsCompleted)
                return;
        }

        if (era != GameManager.instance.player.currentEra)
            return;

        AdvanceEra(true);
    }


    public void AdvanceEra(bool isPlayer)
    {
        if (isPlayer)
        {
            if (GameManager.instance.player.currentEra >= HIVE_ERAS.IMPERIO)
            {
                Debug.LogWarning("No more eras.");
                return;
            }

            GameManager.instance.player.currentEra += 1;
            ChangesNewEra();

            UpgradeLimitsAndRefreshUpgradeButtonUI();

            ForceRecalculateLevels();

            RefreshUI();

            advanceEraSound.EventReference = advanceEraSoundEventReference;

            if(SFXManager.instance != null)
                SFXManager.PlaySFX(advanceEraSound);
        }
        else GameManager.instance.playerIA.currentEra += 1;
    }

    public void ChangesNewEra()
    {
        HIVE_ERAS era = GameManager.instance.player.currentEra;

        LocalizedString newEraName = eraNames[era];

        if (generalInfoView == null)
            generalInfoView = FindFirstObjectByType<GeneralInfoView>();

        generalInfoView.UpdateCurrentEraVisuals(era, newEraName);
}

    public void RefreshUI()
    {
        Debug.Log($"ERA: {GameManager.instance.player.currentEra}");
        Debug.Log($"generalInfoView: {generalInfoView}");
        Debug.Log($"eraNames null? {eraNames == null}");
        Debug.Log($"eraRequirements null? {eraRequirements == null}");

        var era = GameManager.instance.player.currentEra;

        if (!eraNames.ContainsKey(era) || !eraRequirements.ContainsKey(era))
            return;

        if (generalInfoView == null) 
            generalInfoView = FindFirstObjectByType<GeneralInfoView>();

        if (generalInfoView == null)
            return;

        if (!generalInfoView.gameObject.activeInHierarchy)
            return;

       generalInfoView.RefresUI(era);
    }

    public void UpgradeLimitsAndRefreshUpgradeButtonUI()
    {
        HIVE_ERAS newEra = GameManager.instance.player.currentEra;

        foreach (var construction in BuildingManager.Instance.constructionsBuilt)
        {
            StructuresPlayer consFunction = construction.gameObject.GetComponentInChildren<StructuresPlayer>();
            consFunction.currentMaxLevel = consFunction.maxLevelByEra[(int)newEra];
            consFunction.currentTimeUpgrade = consFunction.timeUpgrade[(int)newEra];
            consFunction.currentCostsUpgradeHV = consFunction.costsUpgradeHV[(int)newEra];
            consFunction.currentCostsUpgradeMC = consFunction.costsUpgradeMC[(int)newEra];
            // la cantidad se actualiza en el placeBuilding según la era actual.
            consFunction.RefreshUpgradeUI();
        }
    }

}