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
    [SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] Image userImage;
    [SerializeField] Image userColorImage;

    [Header("Era info")]
    [SerializeField] TextMeshProUGUI currentEraText;
    [SerializeField] Image currentEraImage;
    [SerializeField] RequirementNextEraSlot slotRequirement;
    

    [Header("Buttons")]
    [SerializeField] Button backButton;
    [SerializeField] Button antsInfoButton;
    [SerializeField] Button buildingsInfoButton;

    [Header("ScrollView buildings")]
    [SerializeField] GameObject layoutHorizontal;
    //[SerializeField] UnityEngine.UIElements.ScrollView scrollView;
    #endregion
    public override void Initialize()
    {
        if (backButton != null)
            backButton.onClick.AddListener(()=> ViewManager.ShowLastView(1, false));

        /*if (antsInfoButton != null)
            antsInfoButton.onClick.AddListener(()=> );

        if (buildingsInfoButton != null)
            buildingsInfoButton.onClick.AddListener(()=> );

        */

        if(userNameText != null)
            userNameText.text = GameManager.instance.player.playerName;

        /*if(userImage != null)
            userImage = ;*/

        if(userColorImage != null)
            userColorImage.color = GameManager.instance.player.playerColor;

        if(currentEraImage != null)
            currentEraImage.sprite = EraManager.instance.ERAS_IMAGES[GameManager.instance.player.currentEra];

        UpdateNextEraRequirements(GameManager.instance.player.currentEra);
    }

    /// <summary>
    /// Actualización visual de la era actual del imperio.
    /// </summary>
    /// <param name="currentEra"></param>
    public void UpdateCurrentEraVisuals(HIVE_ERAS currentEra, LocalizedString newEraName)
    {
        currentEraImage.sprite = EraManager.instance.ERAS_IMAGES[currentEra];

        currentEraText.gameObject.GetComponent<LocalizeStringEvent>().StringReference = newEraName;

        newEraName.StringChanged -= OnEraChanged;
        newEraName.StringChanged += OnEraChanged;

        UpdateNextEraRequirements(currentEra);
    }
    private void OnEraChanged(string value)
    {
        currentEraText.text = value;
    }

    /// <summary>
    /// Cambio de requisitos al avanzar de era.
    /// </summary>
    /// <param name="currentEra"></param>
    private void UpdateNextEraRequirements(HIVE_ERAS currentEra)
    {
        switch (currentEra)
        {
            case HIVE_ERAS.BROTE:
                RequirementNextEraSlot firstSlot = Instantiate(slotRequirement, layoutHorizontal.transform);
                RequirementNextEraSlot secondtSlot = Instantiate(slotRequirement, layoutHorizontal.transform);
                RequirementNextEraSlot thirdSlot = Instantiate(slotRequirement, layoutHorizontal.transform);
                RequirementNextEraSlot fourthSlot = Instantiate(slotRequirement, layoutHorizontal.transform);
                RequirementNextEraSlot fifthtSlot = Instantiate(slotRequirement, layoutHorizontal.transform);
                break;
            case HIVE_ERAS.NIDO:
                break;
            case HIVE_ERAS.COLONIA:
                break;
            case HIVE_ERAS.IMPERIO:
                break;
        }
    }
}
