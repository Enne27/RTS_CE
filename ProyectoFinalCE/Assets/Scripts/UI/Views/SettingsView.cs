using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    #region VARIABLES
    [SerializeField] Button backButton;
    [SerializeField] float fadeDuration;
    #endregion
    public override void Initialize()
    {
        backButton.onClick.AddListener(()=> ViewManager.ShowLastView());
    }

    public override void Show()
    {
        base.Show();
        //UIEffects.instance.FadeInUIObject(object_cg, fadeDuration);
    }

    public override void Hide()
    {
        base.Hide();
        //UIEffects.instance.FadeOutUIObject(object_cg, fadeDuration, () => { base.Hide(); });
    }
}
