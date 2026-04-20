using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using FMODUnity;
using FMOD.Studio;



public enum QualityLevel { MuyAlta, Alta, Media, Baja }
public enum WindowMode { Ventana, SinBordes, PantallaCompleta }
public enum Language { Castellano, Catalan, Ingles }

public class SettingsManager : MonoBehaviour
{
    #region VARIABLES
    public static SettingsManager Instance;

    [Header("Data")]
    public GameSettings settings;

    [Header("UI References General")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown windowDropdown;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public TMP_Dropdown languageDropdown;

    [Header("UI References Advanced")]
    public Slider sfxSlider;
    public Slider musicSlider;

    private Bus masterBus;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        settings.musicVCA = RuntimeManager.GetVCA("vca:/Music");
        settings.sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

        LoadSettings();
        HookUI();
        ApplySettings();
        SyncUI();
    }

    /// <summary>
    /// Añadir los listeners a los elementos de UI de la configuración general.
    /// </summary>
    void HookUI()
    {
        // General
        qualityDropdown.onValueChanged.AddListener((i) => SetQuality((QualityLevel)i));
        windowDropdown.onValueChanged.AddListener((i) => SetWindowMode((WindowMode)i));
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        languageDropdown.onValueChanged.AddListener((i) => SetLanguage((Language)i));

        // Advanced
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    /// <summary>
    /// Actualizar los valores de la interfaz.
    /// </summary>
    void SyncUI()
    {
        // General
        qualityDropdown.SetValueWithoutNotify((int)settings.quality);
        windowDropdown.SetValueWithoutNotify((int)settings.windowMode);
        volumeSlider.SetValueWithoutNotify(settings.masterVolume);
        muteToggle.SetIsOnWithoutNotify(settings.isMuted);
        languageDropdown.SetValueWithoutNotify((int)settings.language);

        // Advanced
        musicSlider.SetValueWithoutNotify(settings.musicVolume);
        sfxSlider.SetValueWithoutNotify(settings.sfxVolume);
    }

    #region SCREEN
    public void SetQuality(QualityLevel level)
    {
        settings.quality = level;

        QualitySettings.SetQualityLevel((int)level);
        PlayerPrefs.SetInt("Quality", (int)level);
    }

    public void SetWindowMode(WindowMode mode)
    {
        settings.windowMode = mode;

        switch (mode)
        {
            case WindowMode.Ventana:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case WindowMode.SinBordes:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case WindowMode.PantallaCompleta:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        PlayerPrefs.SetInt("WindowMode", (int)mode);
    }
    #endregion

    #region AUDIO
    public void SetVolume(float volume)
    {
        settings.masterVolume = volume;
        ApplyVolume();
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        settings.musicVolume = volume;
        ApplyVolume();
        PlayerPrefs.SetFloat("MusiVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        settings.sfxVolume = volume;
        ApplyVolume();
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void ToggleMute(bool mute)
    {
        settings.isMuted = mute;
        ApplyVolume();
        PlayerPrefs.SetInt("Mute", mute ? 1 : 0);
    }

    private void ApplyVolume()
    {
        float volume = settings.isMuted ? 0f : settings.masterVolume;
        float musicVolume = settings.isMuted ? 0f : settings.musicVolume;
        float sfxVolume = settings.isMuted ? 0f : settings.sfxVolume;
        
        masterBus.setVolume(volume);
        settings.musicVCA.setVolume(musicVolume);
        settings.sfxVCA.setVolume(sfxVolume);

    }
    #endregion

    public void SetLanguage(Language lang)
    {
        settings.language = lang;

        string localeCode = "en";

        switch (lang)
        {
            case Language.Castellano:
                localeCode = "es-ES";
                break;
            case Language.Catalan:
                localeCode = "ca";
                break;
            case Language.Ingles:
                localeCode = "en";
                break;
        }

        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning("Locale no encontrado: " + localeCode);
        }

        PlayerPrefs.SetInt("Language", (int)lang);
    }


    #region PLAYER_PREFS
    public void LoadSettings()
    {
        settings.quality = (QualityLevel)PlayerPrefs.GetInt("Quality", 2);
        settings.windowMode = (WindowMode)PlayerPrefs.GetInt("WindowMode", 0);
        settings.masterVolume = PlayerPrefs.GetFloat("Volume", 1f);
        settings.isMuted = PlayerPrefs.GetInt("Mute", 0) == 1;
        settings.language = (Language)PlayerPrefs.GetInt("Language", 0);

        //Advanced
        settings.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        settings.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void ApplySettings()
    {
        SetQuality(settings.quality);
        SetWindowMode(settings.windowMode);
        ApplyVolume();
        SetLanguage(settings.language);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    #endregion
}
