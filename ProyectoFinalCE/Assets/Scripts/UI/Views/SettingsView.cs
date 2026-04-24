using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    #region VARIABLES
    [SerializeField] Button backButton;
    [SerializeField] float fadeDuration;

    [Header("AdvancedSettings")]
    [SerializeField] Button advancedQualityButton;
    [SerializeField] Button advancedAudioButton;
    [SerializeField] Button advancedControlsButton;
    #endregion
    public override void Initialize()
    {
        backButton.onClick.AddListener(()=> ViewManager.ShowLastView());

        advancedQualityButton.onClick.AddListener(()=> ViewManager.Show<AdvancedSettingsQualityView>());
        advancedAudioButton.onClick.AddListener(() => ViewManager.Show<AdvancedSettingsAudioView>());

        //advancedControlsButton.onClick.AddListener(()=> ViewManager.Show<AdvancedSettingsControlsView>());
    }

    public override void Show()
    {
        base.Show();
        SettingsManager.instance.LoadSettings();
        SettingsManager.instance.ApplySettings();
        SettingsManager.instance.SyncUI();
        //UIEffects.instance.FadeInUIObject(object_cg, fadeDuration);
    }

    public override void Hide()
    {
        base.Hide();
        //UIEffects.instance.FadeOutUIObject(object_cg, fadeDuration, () => { base.Hide(); });
    }
}
