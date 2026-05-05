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

        advancedAudioButton.onClick.AddListener(() => ViewManager.Show<AdvancedSettingsAudioView>());

        //advancedControlsButton.onClick.AddListener(()=> ViewManager.Show<AdvancedSettingsControlsView>());
        advancedQualityButton.onClick.AddListener(()=> ViewManager.Show<AdvancedSettingsQualityView>());
    }

    public override void Show()
    {
        base.Show();

        SettingsManager.instance.isLoading = true;

        UIEffects.instance.FadeInUIObject(object_cg, fadeDuration);
        
        SettingsManager.instance.LoadSettings();
        SettingsManager.instance.UpdateResolutionDropdownLabels();
        SettingsManager.instance.ApplySettingsInternal();
        SettingsManager.instance.SyncUI();

        SettingsManager.instance.isLoading = false;
    }

    public override void Hide()
    {
        //base.Hide();
        UIEffects.instance.FadeOutUIObject(object_cg, fadeDuration, () => { base.Hide(); });
    }
}
