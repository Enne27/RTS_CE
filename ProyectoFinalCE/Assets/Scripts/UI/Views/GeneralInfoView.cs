using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static PlayerConstants;

public class GeneralInfoView : View
{
    [Header("Era info")]
    [SerializeField] TextMeshProUGUI currentEraText;
    [SerializeField] Image currentEraImage;
    [SerializeField] Image currentEraGameHUDButton;

    [Header("Requirements")]
    [SerializeField] RequirementNextEraSlot slotPrefab;
    [SerializeField] Transform layoutHorizontal;

    [Header("Buttons")]
    [SerializeField] Button backButton;

    private List<RequirementNextEraSlot> activeSlots = new();

    public override void Initialize()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private IEnumerator Start()
    {
        yield return null; // 🔴 IMPORTANTE: esperar a EraManager

        RefreshUI(GameManager.instance.player.currentEra);
    }

    public void RefreshUI(HIVE_ERAS era)
    {
        if (EraManager.instance == null) return;

        UpdateCurrentEraVisuals(
            era,
            EraManager.instance.GetEraName(era)
        );

        UpdateRequirements(
            EraManager.instance.GetRequirements(era)
        );
    }

    public void UpdateCurrentEraVisuals(HIVE_ERAS currentEra, LocalizedString newEraName)
    {
        currentEraImage.sprite = EraManager.instance.GetEraSprite(currentEra);
        currentEraGameHUDButton.sprite = currentEraImage.sprite;

        var localize = currentEraText.GetComponent<LocalizeStringEvent>();
        localize.StringReference = newEraName;

        newEraName.StringChanged -= OnEraChanged;
        newEraName.StringChanged += OnEraChanged;
    }

    private void OnEraChanged(string value)
    {
        currentEraText.text = value;
    }

    public void UpdateRequirements(List<EraRequirement> requirements)
    {
        if (requirements == null)
        {
            Debug.LogError("Requirements NULL -> abort");
            return;
        }

        // 🔴 LIMPIAR SIEMPRE
        foreach (var s in activeSlots)
            if (s != null) Destroy(s.gameObject);

        activeSlots.Clear();

        foreach (var req in requirements)
        {
            if (req == null) continue;

            var slot = Instantiate(slotPrefab, layoutHorizontal);
            slot.Bind(req);
            activeSlots.Add(slot);
        }

        // 🔴 FORZAR REBUILD UI
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutHorizontal as RectTransform);
    }
}
