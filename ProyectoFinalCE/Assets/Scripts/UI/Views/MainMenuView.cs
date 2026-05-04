using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class MainMenuView : View
{
    #region VARIABLES
    [Header("Parameters Efectos UI")]
    [SerializeField] float fadeDuration = 0.5f;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button settingsButton;
    #endregion

    public override void Initialize()
    {
        if(guidComponent == null) 
            guidComponent = GetComponent<GuidComponent>();

        if (object_cg == null)
            object_cg = GetComponent<CanvasGroup>();

        startButton.onClick.AddListener(() => 
        {
            // Hide de esta view y cambio de escena a escena juego.
            Hide();
            ViewManager.Show<GameModesView>();
        });

        exitButton.onClick.AddListener(ScenesManager.Instance.ExitGame);

        creditsButton.onClick.AddListener(() => 
        {
            Hide();
            ScenesManager.Instance.ChangeScene(CREDITS_SCENE_NAME, false);
        });

        settingsButton.onClick.AddListener(() => 
        {
            // Hide de esta view y Show de Settings.
            //Hide();
            ViewManager.Show<SettingsView>();
        });
    }

    public override void Show()
    {
        //base.Show();
        UIEffects.instance.FadeInUIObject(object_cg, fadeDuration, ()=> { base.Show(); });
        //object_cg.alpha = 1f;
    }

    public override void Hide()
    {
        //base.Hide();
        UIEffects.instance.FadeOutUIObject(object_cg, fadeDuration, () => { base.Hide(); });
    }
}
