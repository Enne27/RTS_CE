using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class CreditsView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button exitButton;
    [SerializeField] Button restartButton;
    #endregion
    public override void Initialize()
    {
        exitButton.onClick.AddListener(() =>
        {
            ScenesManager.Instance.ExitGame();
        });

        restartButton.onClick.AddListener(() =>
        {
            ScenesManager.Instance.ChangeScene(MAIN_MENU_SCENE_NAME, true);
        });
    }
}
