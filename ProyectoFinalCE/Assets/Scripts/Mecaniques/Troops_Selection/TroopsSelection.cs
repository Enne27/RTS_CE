using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TroopsSelection : MonoBehaviour
{
    #region Singleton
    public static TroopsSelection Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public List<Ant> unitsSelected;

    [SerializeField] private float dragThreshold = 5f;
    private Vector2 startMousePos;
    private Vector2 currentMousePos;
    private bool isDragging;


    //Input
    [SerializeField] private InputActionMap action;
    private InputAction leftClick;
    private InputAction rightClick;
    private InputAction mousePositionAction;

    [SerializeField] private RectTransform selectionArea;
    private bool isLeftMouseDown;

    private void OnEnable()
    {
        leftClick = action.FindAction("leftClick");
        leftClick.started += OnLeftClickStarted;
        leftClick.canceled += OnLeftClickCanceled;
        leftClick.Enable();

        rightClick = action.FindAction("rightClick");
        rightClick.performed += OnRightClick;
        rightClick.Enable();

        mousePositionAction = action.FindAction("mousePositionAction");
        mousePositionAction.Enable();
    }

    private void Update()
    {
        if (isLeftMouseDown)
        {
            currentMousePos = Mouse.current.position.ReadValue();

            if (!isDragging)
            {
                float dist = Vector2.Distance(currentMousePos, startMousePos);
                if (dist >= dragThreshold)
                {
                    isDragging = true;
                }
            }

            if (isDragging && selectionArea != null)
            {   
                UpdateSelectionBox(startMousePos, currentMousePos);
            }
        }
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (unitsSelected.Count < 1) return;
        Vector2 mousePos = mousePositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldMousePos = hit.point;


            foreach (Ant ant in unitsSelected)
            {
                UnitController.MoveTo(ant, worldMousePos);
            }
        }
    }

    private void OnLeftClickStarted(InputAction.CallbackContext ctx)
    {
        startMousePos = Mouse.current.position.ReadValue();
        isLeftMouseDown = true;
        isDragging = false;
        currentMousePos = startMousePos;
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    {
        isLeftMouseDown = false;

        if (isDragging)
        {
            DragSelection();
        }
        else
        {
            SingleClickSelection();
        }
        selectionArea.sizeDelta = Vector2.zero;
        isDragging = false;
    }
    
    private void UpdateSelectionBox(Vector2 start, Vector2 current)
    {
        Vector2 size = current - start;

        selectionArea.anchoredPosition = start;
        selectionArea.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        selectionArea.pivot = new Vector2(
            size.x >= 0 ? 0 : 1,
            size.y >= 0 ? 0 : 1
        );
    }

    private void DragSelection()
    {
        Vector2 min = Vector2.Min(startMousePos, currentMousePos);
        Vector2 max = Vector2.Max(startMousePos, currentMousePos);
        Rect selectionRect = new Rect(min, max - min);

        for (int i = unitsSelected.Count - 1; i >= 0; i--)
        {
            Ant baseAnt = unitsSelected[i];
            //baseAnt.transform.GetChild(0).gameObject.SetActive(false);
            unitsSelected.RemoveAt(i);
        }

        foreach (Ant baseAnt in GameManager.instance.player.ants)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(baseAnt.transform.position);
            if (screenPos.z < 0)
                continue;

            if (selectionRect.Contains(screenPos))
            {

                //baseAnt.transform.GetChild(0).gameObject.SetActive(true);
                if (!unitsSelected.Contains(baseAnt))
                    unitsSelected.Add(baseAnt);
            }
        }
    }

    private void SingleClickSelection()
    {
        Vector2 mousePos2D = mousePositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos2D);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Ant clickedAnt = hit.collider.GetComponent<Ant>();

            for (int i = unitsSelected.Count - 1; i >= 0; i--)
            {
                Ant ant = unitsSelected[i];
                //ant.transform.GetChild(0).gameObject.SetActive(false);
                unitsSelected.RemoveAt(i);
            }

            if (clickedAnt != null)
            {
                //clickedAnt.transform.GetChild(0).gameObject.SetActive(true);
                unitsSelected.Add(clickedAnt);
            }
        }
    }
}
