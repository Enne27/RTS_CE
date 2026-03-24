using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class GameModesView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button singlePlayerButton;
    [SerializeField] Button creativeModeButton;
    [SerializeField] Button backButton;
    #endregion

    public override void Initialize()
    {
        singlePlayerButton.onClick.AddListener(() => 
        {
            ScenesManager.Instance.ChangeScene(SINGLE_PLAYER_GAME_SCENE_NAME, false);
        });

        creativeModeButton.onClick.AddListener(() => 
        {
            
        });

        backButton.onClick.AddListener(() => 
        {
            ViewManager.ShowLastView(1, false);
        });
    }
}
