using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public QualityLevel quality;
    public WindowMode windowMode;
    public float masterVolume = 1f;
    public bool isMuted = false;
    public Language language;
}

public enum QualityLevel { MuyAlta, Alta, Media, Baja }
public enum WindowMode { Ventana, SinBordes, PantallaCompleta }
public enum Language { Castellano, Catalan, Ingles }

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Data")]
    public GameSettings settings;

    [Header("UI References (TextMeshPro)")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown windowDropdown;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public TMP_Dropdown languageDropdown;

    private Bus masterBus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            masterBus = RuntimeManager.GetBus("bus:/");

            LoadSettings();
            ApplySettings();
            SyncUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HookUI();
    }

    void HookUI()
    {
        qualityDropdown.onValueChanged.AddListener((i) => SetQuality((QualityLevel)i));
        windowDropdown.onValueChanged.AddListener((i) => SetWindowMode((WindowMode)i));
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        languageDropdown.onValueChanged.AddListener((i) => SetLanguage((Language)i));
    }

    void SyncUI()
    {
        qualityDropdown.SetValueWithoutNotify((int)settings.quality);
        windowDropdown.SetValueWithoutNotify((int)settings.windowMode);
        volumeSlider.SetValueWithoutNotify(settings.masterVolume);
        muteToggle.SetIsOnWithoutNotify(settings.isMuted);
        languageDropdown.SetValueWithoutNotify((int)settings.language);
    }

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

    public void SetVolume(float volume)
    {
        settings.masterVolume = volume;
        ApplyVolume();
        PlayerPrefs.SetFloat("Volume", volume);
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
        masterBus.setVolume(volume);
    }

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

    public void LoadSettings()
    {
        settings.quality = (QualityLevel)PlayerPrefs.GetInt("Quality", 2);
        settings.windowMode = (WindowMode)PlayerPrefs.GetInt("WindowMode", 0);
        settings.masterVolume = PlayerPrefs.GetFloat("Volume", 1f);
        settings.isMuted = PlayerPrefs.GetInt("Mute", 0) == 1;
        settings.language = (Language)PlayerPrefs.GetInt("Language", 0);
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
}
