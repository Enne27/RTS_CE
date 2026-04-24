using FMOD.Studio;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Screen")]
    public QualityLevel quality;
    public WindowMode windowMode;
    public AspectRatio aspectRatio;
    public Resolution resolution;

    [Header("Custom Quality")]
    [SerializeField, Range(0.5f,3f)] public float renderScale;
    public UpscalingFilter upscalingFilter;
    public bool highDynamicRange;
    public AntiAliasing antiAliasing;
    public ShadowQuality shadowQuality;
    public ShadowDistance shadowDistance;
    


    [Header("Audio")]
    [SerializeField, Range(0, 1)] public float masterVolume;
    public bool isMuted = false;
    [SerializeField, Range(0, 1)] public float musicVolume;
    [SerializeField, Range(0, 1)] public float sfxVolume;
    public VCA musicVCA;
    public VCA sfxVCA;

    [Header("Language")]
    public Language language;
}