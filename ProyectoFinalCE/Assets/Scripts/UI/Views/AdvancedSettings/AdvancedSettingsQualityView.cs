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

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1;
    }
}
