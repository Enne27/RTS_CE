using Unity.Cinemachine;
using UnityEngine;



public class DebugCameraController : MonoBehaviour
{
    private enum CameraState
    {
        Outside,
        Inside
    }

    [Header("Camera Control")]
    [SerializeField] private bool cameraChangeButton = false;
    [SerializeField] private CameraState cameraState = CameraState.Inside;

    [Header("Virtual Camera References")]
    [SerializeField] private GameObject OutsideCamera;
    [SerializeField] private GameObject InsideCamera;

    // Update is called once per frame
    void Update()
    {
        if (cameraChangeButton)
        {
            switch (cameraState)
            {
                case CameraState.Inside:
                    OutsideCamera.gameObject.SetActive(true);
                    InsideCamera.gameObject.SetActive(false);
                    cameraState = CameraState.Outside;
                    break;
                case CameraState.Outside:
                    InsideCamera.gameObject.SetActive(true);
                    OutsideCamera.gameObject.SetActive(false);
                    cameraState = CameraState.Inside;
                    break;
            }
        }
        cameraChangeButton = false;
    }
}
