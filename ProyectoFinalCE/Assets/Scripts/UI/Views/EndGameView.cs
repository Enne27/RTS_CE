using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class EndGameView : View
{
    #region VARIABLES
    [SerializeField] TextMeshProUGUI victoryTMP;
    [SerializeField] TextMeshProUGUI defeatTMP;
    [SerializeField] Image imageVictoryDefeat;

    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsButton;

    [Header("Sprites")]
    [SerializeField] Sprite victorySprite;
    [SerializeField] Sprite defeatSprite;

    [Header("Sound")]
    [SerializeField] StudioEventEmitter finalSFXSoundEmitter;
    [SerializeField] EventReference finalSFXSoundReferenceVictory;
    [SerializeField] EventReference finalSFXSoundReferenceDefeat;

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
            imageVictoryDefeat.sprite = victorySprite;

            finalSFXSoundEmitter.EventReference = finalSFXSoundReferenceVictory;
        }
        else
        {
            victoryTMP.gameObject.SetActive(false);
            defeatTMP.gameObject.SetActive(true);
            imageVictoryDefeat.sprite = defeatSprite;

            finalSFXSoundEmitter.EventReference = finalSFXSoundReferenceDefeat;
        }

        if (SFXManager.instance != null && finalSFXSoundEmitter != null)
            SFXManager.PlaySFX(finalSFXSoundEmitter);
    }

    private void GoToMainMenu()
    {
        GameManager.instance.ResetValues();
        ScenesManager.Instance.ChangeScene(MAIN_MENU_SCENE_NAME, false);

        if (SFXManager.instance != null)
            SFXManager.StopAllSFX();
    }

    public override void Show()
    {
        base.Show();

    }

    public override void Hide()
    {
        base.Hide();
    }
}