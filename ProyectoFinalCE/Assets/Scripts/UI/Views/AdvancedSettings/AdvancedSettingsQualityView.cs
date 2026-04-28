using UnityEngine;
using UnityEngine.UI;

public class AdvancedSettingsQualityView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button backButton;
    #endregion

    public override void Initialize()
    {
        backButton.onClick.AddListener(() => ViewManager.ShowLastView(1, false));

    }
}
