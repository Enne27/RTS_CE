using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public enum CameraState
{
    Outside,
    Inside
}

public class CameraController : MonoBehaviour
{
    static CameraController cameraController;

    public static CameraController instance
    {
        get
        {
            return FindOrGetPauseController();
        }
    }

    static CameraController FindOrGetPauseController()
    {
        if (cameraController == null)
            cameraController = FindFirstObjectByType<CameraController>();

        return cameraController;
    }

    [Header("Camera Control")]
    [SerializeField] private CameraState cameraState;

    [Header("Virtual Camera References")]
    [SerializeField] private GameObject OutsideCamera;
    [SerializeField] private GameObject InsideCamera;

    [Header("Video Player References")]
    [SerializeField] private VideoPlayer entranceVideoPlayer;
    [SerializeField] private VideoClip enterVideo;
    [SerializeField] private VideoClip exitVideo;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;

    private Coroutine transitionCoroutine;

    public void ChangeCameraMode()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(ChangeCameraRoutine());
    }

    private IEnumerator ChangeCameraRoutine()
    {
        switch (cameraState)
        {
            case CameraState.Outside:
                entranceVideoPlayer.clip = enterVideo;
                break;
            case CameraState.Inside:
                entranceVideoPlayer.clip = exitVideo;
                break;
        }
        // ---------- FADE IN ----------
        entranceVideoPlayer.targetCameraAlpha = 0f;
        entranceVideoPlayer.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            entranceVideoPlayer.targetCameraAlpha =
                Mathf.Lerp(0f, 1f, time / fadeDuration);

            yield return null;
        }

        entranceVideoPlayer.targetCameraAlpha = 1f;

        // ---------- CAMBIO DE CÁMARA ----------
        switch (cameraState)
        {
            case CameraState.Inside:

                OutsideCamera.GetComponent<CameraMovement>().EnableCameraInput();

                InsideCamera.SetActive(false);

                cameraState = CameraState.Outside;
                break;

            case CameraState.Outside:

                InsideCamera.SetActive(true);

                OutsideCamera.GetComponent<CameraMovement>().DisableCameraInput();

                cameraState = CameraState.Inside;
                break;
        }

        // Espera a que termine el blend de Cinemachine
        yield return StartCoroutine(WaitBlend());

        // ---------- ESPERA A QUE TERMINE EL VIDEO ----------
        while (entranceVideoPlayer.isPlaying)
            yield return null;

        // ---------- FADE OUT ----------
        time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            entranceVideoPlayer.targetCameraAlpha =
                Mathf.Lerp(1f, 0f, time / fadeDuration);

            yield return null;
        }

        entranceVideoPlayer.targetCameraAlpha = 0f;
    }

    public void ChangeCameraMode(CameraState changeCameraState, System.Action onComplete = null)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(ChangeCameraRoutine(changeCameraState, onComplete));
    }

    private IEnumerator ChangeCameraRoutine(CameraState changeCameraState, System.Action onComplete)
    {
        switch (cameraState)
        {
            case CameraState.Outside:
                entranceVideoPlayer.clip = enterVideo;
                break;
            case CameraState.Inside:
                entranceVideoPlayer.clip = exitVideo;
                break;
        }

        // ---------- FADE IN ----------
        entranceVideoPlayer.targetCameraAlpha = 0f;
        entranceVideoPlayer.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            entranceVideoPlayer.targetCameraAlpha =
                Mathf.Lerp(0f, 1f, time / fadeDuration);

            yield return null;
        }

        entranceVideoPlayer.targetCameraAlpha = 1f;

        // ---------- CAMBIO DE CÁMARA ----------
        switch (changeCameraState)
        {
            case CameraState.Outside:

                InsideCamera.SetActive(false);
                cameraState = CameraState.Outside;
                break;

            case CameraState.Inside:

                InsideCamera.SetActive(true);
                cameraState = CameraState.Inside;
                break;
        }

        yield return StartCoroutine(WaitBlend());

        onComplete?.Invoke();

        // ---------- ESPERA A QUE TERMINE EL VIDEO ----------
        while (entranceVideoPlayer.isPlaying)
            yield return null;

        // ---------- FADE OUT ----------
        time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            entranceVideoPlayer.targetCameraAlpha =
                Mathf.Lerp(1f, 0f, time / fadeDuration);

            yield return null;
        }

        entranceVideoPlayer.targetCameraAlpha = 0f;
    }

    private IEnumerator WaitBlend()
    {
        var brain = Camera.main.GetComponent<CinemachineBrain>();

        yield return null;

        while (brain.IsBlending)
            yield return null;

        yield return null;
    }
}