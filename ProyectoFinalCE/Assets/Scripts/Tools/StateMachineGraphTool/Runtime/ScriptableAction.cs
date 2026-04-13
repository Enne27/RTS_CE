using UnityEngine;
namespace StateMachine.Runtime
{
    [CreateAssetMenu(fileName = "New ScriptableAction", menuName = "Tools/IA/New ScriptableAction")]
    public class ScriptableAction : ScriptableObject
    {
        [Header("General Settings")]
        public string actionName;
        public string actionDescription;
    }
}

