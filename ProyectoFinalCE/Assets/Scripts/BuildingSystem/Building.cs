using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] public BuildingData data;
    
    [SerializeField] private BuildingModel model;

    private bool isHovered = false;

    [Header("Cameras")]
    private CameraMovement2D cameraMovement;
    [SerializeField] public CameraProjection cameraMinimap;

    [Header("Preview desc")]
    [SerializeField] public Image backgroundImage;
    [SerializeField] public TextMeshProUGUI descriptionTextBlock;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    public string buildingID;
    #endregion

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
        backgroundImage = GetComponentInChildren<Image>();
        backgroundImage.enabled=false;


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
                descriptionTextBlock.text = data.buildName.GetLocalizedString() + "\n" + data.buildDescription.GetLocalizedString();
            if(backgroundImage != null) 
                backgroundImage.enabled = true;
        }
        else if (!hitThis && isHovered)
        {
            isHovered = false;
            model.ChangeModelOutlineColor(Color.black);
            if (descriptionTextBlock != null)
                descriptionTextBlock.text = string.Empty;
            if (backgroundImage != null)
                backgroundImage.enabled = false;
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

        GameHUDView hud = ViewManager.GetView<GameHUDView>();

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
                hud.constructionButton.gameObject.SetActive(false);
                BuildingManager.Instance.CancelPreview();
                StartCoroutine(ActivarMinimap());
                break;
            case BuildingType.Mound:
                CameraController.instance.ChangeCameraMode(CameraState.Inside, ()=> hud.constructionButton.gameObject.SetActive(true));
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