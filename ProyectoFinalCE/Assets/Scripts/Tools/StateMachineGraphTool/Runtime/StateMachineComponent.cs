using System.Collections.Generic;
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

        /// <summary>
        /// Get a reference of the StateMachineManager instance 
        /// </summary>
        /// <returns></returns>
        public StateMachineManager GetMachineManager() => manager;


        /// <summary>
        /// Get a reference of the Context that the State Machines uses for condition check
        /// </summary>
        /// <returns></returns>
        public Context GetStateContext() => context;


        /// <summary>
        /// Sets a new context for have a data change in the context (the StateMachine needs to not be running for this function to work)
        /// </summary>
        /// <param name="context">New Context</param>
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

        /// <summary>
        /// Starts the execution flow of the State Machine
        /// </summary>
        public void StartStateMachineExecution()
        {
            context = new Context();
            manager = new StateMachineManager(controller, context, debugLog);
            execution = true;
            isStopped = false;
        }

        /// <summary>
        /// Stops the execution flow of the State Machine
        /// </summary>
        public void StopStateMachineExecution()
        {
            execution = false;
            isStopped = true;
            context = null;
            manager = null;
        }

        /// <summary>
        /// Resets to the start of the State Machine with new Context and Manager
        /// </summary>
        public void ResetStateMachineExecution()
        {
            StopStateMachineExecution();
            StartStateMachineExecution();
        }

        /// <summary>
        /// Pauses the execution of the State Machine keeping the data
        /// </summary>
        public void PauseStateMachineExecution()
        {
            execution = false;
        }

        /// <summary>
        /// UnPauses the execution of the State Machine
        /// </summary>
        public void UnPauseSateMachineExecution()
        {
            execution = true;
        }

        /// <summary>
        /// Returns the name of the current State running in this moment
        /// </summary>
        /// <returns>String</returns>
        public string GetCurrentStateName() => manager.GetCurrentStateName();

        /// <summary>
        /// Returns a list of all the actions of the current State running in this moment
        /// </summary>
        /// <returns>List of Scriptable Actions</returns>
        public List<ScriptableAction> GetCurrentStateActions() => manager.GetCurrentStateActions();

        void OnValidate()
        {
            if (manager == null) return;

            ChangeDebugLogShow();
        }
    }

#if UNITY_EDITOR
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
#endif
}
