using UnityEditor;
using UnityEngine;

namespace StateMachine.Runtime
{
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
        [Header("Execution Flow")]
        [SerializeField]
        private bool AutomaticStart;

        [HideInInspector]
        public bool execution;
        [HideInInspector]
        public bool isStopped;

        void Start()
        {
            if (AutomaticStart)
                StartStateMachineExecution();
            else
                isStopped = true;
        }

        void FixedUpdate()
        {
            if (execution)
            {
                manager.StateExecutor();
                manager.ExecuteActionsBehaviour();
            }
        }

        public StateMachineManager GetMachineManager() => manager;

        public Context GetStateContext() => context;

        public void SetStateContext(Context context)
        {
            if (isStopped)
                this.context = context;
            else
                Debug.LogWarning("Can't change context while the State Machine Context is running. Use StopStateMachineExecution before this function.");
        }

        private void ChangeDebugLogShow()
        {
            if (debugLog)
                manager.ActivateStateMachineLog();
            else
                manager.DeactivateStateMachineLog();
        }

        public void StartStateMachineExecution()
        {
            context = new Context();
            manager = new StateMachineManager(controller, context, debugLog);
            execution = true;
            isStopped = false;
        }

        public void StopStateMachineExecution()
        {
            execution = false;
            isStopped = true;
            context = null;
            manager = null;
        }

        public void ResetStateMachineExecution()
        {
            StopStateMachineExecution();
            StartStateMachineExecution();
        }

        public void PauseStateMachineExecution()
        {
            execution = false;
        }

        public void UnPauseSateMachineExecution()
        {
            execution = true;
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
}
