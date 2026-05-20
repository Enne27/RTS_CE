using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstantsAndKeys;

public class MusicManager : MonoBehaviour
{
    #region VARIABLES
    StudioEventEmitter musicEventEmitter;

    [Header("Event References de eventos de MÚSICA de FMOD.")]

    [Tooltip("Event reference de la música de los menús.")]
    [SerializeField] EventReference menusMusicReference;

    [Tooltip("Event reference de las escenas de juego.")]
    [SerializeField] EventReference gameMusic;
    #endregion

    #region SINGLETON
    static MusicManager musicManager;

    /// <summary>
    /// Instancia estática del musicManager a la que llamamos cuando lo necesitamos.
    /// </summary>
    public static MusicManager instance
    {
        get
        {
            return RequestMusicManager();
        }
    }

    /// <summary>
    /// Encontramos el musicManager que haya en la escena.
    /// </summary>
    /// <returns>MusicManager</returns>
    private static MusicManager RequestMusicManager()
    {
        if (musicManager == null)
        {
            musicManager = FindFirstObjectByType<MusicManager>();
        }
        return musicManager;
    }
    #endregion

    private void Awake()
    {
        musicEventEmitter = GetComponentInChildren<StudioEventEmitter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscripción al cargar la escena.
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Debe eliminarse la subscripción para que no se llame más de una vez.
    }
    private void OnDestroy()
    {
        if (musicEventEmitter != null)
        {
            musicEventEmitter.Stop();
        }
    }
    #region CHANGE_MUSIC_REFERENCE

    /// <summary>
    /// Subscripción de carga de escena para cambiar la música cuando ocurra.
    /// </summary>
    /// <param name="escena"></param>
    /// <param name="arg1"></param>
    private void OnSceneLoaded(Scene escena, LoadSceneMode arg1)
    {
        //UnityEngine.Debug.Log("Loaded Scene" + escena.name);
        StopMusic();
        switch (escena.name)
        {
            case MAIN_MENU_SCENE_NAME:
            case CREDITS_SCENE_NAME:
                SwitchMusicReference(menusMusicReference);
                break;

            case SINGLE_PLAYER_GAME_SCENE_NAME:
            case CREATIVE_MODE_SCENE_NAME:
                SwitchMusicReference(gameMusic);
                break;
        }
    }

    /// <summary>
    /// CAMBIAR DE MÚSICA SIN CAMBIO DE ESCENA.
    /// Le pasamos el EventReference que corresponda, desde inspector.
    /// Detenemos el que estaba sonando, cambiamos la referencia y hacemos sonar de nuevo.
    /// </summary>
    /// <param name="newReference"></param>
    public void SwitchMusicReference(EventReference newReference, float fadeOutDuration = 1.0f)
    {
        StartCoroutine(_SwitchMusicReference(newReference, fadeOutDuration));
    }

    /// <summary>
    /// Enumerator para cambiar la música.
    /// </summary>
    /// <param name="newReference"></param>
    /// <param name="fadeOutDuration"></param>
    /// <returns></returns>
    IEnumerator _SwitchMusicReference(EventReference newReference, float fadeOutDuration = 1.0f)
    {
        SFXManager.StopAllSFX();
        StopMusic();

        yield return new WaitForSecondsRealtime(fadeOutDuration);

        musicEventEmitter.EventReference = newReference;
        musicEventEmitter.ForceLookUp(); // Método para obligar al evento a buscar la referencia de nuevo, por si ya ha hecho play.
        PlayMusic();
        MusicVolumeParams.instance.ApplyCurrentState();
    }

    #endregion

    #region ParameterActions
    /// <summary>
    /// Getter para el event emitter que necesitamos para modificar los parámetros.
    /// </summary>
    /// <returns></returns>
    public StudioEventEmitter GetEventEmitter()
    {
        return musicEventEmitter;
    }

    /// <summary>
    /// Método para cambiar el valor de los parámetros de FMOD. 
    /// </summary>
    /// <param name="eventEmitter"></param>
    /// <param name="paramName">Nombre del parámetro a modificar.</param>
    /// <param name="newValue">Nuevo valor del parámetro.</param>
    public void ChangeParameterValue(StudioEventEmitter eventEmitter, string paramName, float newValue)
    {
        eventEmitter.EventInstance.setParameterByName(paramName, newValue);
    }
    #endregion

    #region MusicActions
    /// <summary>
    /// Pausamos la música que esté sonando.
    /// </summary>
    public void PauseMusic()
    {
        musicEventEmitter.EventInstance.setPaused(true);
    }

    /// <summary>
    /// Volvemos a reanudar la música.
    /// </summary>
    public void ResumeMusic()
    {
        musicEventEmitter.EventInstance.setPaused(false); // En los event emitters no existe directamente un setPause
    }

    /// <summary>
    /// Detenemos la música que esté sonando.
    /// </summary>
    public void StopMusic()
    {
        musicEventEmitter.Stop();
    }

    /// <summary>
    /// Ponemos la música a Play.
    /// Debe hacerse por separado del SwitchMusicReference porque no termina de detectar los tiempos para cambiar bien.
    /// </summary>
    public void PlayMusic()
    {
        musicEventEmitter.Play();
    }
    #endregion
}