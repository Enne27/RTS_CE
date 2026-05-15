using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static PlayerConstants;

public class GeneralInfoView : View
{
    #region VARIABLES
    [Header("User info")]
    /*[SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] Image userImage;
    [SerializeField] Image userColorImage;*/

    [Header("Era info")]
    [SerializeField] TextMeshProUGUI currentEraText;
    [SerializeField] Image currentEraImage;

    [SerializeField] Image currentEraGameHUDButton;
    
    [Header("Requirements")]
    [SerializeField] RequirementNextEraSlot slotPrefab;
    [SerializeField] Transform layoutHorizontal;

    [Header("Buttons")]
    [SerializeField] Button backButton;
    /*[SerializeField] Button antsInfoButton;
    [SerializeField] Button buildingsInfoButton;*/

    private List<RequirementNextEraSlot> activeSlots = new();

    #endregion

    private void OnEnable()
    {
        EraManager.instance.RefreshUI();
    }

    public override void Initialize()
    {
        var era = GameManager.instance.player.currentEra;

        if (backButton != null)
            backButton.onClick.AddListener(()=> ViewManager.ShowLastView(1, false));

        /*
         * if (antsInfoButton != null)
            antsInfoButton.onClick.AddListener(()=> );

        if (buildingsInfoButton != null)
            buildingsInfoButton.onClick.AddListener(()=> );

        

        if(userNameText != null
            userNameText.text = GameManager.instance.player.playerName;

        if(userImage != null)
            userImage = ;

        if(userColorImage != null)
            userColorImage.color = GameManager.instance.player.playerColor;
        */

        /*if(currentEraImage != null)
            currentEraImage.sprite = EraManager.instance.GetEraSprite(era);*/

        UpdateCurrentEraVisuals(
            era,
            EraManager.instance.GetEraName(era)
        );

        UpdateRequirements(
            EraManager.instance.GetRequirements(era)
        );
    }

    /// <summary>
    /// Actualización visual de la era actual del imperio.
    /// </summary>
    /// <param name="currentEra"></param>
    public void UpdateCurrentEraVisuals(HIVE_ERAS currentEra, LocalizedString newEraName)
    {
        currentEraImage.sprite = EraManager.instance.eraSprites[currentEra];
        currentEraGameHUDButton.sprite = currentEraImage.sprite;

        currentEraText.gameObject.GetComponent<LocalizeStringEvent>().StringReference = newEraName;

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
            EraManager.instance.InitData();
            EraManager.instance.InitRequirements();
            EraManager.instance.RefreshUI();
        }

        foreach (var s in activeSlots)
            Destroy(s.gameObject);

        activeSlots.Clear();

        foreach (var req in requirements)
        {
            var slot = Instantiate(slotPrefab, layoutHorizontal);
            slot.Bind(req);
            activeSlots.Add(slot);
        }
    }

    private void CreateSlot(RequirementID id,  int totalQuantity)
    {
        RequirementNextEraSlot slot = Instantiate(slotPrefab, layoutHorizontal.transform);
        slot.Setup(id, 0, totalQuantity, false);
    }
}
