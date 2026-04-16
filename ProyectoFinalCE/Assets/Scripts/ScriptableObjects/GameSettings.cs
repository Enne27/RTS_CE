using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public QualityLevel quality;
    public WindowMode windowMode;
    public float masterVolume = 1f;
    public bool isMuted = false;
    public Language language;
}