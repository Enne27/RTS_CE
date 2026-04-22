using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingData data;

    private BuildingModel model;
    private bool isHovered = false;

    private CameraMovement2D cameraMovement;
    public TextMeshProUGUI descriptionTextBlock; 

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    private void Awake()
    {
        cameraMovement = FindFirstObjectByType<CameraMovement2D>();
    }

    public void Setup(BuildingData data, float rotation)
    {
        this.data = data;
        model = Instantiate(data.buildModel, transform.position, Quaternion.identity, transform);
        model.Rotate(rotation);
        descriptionTextBlock = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Mouse.current == null) return;

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
        cameraMovement.ZoomOnBuilding(transform);

        switch (data.buildingType)
        {
            case BuildingType.QueenChamber:
                break;
            case BuildingType.BroodChamber:
                ViewManager.Show<BroodChamberView>();
                ViewManager.GetView<GameHUDView>().Show();
                break;
            case BuildingType.StorageChamber:
                break;
            default:
                break;
        }
    }

}