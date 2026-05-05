using UnityEngine;
using UnityEngine.UI;
using static ConstantsAndKeys;

public class PauseMenuView : View
{
    #region VARIABLES
    [Header("Buttons")]
    [SerializeField] Button settingsButton;
    [SerializeField] Button startButton;
    [SerializeField] Button controlsButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button backButton;

    [Header("Cameras")]
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] CameraMovement2D cameraMovementNest;

    #endregion
    public override void Initialize()
    {
        startButton.onClick.AddListener(() => 
        {
            //Hide();
            ScenesManager.Instance.ChangeScene(MAIN_MENU_SCENE_NAME, false);
            //ViewManager.ShowLastView();
        });

        controlsButton.onClick.AddListener(() => 
        {
            //Hide();
            ViewManager.Show<ControlsView>();
        });

        settingsButton.onClick.AddListener(() =>
        {
            //Hide();
            ViewManager.Show<SettingsView>();
        });

        /* TBI
        saveButton.onClick.AddListener(() =>
        {
            //Hide();
            SaveSystem.SaveGame();
        });
        */

        backButton.onClick.AddListener(() => 
        {
            //Hide();
            //ViewManager.ShowLastView(1, false);
            PauseController.instance.TogglePause();
        });
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
        cameraMovement?.DisableCameraInput();
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1;
        cameraMovement?.EnableCameraInput();


    }
}
