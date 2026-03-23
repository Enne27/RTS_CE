using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : MonoBehaviour
{
    public FlowField_Manager flowField_Editor;
    public BaseAnt unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;

    static public List<BaseAnt> antsInGame; //TODO: make singleton instead of static public var
    static public List<BaseAnt> activeAnts; //TODO: make singleton instead of static public var


    //Input
    public InputActionMap action;
    private InputAction spawnUnits;
    private InputAction destroyUnits;

    private void OnEnable()
    {
        spawnUnits = action.FindAction("spawnUnits");
        spawnUnits.performed += SpawnUnits;
        spawnUnits.Enable();

        destroyUnits = action.FindAction("destroyUnits");
        destroyUnits.performed += DestroyUnits;
        destroyUnits.Enable();
    }
    private void Awake()
    {
        antsInGame = new List<BaseAnt>();
        activeAnts = new List<BaseAnt>();
    }

    private void FixedUpdate()
    {
        Debug.Log("Active ants: " + activeAnts.Count);

        //foreach (FlowField flowField in FlowField_Manager.Instance.flowFields)
        //{
        //    foreach (Troop  troop in flowField)
        //    {
        //        Cell cellBelow = flowField.GetCellFromWorldPos(troop.transform.position);
        //        Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
        //        Rigidbody troopRB = troop.GetComponent<Rigidbody>();
        //        troopRB.linearVelocity = moveDirection * moveSpeed;
        //    }
        //}

        //foreach (GameObject troop in unitsInGame) //GameObject Should be of type (Troop/unit/baseAnt/ant)
        //{
        //    //Each ant should have a flowField index refearing to a flow field array
        //    //it should exist a invalid flow field index in case this ant/unit has no instruction (0 in this example)
        //    if (troop.flowFieldIndex == 0) continue;

        //    Cell cellBelow = flowFieldArray[troop.flowFieldIndex].GetCellFromWorldPos(troop.transform.position);
        //    Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
        //    Rigidbody troopRB = troop.GetComponent<Rigidbody>();
        //    troopRB.linearVelocity = moveDirection * moveSpeed;
        //}


        for (int i = 0; i < activeAnts.Count; i++)
        {
            Cell cellBelow = FlowField_Manager.Instance.flowFields[activeAnts[i].flowFieldInxex].GetCellFromWorldPos(activeAnts[i].transform.position);
            if (cellBelow.worldPos == activeAnts[i].transform.position)
            {
                activeAnts.RemoveAt(i);
                continue;
            }
            Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            Rigidbody troopRB = activeAnts[i].GetComponent<Rigidbody>();
            troopRB.linearVelocity = moveDirection * moveSpeed;
        }
        //foreach (BaseAnt baseAnt in activeAnts) //GameObject Should be of type (Troop/unit/baseAnt/ant)
        //                                                //activeUnitsInGame tries to avoid empty iterations where the troop has no instruction
        //                                                //but maybe is worse baceuse has to exist another array/list with units added and removed each time thay get or lose instructuins
        //{
        //    //Each ant should have a flowField index refearing to a flow field array
        //    //in this case shouldn't be needed the invalid index for no flow field
        //    //if (troop.flowFieldIndex == 0) continue;


        //}
    }

    private void SpawnUnits(InputAction.CallbackContext context)
    {
        Debug.Log("Spawn");
        Vector2Int gridSize = flowField_Editor.gridSize;
        float nodeRadius = flowField_Editor.cellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        Vector3 newPos;
        for (int i = 0; i < numUnitsPerSpawn; i++)
        {
            BaseAnt newUnit = Instantiate(unitPrefab);
            newUnit.transform.parent = transform;
            antsInGame.Add(newUnit);

            newPos = new Vector3(Random.Range(-maxSpawnPos.x / 2, maxSpawnPos.x / 2), 0, Random.Range(-maxSpawnPos.y / 2, maxSpawnPos.y / 2));
            newUnit.transform.position = newPos;
        }
    }

    private void DestroyUnits(InputAction.CallbackContext context)
    {
        foreach (BaseAnt baseAnt in antsInGame)
        {
            Destroy(baseAnt);
        }
        antsInGame.Clear();
    }
} 