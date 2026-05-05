using Unity.Cinemachine;
using UnityEngine;


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

    public void ChangeCameraMode()
    {
        switch (cameraState)
        {
            case CameraState.Inside:
                //OutsideCamera.SetActive(true);
                OutsideCamera.GetComponent<CameraMovement>().EnableCameraInput();
                InsideCamera.SetActive(false);
                cameraState = CameraState.Outside;
                break;
            case CameraState.Outside:
                InsideCamera.SetActive(true);
                OutsideCamera.GetComponent<CameraMovement>().DisableCameraInput();
                //OutsideCamera.SetActive(false);
                cameraState = CameraState.Inside;
                break;
        }
    }

    public void ChangeCameraMode(CameraState changeCameraState)
    {
        switch (changeCameraState)
        {
            case CameraState.Outside:
                //OutsideCamera.SetActive(true);
                InsideCamera.SetActive(false);
                cameraState = CameraState.Outside;
                break;
            case CameraState.Inside:
                InsideCamera.SetActive(true);
                //OutsideCamera.SetActive(false);
                cameraState = CameraState.Inside;
                break;
        }
    }
}
