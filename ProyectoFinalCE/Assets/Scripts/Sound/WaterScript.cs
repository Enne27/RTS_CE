using FMODUnity;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    #region SINGLETON
    static WaterScript windScript;

    public static WaterScript instance
    {
        get
        {
            return RequestWindScript();
        }
    }

    private static WaterScript RequestWindScript()
    {
        if (windScript == null)
        {
            windScript = FindFirstObjectByType<WaterScript>();
        }
        return windScript;
    }
    #endregion

    #region VARIABLES
    [SerializeField, Tooltip("EventEmitter del agua")] StudioEventEmitter waterSound;
    const string windStrengthParameter = "WaterStrength";
    #endregion


    private void Start()
    {
        PlaySound();
    }

    public void SetStrengthValue(float newValue)
    {
        if (waterSound)
        {
            waterSound.SetParameter(windStrengthParameter, newValue);
        }
    }

    public void PlaySound()
    {
        if (waterSound)
        {
            SFXManager.PlaySFX(waterSound);
        }
    }

    public void StopSound()
    {
        if (waterSound)
        {
            SFXManager.StopSFX(waterSound);
        }
    }

    private void OnDestroy()
    {
        StopSound();
    }

}
