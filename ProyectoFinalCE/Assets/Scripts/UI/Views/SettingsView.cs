using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    #region VARIABLES
    [SerializeField] Button backButton;
    #endregion
    public override void Initialize()
    {
        backButton.onClick.AddListener(()=> ViewManager.ShowLastView());
    }
}
