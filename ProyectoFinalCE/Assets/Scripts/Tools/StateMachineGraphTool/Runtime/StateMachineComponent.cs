using StateMachine.Runtime;
using UnityEditor;
using UnityEngine;

//This is just a base component with basic behaviour for the state machine you can do yourself a similar custom script using this as base
[AddComponentMenu("State Machine Component")]
public class StateMachineComponent : MonoBehaviour
{
    [Tooltip("Give this component to the gameObject you want to give a State Machine Behaviour")]
    private StateMachineManager manager;
    private Context context;

    [Header("State Machine Controller")]
    [SerializeField, Tooltip("This is the state machine graph you have to make and this script will read the infotmation from it to controll the behaviour made")] 
    private StateMachineController controller;
    [SerializeField, Tooltip("This will show debug information in console from the state machine manager")]
    private bool debugLog;

    void Start()
    {
        manager = new StateMachineManager(controller, context, debugLog);
    }

    void FixedUpdate()
    {
        manager.StateExecutor();
        manager.ExecuteActionsBehaviour();
    }

    public StateMachineManager GetMachineManager() => manager;

    public Context GetStateContext() => context;

    private void ChangeDebugLogShow()
    {
        if(debugLog) 
            manager.ActivateStateMachineLog();
        else 
            manager.DeactivateStateMachineLog();
    }

    void OnValidate()
    {
        if (manager == null) return;

        ChangeDebugLogShow();
    }
}

public class GenerateStateMachineGameObject
{
    [MenuItem("GameObject/State Machine GameObject", false, 10)]
    static void CrearObjeto()
    {
        GameObject go = new GameObject("State Machine GameObject");
        go.AddComponent<StateMachineComponent>();
        Selection.activeGameObject = go;
    }
}