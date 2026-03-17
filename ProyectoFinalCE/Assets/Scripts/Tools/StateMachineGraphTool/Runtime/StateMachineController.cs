using System.Collections.Generic;
using UnityEngine;
namespace StateMachine.Runtime
{
    public class StateMachineController : ScriptableObject
    {
        [SerializeReference]
        public List<StateMachineRuntimeNode> Nodes = new();
    }
}

