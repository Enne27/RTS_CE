using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingData data;

    [SerializeField] private BuildingModel model;

    private bool isHovered = false;

    private CameraMovement2D cameraMovement;
    public TextMeshProUGUI descriptionTextBlock;
    [SerializeField] public CameraProjection cameraMinimap;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    public string buildingID;

    private void Awake()
    {
        cameraMovement = FindFirstObjectByType<CameraMovement2D>();
    }

    public void Setup(BuildingData data, float rotation)
    {
        this.data = data;

        buildingID = data.name;

        model = Instantiate(data.buildModel, transform.position, Quaternion.identity, transform);
        model.Rotate(rotation);
        descriptionTextBlock = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        if (Time.timeScale == 0) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        bool hitThis = false;

        if (Physics.Raycast(ray, out hit))
            if (hit.transform.IsChildOf(transform))
                hitThis = true;

        if (hitThis && !isHovered)
        {
            isHovered = true;
            model.ChangeModelOutlineColor(Color.yellow);
            if(descriptionTextBlock != null)
                descriptionTextBlock.text = data.buildDescription.GetLocalizedString();
        }
        else if (!hitThis && isHovered)
        {
            isHovered = false;
            model.ChangeModelOutlineColor(Color.black);
            if (descriptionTextBlock != null)
                descriptionTextBlock.text = string.Empty;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray clickRay = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(clickRay, out RaycastHit clickHit))
                if (clickHit.transform.IsChildOf(transform))
                {
                    if (Time.time - lastClickTime <= doubleClickThreshold)
                        OnDoubleClick();
                    lastClickTime = Time.time;
                }
        }
    }

    private void OnDoubleClick()
    {
        Debug.Log("Double click en building");


        switch (data.buildingType)
        {
            case BuildingType.QueenChamber:
                cameraMovement.ZoomOnBuilding(transform);
                break;
            case BuildingType.BroodChamber:
                cameraMovement.ZoomOnBuilding(transform);
                ViewManager.Show<BroodChamberView>();
                ViewManager.GetView<GameHUDView>().Show();
                break;
            case BuildingType.StorageChamber:
                cameraMovement.ZoomOnBuilding(transform);
                break;
            case BuildingType.Entrance:
                CameraController.instance.ChangeCameraMode(CameraState.Outside);
                BuildingManager.Instance.CancelPreview();
                StartCoroutine(ActivarMinimap());
                break;
            case BuildingType.Mound:
                CameraController.instance.ChangeCameraMode(CameraState.Inside);
                if (cameraMinimap != null) cameraMinimap.SetRenderingEnabled(false);
                break;
            default:
                break;
        }
    }
    IEnumerator ActivarMinimap()
    {
        yield return new WaitForSeconds(2f);

        if (cameraMinimap != null)
            cameraMinimap.SetRenderingEnabled(true);
    }
    
}