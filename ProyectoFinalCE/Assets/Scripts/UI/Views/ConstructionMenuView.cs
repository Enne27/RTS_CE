using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ConstructionMenuView : View
{
    [System.Serializable]
    public class ConstructionButton
    {
        public Button buttonComponent;
        public BuildingData buildingData;
    }

    #region VARIABLES
    [SerializeField] private List<ConstructionButton> constructionsButtons;
    [SerializeField] GameObject buildingInfo;

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

    private Vector3 originalScale;

    [Header("Sound")]
    [Tooltip("Event Emitter del sonido para onHover un botón.")] 
    [SerializeField] StudioEventEmitter onHoverEmitter;
    #endregion

    public override void Initialize()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        foreach (var mb in constructionsButtons)
        {
            if (mb.buttonComponent != null)
            {
                originalScale = mb.buttonComponent.transform.localScale;
                SetupButtonEvents(mb);
            }
        }
    }

    private void SetupButtonEvents(ConstructionButton data)
    {
        EventTrigger trigger = data.buttonComponent.GetComponent<EventTrigger>() ??
                              data.buttonComponent.gameObject.AddComponent<EventTrigger>();

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
        exitEntry.callback.AddListener((e) => OnButtonHoverEnd(data.buttonComponent));
        trigger.triggers.Add(exitEntry);

        // Evento Click
        //data.buttonComponent.onClick.AddListener(() =>);
    }

    private void OnButtonHoverStart(ConstructionButton data)
    {
        SFXManager.PlaySFX(onHoverEmitter);
        // Animación con LeanTween
        LeanTween.cancel(data.buttonComponent.gameObject);
        LeanTween.scale(data.buttonComponent.gameObject, originalScale * hoverScale, scaleDuration)
            .setEase(easeType);

        // Mostrar panel de información
        //UpdateInfoPanel(data.buildingData);
        buildingInfo.SetActive(true);
    }

    private void OnButtonHoverEnd(Button button)
    {
        SFXManager.StopSFX(onHoverEmitter);
        // Animación de regreso con LeanTween
        LeanTween.cancel(button.gameObject);
        LeanTween.scale(button.gameObject, originalScale, scaleDuration)
            .setEase(LeanTweenType.easeInOutQuad);
        buildingInfo.SetActive(false);
    }

    /*private void UpdateInfoPanel(Minigame data)
    {
        nameLocalizedText.StringReference = data.minigameName;
        descriptionLocalizedText.StringReference = data.minigameDescription;
        cost. = data.minigameControls;
    }*/
}
