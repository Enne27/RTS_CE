using FMODUnity;
using UnityEngine;

public class MusicVolumeParams : MonoBehaviour
{
    #region SOUND_params
    private StudioEventEmitter musicEmitter;
    private const string musicParamName = "Speaking";
    [SerializeField] private float musicVolumeValueMin;
    [SerializeField] private float musicVolumeValueMax;

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
            musicVolumeParamsScript = FindObjectOfType<MusicVolumeParams>();
        }
        return musicVolumeParamsScript;
    }
    #endregion

    private void Start()
    {
        musicEmitter = MusicManager.instance.GetEventEmitter();
    }

    public void ChangeMusicToSpeaking()
    {

        musicEmitter = MusicManager.instance.GetEventEmitter();
        MusicManager.instance.ChangeParameterValue(musicEmitter, musicParamName, musicVolumeValueMin);
    }

    public void ChangeMusicToNotSpeaking()
    {
        MusicManager.instance.ChangeParameterValue(musicEmitter, musicParamName, musicVolumeValueMax);
    }
}
