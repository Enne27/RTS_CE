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
    static public HashSet<BaseAnt> activeAnts; //TODO: make singleton instead of static public var
    private List<BaseAnt> antsToRemove = new List<BaseAnt>();


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
        activeAnts = new HashSet<BaseAnt>();
    }

    private void FixedUpdate()
    {
        //If no units in are using a flow field. Flow field should be destroyed or reused in a pool
        //If unit end his work should be removed from active ants
        Debug.Log("Active ants: " + activeAnts.Count);  
        Debug.Log("Active flowFields: " + FlowField_Manager.Instance.flowFields.Count);


        #region Ideas
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

        //foreach (BaseAnt baseAnt in activeAnts) //GameObject Should be of type (Troop/unit/baseAnt/ant)
        //                                                //activeUnitsInGame tries to avoid empty iterations where the troop has no instruction
        //                                                //but maybe is worse baceuse has to exist another array/list with units added and removed each time thay get or lose instructuins
        //{
        //    //Each ant should have a flowField index refearing to a flow field array
        //    //in this case shouldn't be needed the invalid index for no flow field
        //    //if (troop.flowFieldIndex == 0) continue;


        //}

        #endregion

        antsToRemove.Clear();

        foreach (BaseAnt ant in activeAnts)
        {

            //Cell cellBelow = FlowField_Manager.Instance
            //.flowFields[ant.flowFieldInxex]
            //.GetCellFromWorldPos(ant.transform.position);

            //Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            //ant.transform.position += moveDirection * moveSpeed * Time.deltaTime;

            
            Cell cellBelow = FlowField_Manager.Instance
                .flowFields[ant.flowFieldInxex]
                .GetCellFromWorldPos(ant.transform.position);

            // Dirección hacia la celda
            Vector3 desiredDirection = new Vector3(
                cellBelow.bestDirection.Vector.x,
                0,
                cellBelow.bestDirection.Vector.y
            ).normalized;

            // Si ya está en destino
            //TODO: Quitar hormiga si esta bloqueada por otra en el destino por separacion.
            if (desiredDirection == Vector3.zero)
            {
                Debug.Log("Hormiga ha llegado a su destino");
                antsToRemove.Add(ant);
                continue;
            }

            // --- STEERING ---
            float maxSpeed = moveSpeed;
            float steeringStrength = 5f;

            Vector3 currentVelocity = ant.currentVelocity;
            Vector3 desiredVelocity = desiredDirection * maxSpeed;
            Vector3 steering = (desiredVelocity - currentVelocity) * steeringStrength;
            currentVelocity += steering * Time.deltaTime;
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
            ant.currentVelocity = currentVelocity;
            ant.transform.position += currentVelocity * Time.deltaTime;
            if (currentVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
                ant.transform.rotation = Quaternion.Slerp(
                    ant.transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }
        }

        // Remover ants
        
        foreach (BaseAnt ant in antsToRemove)
        {
            activeAnts.Remove(ant);
        }

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