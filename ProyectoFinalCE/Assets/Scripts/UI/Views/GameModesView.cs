using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class GameModesView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button singlePlayerButton;
    [SerializeField] Button singleLoadPlayerButton;
    [SerializeField] Button creativeModeButton;
    [SerializeField] Button backButton;
    #endregion

    public override void Initialize()
    {
        singlePlayerButton.onClick.AddListener(() =>
        {
            ScenesManager.Instance.ChangeScene(SINGLE_PLAYER_GAME_SCENE_NAME, false);
        });

        singleLoadPlayerButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoadGameAfterScene());
        });

        creativeModeButton.onClick.AddListener(() =>
        {
            
        });

        backButton.onClick.AddListener(() =>
        {
            ViewManager.ShowLastView(1, false);
        });
    }

    private IEnumerator LoadGameAfterScene()
    {
        Debug.Log("GameModesView.LoadGameAfterScene: start");

        // Marcar que se va a cargar una partida ANTES de cambiar de escena
        AntCreation.MarkLoaded();
        Debug.Log("GameModesView.LoadGameAfterScene: MarkLoaded called");

        // Cambiar escena
        ScenesManager.Instance.ChangeScene(SINGLE_PLAYER_GAME_SCENE_NAME, false);

        yield return new WaitUntil(() => GameManager.instance != null && GameManager.instance.player != null && AntCreation.Instance != null && GameFactory.Instance != null);

        Debug.Log("GameModesView.LoadGameAfterScene: scene ready, loading save");
        SaveSystem.LoadGame();
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