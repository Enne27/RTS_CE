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
            UIEffects.instance.FadeOutUIObject(object_cg, fadeDuration);
            // Hide de esta view y cambio de escena a escena juego.
        });

        exitButton.onClick.AddListener(ScenesManager.Instance.ExitGame);

        creditsButton.onClick.AddListener(() => 
        {
            ScenesManager.Instance.ChangeScene(CREDITS_SCENE_NAME, true);
        });

        settingsButton.onClick.AddListener(() => 
        { 
            // Hide de esta view y Show de Settings.
        });

    }
}
