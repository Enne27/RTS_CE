using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class EndGameView : View
{
    #region VARIABLES
    [SerializeField] TextMeshProUGUI victoryTMP;
    [SerializeField] TextMeshProUGUI defeatTMP;

    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsButton;
    #endregion


    public override void Initialize()
    {
        restartButton.onClick.AddListener(GoToMainMenu);
        creditsButton.onClick.AddListener(() => ScenesManager.Instance.ChangeScene(CREDITS_SCENE_NAME, false));
        exitButton.onClick.AddListener(() => ScenesManager.Instance.ExitGame());
    }

    public void PlayerWin(bool hasWon)
    {
        if (hasWon)
        {
            victoryTMP.gameObject.SetActive(true);
            defeatTMP.gameObject.SetActive(false);
        }
        else
        {
            victoryTMP.gameObject.SetActive(false);
            defeatTMP.gameObject.SetActive(true);
        }
    }

    private void GoToMainMenu()
    {
        GameManager.instance.ResetValues();
        ScenesManager.Instance.ChangeScene(MAIN_MENU_SCENE_NAME, false);
    } 
}