using System.Collections;
using UnityEngine;

public class UIEffects : MonoBehaviour
{
    #region "Singleton"
    [Header("Singleton")]
    static UIEffects uiEffects;
    public static UIEffects instance
    {
        get
        {
            return RequestUIEffects();
        }
    }

    static UIEffects RequestUIEffects()
    {
        if (uiEffects == null)
            uiEffects = FindFirstObjectByType<UIEffects>();
        return uiEffects;
    }
    #endregion

    #region FADE
    /// <summary>
    /// FadeIn del CanvasGroup.
    /// </summary>
    /// <param name="objectCG">CanvasGroup del objeto.</param>
    /// <param name="duration">Tiempo que dura el efecto Fade.</param>
    public void FadeInUIObject(CanvasGroup objectCG, float duration)
    {
        StartCoroutine(FadeInCoroutine(objectCG, duration));
    }
    /// <summary>
    /// FadeOut del CanvasGroup.
    /// </summary>
    /// <param name="objectCG"></param>
    /// <param name="duration"></param>
    public void FadeOutUIObject(CanvasGroup objectCG, float duration)
    {
        StartCoroutine(FadeOutCoroutine(objectCG, duration));
    }
    #region Coroutines
    IEnumerator FadeInCoroutine(CanvasGroup objectCG, float duration)
    {
        float elapsedTime = 0f;

        objectCG.alpha = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            objectCG.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        objectCG.alpha = 1f;
    }
    IEnumerator FadeOutCoroutine(CanvasGroup objectCG, float duration)
    {
        float elapsedTime = 0f;

        objectCG.alpha = 1f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            objectCG.alpha = 1 - Mathf.Clamp01(elapsedTime / duration);
            yield return null;

        }
        objectCG.alpha = 0f;
    }
    #endregion
    #endregion


    #region SOUND

    #endregion


    #region SCALE

    #endregion
}
