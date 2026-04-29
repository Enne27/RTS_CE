using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    public void FadeInUIObject(CanvasGroup objectCG, float duration, System.Action onComplete = null)
    {
        // StartCoroutine(FadeInCoroutine(objectCG, duration, onComplete));
        FadeUI(objectCG, 1f, duration, onComplete);
    }
    /// <summary>
    /// FadeOut del CanvasGroup.
    /// </summary>
    /// <param name="objectCG"></param>
    /// <param name="duration"></param>
    public void FadeOutUIObject(CanvasGroup objectCG, float duration, System.Action onComplete = null)
    {
        // StartCoroutine(FadeOutCoroutine(objectCG, duration, onComplete));
        FadeUI(objectCG, 0f, duration, onComplete);
    }

    /// <summary>
    /// Internamente hace los fades dependiendo de la intenci�n.
    /// </summary>
    /// <param name="objectCG">Canvas group del objeto a Fade.</param>
    /// <param name="targetAlpha">Alfa deseado para el cg</param>
    /// <param name="duration">Duraci�n del fade</param>
    /// <param name="onComplete">Evento que sucede al completar el efecto.</param>
    private void FadeUI(CanvasGroup objectCG, float targetAlpha, float duration, System.Action onComplete = null)
    {
        LeanTween.cancel(objectCG.gameObject);

        // Que solo sea interactuable mientras es medio visible. (El n�mero es decisi�n de dise�o.)
        objectCG.interactable = targetAlpha > 0.5f;
        objectCG.blocksRaycasts = targetAlpha > 0.5f;

        LeanTween.value(objectCG.gameObject, objectCG.alpha, targetAlpha, duration)
            .setOnUpdate((float val) => objectCG.alpha = val)
            .setEase(LeanTweenType.linear)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                objectCG.interactable = targetAlpha > 0.5f;
                objectCG.blocksRaycasts = targetAlpha > 0.5f;
                onComplete?.Invoke();
            });
    }
    #region Coroutines
    /*IEnumerator FadeInCoroutine(CanvasGroup objectCG, float duration, System.Action onComplete = null)
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
        onComplete?.Invoke();
    }
    IEnumerator FadeOutCoroutine(CanvasGroup objectCG, float duration, System.Action onComplete = null)
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
        onComplete?.Invoke();
    }*/
    #endregion
    #endregion


    #region SOUND

    #endregion

    #region MOVEMENTS
    public void SlideUI(RectTransform rect, Vector2 targetPosition, float duration, System.Action onComplete = null)
    {
        LeanTween.cancel(rect.gameObject);

        LeanTween.value(rect.gameObject, rect.anchoredPosition, targetPosition, duration)
            .setOnUpdate((Vector2 val) => rect.anchoredPosition = val)
            .setEase(LeanTweenType.easeOutCubic)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => onComplete?.Invoke());
    }


    #endregion

    #region SCALE
    /// <summary>
    /// Incrementa la escala del objeto usando un multiplicador y su escala original.
    /// </summary>
    public void IncreaseScale(GameObject gameObject, Vector3 originalScale, float scaleFactor, float duration)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Devuelve la escala del objeto a su tama�o original.
    /// </summary>
    public void RestartScale(GameObject gameObject, Vector3 originalScale, float duration)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale, duration)
            .setIgnoreTimeScale(true)
            .setEase(LeanTweenType.easeInOutQuad);
    }
    #endregion
}