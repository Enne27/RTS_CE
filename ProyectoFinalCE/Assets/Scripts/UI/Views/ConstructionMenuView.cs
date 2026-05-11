using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ConstructionMenuView : View
{
    [System.Serializable]
    public class ConstructionButton
    {
        public Button buttonComponent;
        public BuildingData buildingData;
        public Vector3 originalScale;
    }

    #region VARIABLES
    [Header("Info view")]
    [SerializeField] private List<ConstructionButton> constructionsButtons;
    [SerializeField] GameObject buildingInfo;
    [SerializeField] TextMeshProUGUI buildingName;
    [SerializeField] TextMeshProUGUI buildingDescText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image buildingPreviewImage;

    [Header("LocalizedStrings")]
    [SerializeField] private LocalizeStringEvent chamberDescription;
    [SerializeField] private LocalizeStringEvent chamberName;

    [Header("ConstructionButtons")]
    [SerializeField] Button queenChamberButton;
    [SerializeField] Button broodChamberButton;
    [SerializeField] Button storageChamberButton;
    [SerializeField] Button tunnelButton;

    [Header("LeanTween Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeOutBack;

    [Header("Sound")]
    [Tooltip("Event Emitter del sonido para onHover un botón.")] 
    [SerializeField] StudioEventEmitter onHoverEmitter;

    private BuildingManager buildingMa;
    #endregion

    public override void Initialize()
    {
        buildingMa = BuildingManager.Instance;

        InitializeButtons();
        queenChamberButton.onClick.AddListener(() =>
        {
            buildingMa.CancelPreview();

            if(buildingMa.queenChambersCount < buildingMa.queenChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
                buildingMa.preview = buildingMa.CreatePreview(buildingMa.queenChamberData, buildingMa.mousePos);
        });

        broodChamberButton.onClick.AddListener(() =>
        {
            buildingMa.CancelPreview();

            if (buildingMa.broodChambersCount < buildingMa.broodChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
            {
                buildingMa.preview = buildingMa.CreatePreview(buildingMa.broodChamberData, buildingMa.mousePos);
            }
        });

        storageChamberButton.onClick.AddListener(() =>
        {
            buildingMa.CancelPreview();

            if (buildingMa.storageChambersCount < buildingMa.storageChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
            {
                buildingMa.preview = buildingMa.CreatePreview(buildingMa.storageChamberData, buildingMa.mousePos);
            }
        });

        tunnelButton.onClick.AddListener(() =>
        {
            buildingMa.CancelPreview();

            buildingMa.preview = buildingMa.CreatePreview(buildingMa.tunnelChamberData, buildingMa.mousePos);
        });     
    }

    private void InitializeButtons()
    {
        if(constructionsButtons.Count > 0)
        {
            foreach (var mb in constructionsButtons)
            {
                if (mb.buttonComponent != null)
                {
                    mb.originalScale = mb.buttonComponent.transform.localScale;
                    SetupButtonEvents(mb);
                }
            }
        }
    }

    private void SetupButtonEvents(ConstructionButton data)
    {
        EventTrigger trigger = data.buttonComponent.GetComponent<EventTrigger>() ??
                              data.buttonComponent.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // Evento Hover Enter
        EventTrigger.Entry enterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enterEntry.callback.AddListener((e) => OnButtonHoverStart(data));
        trigger.triggers.Add(enterEntry);

        // Evento Hover Exit
        EventTrigger.Entry exitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exitEntry.callback.AddListener((e) => OnButtonHoverEnd(data));
        trigger.triggers.Add(exitEntry);
    }

    private void OnButtonHoverStart(ConstructionButton data)
    {
        if (onHoverEmitter != null) SFXManager.PlaySFX(onHoverEmitter);

        // Animación con LeanTween
        LeanTween.cancel(data.buttonComponent.gameObject);
        LeanTween.scale(data.buttonComponent.gameObject, data.originalScale * hoverScale, scaleDuration)
            .setEase(easeType)
            .setIgnoreTimeScale(true);

        // Mostrar panel de información
        UpdateInfoPanel(data.buildingData);
        buildingInfo.SetActive(true);
    }

    private void OnButtonHoverEnd(ConstructionButton data)
    {
        SFXManager.StopSFX(onHoverEmitter);
        // Animación de regreso con LeanTween
        LeanTween.cancel(data.buttonComponent.gameObject);
        LeanTween.scale(data.buttonComponent.gameObject, data.originalScale, scaleDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setIgnoreTimeScale(true);
        buildingInfo.SetActive(false);
    }

   private void UpdateInfoPanel(BuildingData data)
   {
        chamberName.StringReference = data.buildName;
        chamberDescription.StringReference = data.buildDescription;
        //costText.text = data.costMC.ToString() + ", " + data.costHV.ToString();
        costText.text = $"<sprite name=\"huevas\"> {data.costHV}<space=100><sprite name=\"materiales\"> {data.costMC}";
        levelText.text = "lvl 1";
        buildingPreviewImage.sprite = data.previewSprite;
   }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1;
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
        ViewManager.GetView<GameHUDView>().Show();
    }
}
