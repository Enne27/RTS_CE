using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum QualityLevel { VeryHigh, High, Medium, Low, Custom }
public enum WindowMode { Window, Borderless, Fullscreen }
public enum AspectRatio { _16x9, _16x10, _21x9, _32x9 }
public enum Resolution { HD, PHD, FHD, QHD, UHD }
public enum UpscalingFilter { Automatic, Bilinear, NearestNeighbor, FidelityFXSuperResolution, SpatialTemporalPostProcessing }
public enum AntiAliasing { Disabled, _2x, _4x, _8x }
public enum ShadowQuality { Low, Medium, High }
public enum ShadowDistance { VeryClose, Close, Far, VeryFar }
public enum Language { Spanish, Catalan, English }

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager settingsManager;

    [Header("Data")]
    public GameSettings settings;
    public UniversalRenderPipelineAsset CustomRenderer;

    [Header("UI General")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown windowDropdown;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public TMP_Dropdown languageDropdown;

    [Header("UI Advanced Graphics")]
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown screenResolutionDropdown;
    public TextMeshProUGUI presetSelected;
    public Slider renderScaleSlider;
    public TextMeshProUGUI renderScaleLabel;
    public TMP_Dropdown upscalingFilter;
    public Toggle highDynamicRangeToggle;
    public TMP_Dropdown antialiasingDropdown;
    public TMP_Dropdown shadowQualityDropdown;
    public TMP_Dropdown shadowDistanceDropdown;

    [Header("UI Audio")]
    public Slider sfxSlider;
    public Slider musicSlider;

    private Bus masterBus;

    public bool isLoading = false;

    public static SettingsManager instance
    {
        get
        {
            if (settingsManager == null)
                settingsManager = FindFirstObjectByType<SettingsManager>();
            return settingsManager;
        }
    }

    private void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        settings.musicVCA = RuntimeManager.GetVCA("vca:/Music");
        settings.sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

        isLoading = true;

        LoadSettings();
        SyncUI();
        ApplySettings();

        isLoading = false;

        HookUI();
        UpdatePresetText();


    }

    void HookUI()
    {
        qualityDropdown.onValueChanged.AddListener((i) => SetQuality((QualityLevel)i));
        windowDropdown.onValueChanged.AddListener((i) => SetWindowMode((WindowMode)i));
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        languageDropdown.onValueChanged.AddListener((i) => SetLanguage((Language)i));

        // Advanced Graphics
        aspectRatioDropdown.onValueChanged.AddListener((i) => SetAspectRatio((AspectRatio)i));
        screenResolutionDropdown.onValueChanged.AddListener((i) => SetResolution((Resolution)i));
        renderScaleSlider.onValueChanged.AddListener(SetRenderScale);
        upscalingFilter.onValueChanged.AddListener((i) => SetUpscaling((UpscalingFilter)i));
        highDynamicRangeToggle.onValueChanged.AddListener(SetHDR);
        antialiasingDropdown.onValueChanged.AddListener((i) => SetAA((AntiAliasing)i));
        shadowQualityDropdown.onValueChanged.AddListener((i) => SetShadowQuality((ShadowQuality)i));
        shadowDistanceDropdown.onValueChanged.AddListener((i) => SetShadowDistance((ShadowDistance)i));

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SyncUI()
    {
        qualityDropdown.SetValueWithoutNotify((int)settings.quality);
        UpdatePresetText();
        windowDropdown.SetValueWithoutNotify((int)settings.windowMode);
        volumeSlider.SetValueWithoutNotify(settings.masterVolume);
        muteToggle.SetIsOnWithoutNotify(settings.isMuted);
        languageDropdown.SetValueWithoutNotify((int)settings.language);

        aspectRatioDropdown.SetValueWithoutNotify((int)settings.aspectRatio);
        screenResolutionDropdown.SetValueWithoutNotify((int)settings.resolution);
        renderScaleSlider.SetValueWithoutNotify(settings.renderScale);
        upscalingFilter.SetValueWithoutNotify((int)settings.upscalingFilter);
        highDynamicRangeToggle.SetIsOnWithoutNotify(settings.highDynamicRange);
        antialiasingDropdown.SetValueWithoutNotify((int)settings.antiAliasing);
        shadowQualityDropdown.SetValueWithoutNotify((int)settings.shadowQuality);
        shadowDistanceDropdown.SetValueWithoutNotify((int)settings.shadowDistance);

        musicSlider.SetValueWithoutNotify(settings.musicVolume);
        sfxSlider.SetValueWithoutNotify(settings.sfxVolume);
    }

    void SetCustomQuality()
    {
        if (isLoading) 
            return;
        else
        {
            SetQuality(QualityLevel.Custom);
            qualityDropdown.SetValueWithoutNotify((int)QualityLevel.Custom);
            UpdatePresetText();
        }
    }

    #region GRAPHICS

    public void SetQuality(QualityLevel level)
    {
        settings.quality = level;
        QualitySettings.SetQualityLevel(Mathf.Clamp((int)level, 0, 3));
        PlayerPrefs.SetInt("Quality", (int)level);
        UpdatePresetText();
    }
    void UpdatePresetText()
    {
        if (presetSelected == null) return;

        presetSelected.text = settings.quality switch
        {
            QualityLevel.VeryHigh => "Very High",
            QualityLevel.High => "High",
            QualityLevel.Medium => "Medium",
            QualityLevel.Low => "Low",
            QualityLevel.Custom => "Custom",
            _ => "Unknown"
        };
    }

    public void SetAspectRatio(AspectRatio ratio)
    {
        settings.aspectRatio = ratio;
        ApplyResolution();
        PlayerPrefs.SetInt("AspectRatio", (int)ratio);
    }

    public void SetResolution(Resolution res)
    {
        settings.resolution = res;
        ApplyResolution();
        PlayerPrefs.SetInt("Resolution", (int)res);
    }

    void ApplyResolution()
    {
        Vector2Int baseResolution = settings.resolution switch
        {
            Resolution.HD => new Vector2Int(1280, 720),
            Resolution.PHD => new Vector2Int(1600, 900),
            Resolution.FHD => new Vector2Int(1920, 1080),
            Resolution.QHD => new Vector2Int(2560, 1440),
            Resolution.UHD => new Vector2Int(3840, 2160),
            _ => new Vector2Int(1920, 1080)
        };

        float aspect = settings.aspectRatio switch
        {
            AspectRatio._16x9 => 16f / 9f,
            AspectRatio._16x10 => 16f / 10f,
            AspectRatio._21x9 => 21f / 9f,
            AspectRatio._32x9 => 32f / 9f,
            _ => 16f / 9f
        };

        int width = baseResolution.x;
        int height = Mathf.RoundToInt(width / aspect);

        if (settings.windowMode == WindowMode.Window)
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
        else if (settings.windowMode == WindowMode.Borderless)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else if (settings.windowMode == WindowMode.Fullscreen)
        {
            Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        }
    }

    public void SetRenderScale(float value)
    {
        float rounded = Mathf.Round(value * 10f) / 10f;

        settings.renderScale = rounded;
        CustomRenderer.renderScale = rounded;

        renderScaleSlider.SetValueWithoutNotify(rounded);

        renderScaleLabel.text = rounded.ToString("0.0") + "x";
        SetCustomQuality();
        PlayerPrefs.SetFloat("RenderScale", rounded);
    }

    public void SetUpscaling(UpscalingFilter filter)
    {
        settings.upscalingFilter = filter;
        CustomRenderer.upscalingFilter = filter switch
        {
            UpscalingFilter.Automatic => UpscalingFilterSelection.Auto,
            UpscalingFilter.Bilinear => UpscalingFilterSelection.Linear,
            UpscalingFilter.NearestNeighbor => UpscalingFilterSelection.Point,
            UpscalingFilter.FidelityFXSuperResolution => UpscalingFilterSelection.FSR,
            UpscalingFilter.SpatialTemporalPostProcessing => UpscalingFilterSelection.STP,
            _ => UpscalingFilterSelection.Auto
        };
        SetCustomQuality();
        PlayerPrefs.SetInt("Upscaling", (int)filter);
    }

    public void SetHDR(bool enabled)
    {
        settings.highDynamicRange = enabled;
        CustomRenderer.supportsHDR = enabled;
        SetCustomQuality();
        PlayerPrefs.SetInt("HDR", enabled ? 1 : 0);
    }

    public void SetAA(AntiAliasing aa)
    {
        settings.antiAliasing = aa;

        CustomRenderer.msaaSampleCount = aa switch
        {
            AntiAliasing._8x => 8,
            AntiAliasing._4x => 4,
            AntiAliasing._2x => 2,
            AntiAliasing.Disabled => 1,
            _ => 1
        };

        SetCustomQuality();
        PlayerPrefs.SetInt("AA", (int)aa);
    }

    public void SetShadowQuality(ShadowQuality quality)
    {
        settings.shadowQuality = quality;
        CustomRenderer.mainLightShadowmapResolution = quality switch
        {
            ShadowQuality.High => 2048,
            ShadowQuality.Medium => 1024,
            ShadowQuality.Low => 512,
            _ => 1024
        };

        CustomRenderer.additionalLightsShadowmapResolution = quality switch
        {
            ShadowQuality.High => 1024,
            ShadowQuality.Medium => 512,
            ShadowQuality.Low => 256,
            _ => 512
        };
        SetCustomQuality();
        PlayerPrefs.SetInt("ShadowQuality", (int)quality);
    }

    public void SetShadowDistance(ShadowDistance dist)
    {
        settings.shadowDistance = dist;

        float distance = dist switch
        {
            ShadowDistance.VeryFar => 1000f,
            ShadowDistance.Far => 500f,
            ShadowDistance.Close => 100f,
            ShadowDistance.VeryClose => 50f,
            _ => 100f
        };

        CustomRenderer.shadowDistance = distance;
        SetCustomQuality();
        PlayerPrefs.SetInt("ShadowDistance", (int)dist);
    }

    public void SetWindowMode(WindowMode mode)
    {
        settings.windowMode = mode;

        Screen.fullScreenMode = mode switch
        {
            WindowMode.Window => FullScreenMode.Windowed,
            WindowMode.Borderless => FullScreenMode.FullScreenWindow,
            WindowMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
            _ => FullScreenMode.Windowed
        };

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
        PlayerPrefs.SetFloat("MusicVolume", volume);
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

        string localeCode = lang switch
        {
            Language.Spanish => "es-ES",
            Language.Catalan => "ca",
            Language.English => "en",
            _ => "en"
        };

        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;

        PlayerPrefs.SetInt("Language", (int)lang);
    }

    #region SAVE/LOAD

    public void LoadSettings()
    {
        settings.quality = (QualityLevel)PlayerPrefs.GetInt("Quality", 2);
        settings.windowMode = (WindowMode)PlayerPrefs.GetInt("WindowMode", 0);
        settings.masterVolume = PlayerPrefs.GetFloat("Volume", 1f);
        settings.isMuted = PlayerPrefs.GetInt("Mute", 0) == 1;
        settings.language = (Language)PlayerPrefs.GetInt("Language", 0);

        settings.aspectRatio = (AspectRatio)PlayerPrefs.GetInt("AspectRatio", 0);
        settings.resolution = (Resolution)PlayerPrefs.GetInt("Resolution", 2);
        settings.renderScale = PlayerPrefs.GetFloat("RenderScale", 1f);
        settings.upscalingFilter = (UpscalingFilter)PlayerPrefs.GetInt("Upscaling", 0);
        settings.highDynamicRange = PlayerPrefs.GetInt("HDR", 0) == 1;
        settings.antiAliasing = (AntiAliasing)PlayerPrefs.GetInt("AA", 3);
        settings.shadowQuality = (ShadowQuality)PlayerPrefs.GetInt("ShadowQuality", 1);
        settings.shadowDistance = (ShadowDistance)PlayerPrefs.GetInt("ShadowDistance", 2);

        settings.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        settings.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void ApplySettings()
    {
        
        SetWindowMode(settings.windowMode);
        ApplyResolution();
        SetRenderScale(settings.renderScale);
        SetUpscaling(settings.upscalingFilter);
        SetHDR(settings.highDynamicRange);
        SetAA(settings.antiAliasing);
        SetShadowQuality(settings.shadowQuality);
        SetShadowDistance(settings.shadowDistance);
        SetQuality(settings.quality);
        ApplyVolume();
        SetLanguage(settings.language);
    }

    public void ApplySettingsInternal()
    {
        // NO usan SetCustomQuality
        SetQuality(settings.quality);
        // Graphics
        CustomRenderer.renderScale = settings.renderScale;

        CustomRenderer.upscalingFilter = settings.upscalingFilter switch
        {
            UpscalingFilter.Automatic => UpscalingFilterSelection.Auto,
            UpscalingFilter.Bilinear => UpscalingFilterSelection.Linear,
            UpscalingFilter.NearestNeighbor => UpscalingFilterSelection.Point,
            UpscalingFilter.FidelityFXSuperResolution => UpscalingFilterSelection.FSR,
            UpscalingFilter.SpatialTemporalPostProcessing => UpscalingFilterSelection.STP,
            _ => UpscalingFilterSelection.Auto
        };

        CustomRenderer.supportsHDR = settings.highDynamicRange;

        CustomRenderer.msaaSampleCount = settings.antiAliasing switch
        {
            AntiAliasing._8x => 8,
            AntiAliasing._4x => 4,
            AntiAliasing._2x => 2,
            AntiAliasing.Disabled => 1,
            _ => 1
        };

        CustomRenderer.mainLightShadowmapResolution = settings.shadowQuality switch
        {
            ShadowQuality.High => 2048,
            ShadowQuality.Medium => 1024,
            ShadowQuality.Low => 512,
            _ => 1024
        };

        CustomRenderer.additionalLightsShadowmapResolution = settings.shadowQuality switch
        {
            ShadowQuality.High => 1024,
            ShadowQuality.Medium => 512,
            ShadowQuality.Low => 256,
            _ => 512
        };

        CustomRenderer.shadowDistance = settings.shadowDistance switch
        {
            ShadowDistance.VeryFar => 1000f,
            ShadowDistance.Far => 500f,
            ShadowDistance.Close => 100f,
            ShadowDistance.VeryClose => 50f,
            _ => 100f
        };

        // Screen
        SetWindowMode(settings.windowMode);
        ApplyResolution();

        // Audio
        ApplyVolume();

        // Language
        SetLanguage(settings.language);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    #endregion
}
