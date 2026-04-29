using TMPro;
using UnityEngine;
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
    

    [Header("Buttons")]
    [SerializeField] Button backButton;
    [SerializeField] Button antsInfoButton;
    [SerializeField] Button buildingsInfoButton;

    [Header("ScrollView buildings")]
    [SerializeField] UnityEngine.UIElements.ScrollView scrollView;
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
    }

    /// <summary>
    /// Actualización visual de la era actual del imperio.
    /// </summary>
    /// <param name="currentEra"></param>
    public void UpdateCurrentEraVisuals(HIVE_ERAS currentEra)
    {
        //currentEraImage.sprite = ;
        currentEraText.text = GameManager.instance.player.currentEra.ToString(); // TEMPORAL. SE REQUIERE KEYS DE LAS TABLAS

        UpdateNextEraRequirements(currentEra);
    }

    /// <summary>
    /// Cambio de requisitos al avanzar de era.
    /// </summary>
    /// <param name="currentEra"></param>
    private void UpdateNextEraRequirements(HIVE_ERAS currentEra)
    {
        
    }
}
