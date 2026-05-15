using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using static PlayerConstants;

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
        RefreshUI();

        if (generalInfoView == null)
            generalInfoView = FindFirstObjectByType<GeneralInfoView>();

        if(advanceEraSound == null)
            advanceEraSound = new StudioEventEmitter();
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
                    /*new EraRequirement(RequirementID.QUEEN_CHAMBER, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.ANT, 2, RequirementType.COUNT),
                    new EraRequirement(RequirementID.STORAGE_CHAMBER, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.BROOD_CHAMBER, 1, RequirementType.COUNT
                    new EraRequirement(RequirementID.EXPLORATION, 3, RequirementType.COUNT),),*/
                    new EraRequirement(RequirementID.EXPLORATION, 1, RequirementType.COUNT),
                }
            },
            { 
                HIVE_ERAS.NIDO, new List<EraRequirement>()
                {
                    //new EraRequirement(RequirementID.QUEEN_CHAMBER, 1, RequirementType.LEVEL, 2),
                    new EraRequirement(RequirementID.WORKER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.EXPLORER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.BERSERKER_ANT, 1, RequirementType.COUNT),
                    new EraRequirement(RequirementID.SOLDIER_ANT, 1, RequirementType.COUNT),
                    //new EraRequirement(RequirementID.STORAGE_CHAMBER, 2, RequirementType.LEVEL, 2),
                    //new EraRequirement(RequirementID.BROOD_CHAMBER, 2, RequirementType.LEVEL, 2),
                    //new EraRequirement(RequirementID.EXPLORATION, 5, RequirementType.COUNT),
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

        foreach (var req in requirements)
        {
            if (req.id == id)
            {
                req.AddProgress(amount);
                changed = true;
            }
        }

        if (changed)
        {
            RefreshUI();
            CheckEraCompletion(era);
        }
    }

    private void CheckEraCompletion(HIVE_ERAS era)
    {
        foreach (var req in eraRequirements[era])
        {
            if (!req.IsCompleted)
                return;
        }

        AdvanceEra(true);
    }


    public void AdvanceEra(bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.player.currentEra += 1;
            ChangesNewEra();

            HIVE_ERAS newEra = GameManager.instance.player.currentEra;

            RefreshUI();

            foreach (var construction in BuildingManager.Instance.constructionsBuilt)
            {
                StructuresPlayer consFunction = construction.gameObject.GetComponentInChildren<StructuresPlayer>();
                consFunction.currentMaxLevel = consFunction.maxLevelByEra[(int)newEra];
                // la cantidad se actualiza en el placeBuilding según la era actual.

            }

            RefreshUI();

            advanceEraSound.EventReference = advanceEraSoundEventReference;
            SFXManager.PlaySFX(advanceEraSound);
        }
        else GameManager.instance.playerIA.currentEra += 1;
    }

    public void ChangesNewEra()
    {
        HIVE_ERAS era = GameManager.instance.player.currentEra;

        LocalizedString newEraName = eraNames[era];

        generalInfoView.UpdateCurrentEraVisuals(era, newEraName);
    }

    public void RefreshUI()
    {
        if (generalInfoView == null) return;

        if (!generalInfoView.gameObject.activeInHierarchy) return;

        var era = GameManager.instance.player.currentEra;

        generalInfoView.UpdateCurrentEraVisuals(
            era,
            eraNames[era]
        );

        generalInfoView.UpdateRequirements(
            eraRequirements[era]
        );
    }

}