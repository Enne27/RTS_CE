using FMODUnity;
using UnityEngine;

public class MusicVolumeParams : MonoBehaviour
{
    #region SOUND_params
    private StudioEventEmitter musicEmitter;
    private const string musicParamName = "Speaking";
    [SerializeField] private float musicVolumeValueMin;
    [SerializeField] private float musicVolumeValueMax;
    private float currentValue = 1;

    #endregion

    #region SINGLETON
    static MusicVolumeParams musicVolumeParamsScript;

    public static MusicVolumeParams instance
    {
        get
        {
            return RequestWindScript();
        }
    }

    private static MusicVolumeParams RequestWindScript()
    {
        if (musicVolumeParamsScript == null)
        {
            musicVolumeParamsScript = FindFirstObjectByType<MusicVolumeParams>();
        }
        return musicVolumeParamsScript;
    }
    #endregion


    public void ChangeMusicToSpeaking()
    {
        currentValue = musicVolumeValueMin;
        ApplyCurrentState();
    }

    public void ChangeMusicToNotSpeaking()
    {
        currentValue = musicVolumeValueMax;
        ApplyCurrentState();
    }

    public void ApplyCurrentState()
    {
        var emitter = MusicManager.instance.GetEventEmitter();

        if (emitter != null && emitter.EventInstance.isValid())
            emitter.EventInstance.setParameterByName(musicParamName, currentValue);
    }
}
