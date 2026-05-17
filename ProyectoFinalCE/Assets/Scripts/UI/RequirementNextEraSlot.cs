using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class RequirementNextEraSlot : MonoBehaviour
{
    #region VARIABLES
    private Dictionary<RequirementID, Sprite> sprites;
    private Dictionary<RequirementID, LocalizedString> localizedNames;


    [Header("Components prefab")]
    [SerializeField] TextMeshProUGUI quantity_TMP;
    [SerializeField] TextMeshProUGUI name_TMP;
    [SerializeField] Image imageObject;


    [Header("Exploration")]
    [SerializeField] Sprite exploration;
    [SerializeField] LocalizedString exploration_ls;

    [Header("Ants")]
    [SerializeField] Sprite ant;
    [SerializeField] Sprite acidAnt;
    [SerializeField] Sprite berserkerAnt;
    [SerializeField] Sprite explorerAnt;
    [SerializeField] Sprite kamikazeAnt;
    [SerializeField] Sprite crazyAnt;
    [SerializeField] Sprite soldierAnt;
    [SerializeField] Sprite workerAnt;

    [Header("Buildings")]
    [SerializeField] Sprite broodChamber;
    [SerializeField] Sprite queenChamber;
    [SerializeField] Sprite storageChamber;


    [Header("Localization Ants")]
    [SerializeField] LocalizedString ant_ls;
    [SerializeField] LocalizedString acidAnt_ls;
    [SerializeField] LocalizedString berserkerAnt_ls;
    [SerializeField] LocalizedString explorerAnt_ls;
    [SerializeField] LocalizedString kamikazeAnt_ls;
    [SerializeField] LocalizedString crazyAnt_ls;
    [SerializeField] LocalizedString soldierAnt_ls;
    [SerializeField] LocalizedString workerAnt_ls;


    [Header("Localization Buildings")]
    [SerializeField] LocalizedString broodChamber_ls;
    [SerializeField] LocalizedString queenChamber_ls;
    [SerializeField] LocalizedString storageChamber_ls;


    [Header("TextColors")]
    [SerializeField] Color completedColor; // #1DB469
    [SerializeField] Color uncompletedColor; // #D64224


    private EraRequirement requirement;
    private LocalizedString currentLocalized;
    #endregion

    private void Awake()
    {
        sprites = new()
        {
            { RequirementID.ANT, ant},
            { RequirementID.ACID_ANT, acidAnt },
            { RequirementID.BERSERKER_ANT, berserkerAnt },
            { RequirementID.EXPLORER_ANT, explorerAnt },
            { RequirementID.CRAZY_ANT, crazyAnt },
            { RequirementID.KAMIKAZE_ANT, kamikazeAnt },
            { RequirementID.SOLDIER_ANT, soldierAnt },
            { RequirementID.WORKER_ANT, workerAnt },

            { RequirementID.BROOD_CHAMBER, broodChamber },
            { RequirementID.QUEEN_CHAMBER, queenChamber },
            { RequirementID.STORAGE_CHAMBER, storageChamber },

            { RequirementID.EXPLORATION, exploration }
        };

        localizedNames = new()
        {
            { RequirementID.ANT, ant_ls},
            { RequirementID.ACID_ANT, acidAnt_ls },
            { RequirementID.BERSERKER_ANT, berserkerAnt_ls },
            { RequirementID.EXPLORER_ANT, explorerAnt_ls },
            { RequirementID.KAMIKAZE_ANT, kamikazeAnt_ls },
            { RequirementID.CRAZY_ANT, crazyAnt_ls },
            { RequirementID.SOLDIER_ANT, soldierAnt_ls },
            { RequirementID.WORKER_ANT, workerAnt_ls },

            { RequirementID.BROOD_CHAMBER, broodChamber_ls },
            { RequirementID.QUEEN_CHAMBER, queenChamber_ls },
            { RequirementID.STORAGE_CHAMBER, storageChamber_ls },

            { RequirementID.EXPLORATION, exploration_ls }
        };
    }

    private void OnDestroy()
    {
        if (requirement != null)
            requirement.OnChanged -= RefreshQuantityText;

        if (currentLocalized != null)
            currentLocalized.StringChanged -= UpdateName;
    }

    public void Bind(EraRequirement req)
    {
        if (req == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (sprites == null) Awake();

        if (!sprites.ContainsKey(req.id) || !localizedNames.ContainsKey(req.id))
        {
            Debug.LogError($"Falta config para {req.id}");
            gameObject.SetActive(false);
            return;
        }

        // 🔴 limpiar anterior
        if (requirement != null)
            requirement.OnChanged -= RefreshQuantityText;

        if (currentLocalized != null)
            currentLocalized.StringChanged -= UpdateName;

        requirement = req;

        imageObject.sprite = sprites[req.id];

        currentLocalized = localizedNames[req.id];
        currentLocalized.StringChanged += UpdateName;

        UpdateName(currentLocalized.GetLocalizedString());

        requirement.OnChanged += RefreshQuantityText;

        RefreshQuantityText();
    }

    private void UpdateName(string baseName)
    {
        if (requirement == null) return;

        if (requirement.type == RequirementType.LEVEL)
            name_TMP.text = $"{baseName} Lv. {requirement.requiredLevel}";
        else
            name_TMP.text = baseName;
    }

    private void RefreshQuantityText()
    {
        if (requirement == null) return;

        Color color = requirement.IsCompleted ? completedColor : uncompletedColor;
        string hex = ColorUtility.ToHtmlStringRGB(color);

        quantity_TMP.text =
            $"<color=#{hex}>{requirement.currentQuantity}/{requirement.targetQuantity}</color>";
    }
}
