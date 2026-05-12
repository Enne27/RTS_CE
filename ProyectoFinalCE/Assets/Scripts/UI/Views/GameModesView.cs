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
        // Cambiar escena
        ScenesManager.Instance.ChangeScene(SINGLE_PLAYER_GAME_SCENE_NAME, false);
        yield return new WaitForSeconds(1f);
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
