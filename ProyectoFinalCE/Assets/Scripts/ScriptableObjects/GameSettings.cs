using FMOD.Studio;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Screen")]
    public QualityLevel quality;
    public WindowMode windowMode;

    [Header("Audio")]
    public float masterVolume = 1f;
    public bool isMuted = false;
    [SerializeField, Range(0, 1)] public float musicVolume;
    [SerializeField, Range(0, 1)] public float sfxVolume;
    public VCA musicVCA;
    public VCA sfxVCA;


    [Header("Language")]
    public Language language;
}