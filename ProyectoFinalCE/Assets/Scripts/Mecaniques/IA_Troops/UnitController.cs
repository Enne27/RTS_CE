using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    public FlowField_Test flowField_Test;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;

    private List<GameObject> unitsInGame;

    //Input
    public InputActionMap action;
    private InputAction spawnUnits;
    private InputAction destroyUnits;


    private void OnEnable()
    {
        spawnUnits = action.FindAction("spawnUnits");
        spawnUnits.Enable();
        destroyUnits = action.FindAction("destroyUnits");
        destroyUnits.Enable();
    }

    private void OnDisable()
    {
        spawnUnits.Disable();
        destroyUnits.Disable();
    }

    private void Awake()
    {
        unitsInGame = new List<GameObject>();
    }

    void Update()
    {
        if (spawnUnits.WasPressedThisFrame()) SpawnUnits();

        if (destroyUnits.WasPressedThisFrame()) DestroyUnits();
    }

    private void FixedUpdate()
    {
        if (flowField_Test.curFlowField == null) { return; }
        foreach (GameObject unit in unitsInGame)
        {
            Cell cellBelow = flowField_Test.curFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            Rigidbody unitRB = unit.GetComponent<Rigidbody>();
            unitRB.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void SpawnUnits()
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

            newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
            newUnit.transform.position = newPos;
        }
    }

    private void DestroyUnits()
    {
        foreach (GameObject go in unitsInGame)
        {
            Destroy(go);
        }
        unitsInGame.Clear();
    }
}