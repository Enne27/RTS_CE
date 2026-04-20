using UnityEngine;
using UnityEngine.UI;

public class AdvancedSettingsAudioView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button backButton;

    [Header("Sliders")]
    [SerializeField] Slider SFXslider;
    [SerializeField] Slider musicSlider;
    #endregion

    public override void Initialize()
    {
        backButton.onClick.AddListener(()=> ViewManager.ShowLastView(1, false));

        //SFXslider.onValueChanged.AddListener();
        //musicSlider.onValueChanged.AddListener();
    }
}
