using UnityEngine;
namespace StateMachine.Runtime
{
    public abstract class ScriptableAction : ScriptableObject
    {
        [Header("General Settings")]
        public string actionName;
        [TextArea(2,5)]
        public string actionDescription;

        public abstract void Execute(StateMachineManager manager);
    }
}