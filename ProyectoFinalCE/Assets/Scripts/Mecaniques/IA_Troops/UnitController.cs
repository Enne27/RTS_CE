using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    public FlowField_Manager flowField_Test;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;

    private List<GameObject> unitsInGame;
    [SerializeField]private List<GameObject> unitsSelected;

    public float dragThreshold = 5f;

    private bool isDragging;
    [SerializeField]private Vector2 startMousePos;
    [SerializeField]private Vector2 currentMousePos;

    //Input
    public InputActionMap action;
    private InputAction spawnUnits;
    private InputAction destroyUnits;
    private InputAction leftClick;
    private InputAction rightClick;
    private InputAction mousePositionAction;

    public RectTransform selectionArea;
    private bool isLeftMouseDown;


    private void OnEnable()
    {
        spawnUnits = action.FindAction("spawnUnits");
        spawnUnits.performed += SpawnUnits;
        spawnUnits.Enable();

        destroyUnits = action.FindAction("destroyUnits");
        destroyUnits.performed += DestroyUnits;
        destroyUnits.Enable();

        leftClick = action.FindAction("leftClick");
        leftClick.started += OnLeftClickStarted;
        leftClick.canceled += OnLeftClickCanceled;
        leftClick.Enable();

        rightClick = action.FindAction("rightClick");
        rightClick.performed += OnRightClick;
        rightClick.Enable();
    }
    private void Awake()
    {
        unitsInGame = new List<GameObject>();
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

        if (rightClick.WasPressedThisFrame())
        {
            Vector2 mousePos2D = mousePositionAction.ReadValue<Vector2>();
            Vector3 mousePos = new Vector3(mousePos2D.x, mousePos2D.y, 10f);
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            FlowField_Manager.Instance.InitializeFlowField(worldMousePos);
        }
    }

    private void FixedUpdate()
    {

        //foreach (FlowField flowField in FlowField_Manager.Instance.flowFields)
        //{
        //    foreach ()
        //    {

        //    }
        //}
        //if (flowField_Test.curFlowField == null) { return; }
        //foreach (GameObject unit in unitsInGame)
        //{
        //    Cell cellBelow = flowField_Test.curFlowField.GetCellFromWorldPos(unit.transform.position);
        //    Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
        //    Rigidbody unitRB = unit.GetComponent<Rigidbody>();
        //    unitRB.linearVelocity = moveDirection * moveSpeed;
        //}
    }

    private void SpawnUnits(InputAction.CallbackContext context)
    {
        Debug.Log("Spawn");
        Vector2Int gridSize = flowField_Test.gridSize;
        float nodeRadius = flowField_Test.cellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        Vector3 newPos;
        for (int i = 0; i < numUnitsPerSpawn; i++)
        {
            GameObject newUnit = Instantiate(unitPrefab);
            newUnit.transform.parent = transform;
            unitsInGame.Add(newUnit);

            newPos = new Vector3(Random.Range(-maxSpawnPos.x / 2, maxSpawnPos.x / 2), 0, Random.Range(-maxSpawnPos.y / 2, maxSpawnPos.y / 2));
            newUnit.transform.position = newPos;
        }
    }

    private void DestroyUnits(InputAction.CallbackContext context)
    {
        foreach (GameObject go in unitsInGame)
        {
            Destroy(go);
        }
        unitsInGame.Clear();
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void OnLeftClickStarted(InputAction.CallbackContext ctx)
    {
        Debug.Log("Started");
        startMousePos = Mouse.current.position.ReadValue();
        isLeftMouseDown = true;
        isDragging = false;
        currentMousePos = startMousePos;
    }

    void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    {
        isLeftMouseDown = false;

        if (isDragging)
        {
            DragSelection();
        }
        else
        {
            // Single click select
            //PerformSingleClick();
        }
        selectionArea.sizeDelta = Vector2.zero;
        isDragging = false;
    }
    private void UpdateSelectionBox(Vector2 start, Vector2 current)
    {
        Vector2 size = current - start;

        selectionArea.anchoredPosition = start;
        selectionArea.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        // Fix direction (so it grows correctly)
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
            GameObject go = unitsSelected[i];
            go.transform.GetChild(0).gameObject.SetActive(false);
            unitsSelected.RemoveAt(i);
        }

        foreach (GameObject go in unitsInGame)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(go.transform.position);
            if (screenPos.z < 0)
                continue;

            if (selectionRect.Contains(screenPos))
            {

                go.transform.GetChild(0).gameObject.SetActive(true);
                if (!unitsSelected.Contains(go))
                    unitsSelected.Add(go);
            }
        }
    }
    private void SingleClickSelection()
    {

    }
} 