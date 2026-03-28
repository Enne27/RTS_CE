using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    #region SINGLETON
    public static ScenesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// Método para cambiar de escena usando el ViewManager.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="rememberCurrentScene">Recordar la escena actual, no la nueva.</param>
    public void ChangeScene(string sceneName, bool rememberCurrentScene)
    {
        if (rememberCurrentScene)
            ViewManager.Instance.RememberCurrentView();

        ChangeScene(sceneName);
    }

    /// <summary>
    /// Método base para cambiar de escena usando su nombre.
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Salir del juego y si estamos en editor, detener la ejecución de la ventana Game.UI_manager
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
