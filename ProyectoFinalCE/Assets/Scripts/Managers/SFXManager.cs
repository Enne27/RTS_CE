using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region VARIABLES

    static SFXManager sfxManager;

    List<StudioEventEmitter> sfxEventEmitters = new List<StudioEventEmitter>(); // Pueden haber varios SFX al mismo tiempo.
    float cleanupTimer = 1.0f; // Contadores para limpiar la lista de sonidos.
    float cleanupEvery = 1.0f;

    #endregion

    #region SINGLETON
    /// <summary>
    /// Obtener la instancia de SFXmanager.
    /// </summary>
    public static SFXManager instance
    {
        get
        {
            return RequestSFXManager();
        }
    }

    /// <summary>
    /// Comprobar que ni SFXmanager ni la lista de sonidos sean nulos.
    /// </summary>
    /// <returns></returns>
    static SFXManager RequestSFXManager()
    {
        if (!sfxManager)
        {
            SetSFXManagerInstance(FindFirstObjectByType<SFXManager>());
        }

        return sfxManager;
    }

    /// <summary>
    /// Crear una nueva lista de EventEmitters si no se había inicializado.
    /// </summary>
    static void InitializeSFXManager()
    {
        if (sfxManager == null) return;
        if (sfxManager.sfxEventEmitters == null)
        {
            sfxManager.sfxEventEmitters = new List<StudioEventEmitter>();
        }
    }

    /// <summary>
    /// Setear la instancia del SFX por esta e inicializarla.
    /// </summary>
    /// <param name="anInstance"></param>
    static void SetSFXManagerInstance(SFXManager anInstance)
    {
        sfxManager = anInstance;
        InitializeSFXManager();
    }

    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        if (sfxManager == null)
            SetSFXManagerInstance(this);
        else
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        if (cleanupTimer >= cleanupEvery) // Comprobamos cada cierto tiempo para borrar los SFX que se hayan quedado colgados allí.
            CleanUp();
        else
        {
            cleanupTimer += Time.deltaTime;
        }
    }
    #endregion

    #region SINGLE_SFX
    /// <summary>
    /// Hacemos sonar el SFX que queramos y lo añadimos a la lista si no estaba.
    /// </summary>
    /// <param name="e"></param>
    public static void PlaySFX(StudioEventEmitter e)
    {
        if (sfxManager == null) RequestSFXManager();
        if (!instance.sfxEventEmitters.Contains(e))
        {
            sfxManager.sfxEventEmitters.Add(e);
        }
        e.Play();
    }

    /// <summary>
    /// Detener el SFX que esté sonando y eliminarlo de nuestra lista.
    /// </summary>
    /// <param name="e"></param>
    public static void StopSFX(StudioEventEmitter e)
    {
        if (instance.sfxEventEmitters.Contains(e))
        {
            sfxManager.sfxEventEmitters.Remove(e);
        }
        e.Stop();
    }
    #endregion

    #region ALL_SFX
    /// <summary>
    /// Silenciar todos los SFX a la vez.
    /// </summary>
    public static void StopAllSFX()
    {
        foreach (StudioEventEmitter e in instance.sfxEventEmitters)
        {
            e.Stop();
        }
    }

    /// <summary>
    /// Pausar el sonido de cada SFX que esté en la lista en ese momento.
    /// </summary>
    public static void Pause()
    {
        foreach (StudioEventEmitter e in instance.sfxEventEmitters)
        {
            e.EventInstance.setPaused(true);
        }
    }

    /// <summary>
    /// Volver a hacer sonar los SFX que estén en la lista.
    /// </summary>
    public static void Resume()
    {
        if (instance.sfxEventEmitters.Count > 0)
        {
            foreach (StudioEventEmitter e in sfxManager.sfxEventEmitters)
            {
                if (e != null) e.EventInstance.setPaused(false);
            }
        }
    }

    /// <summary>
    /// Eliminamos los SFX que estén en la lista porque no le hayamos dado a Stop cuando hayan terminado.
    /// </summary>
    private void CleanUp()
    {
        sfxEventEmitters.RemoveAll((StudioEventEmitter e) => e == null || !e.IsPlaying());
    }
    #endregion
}