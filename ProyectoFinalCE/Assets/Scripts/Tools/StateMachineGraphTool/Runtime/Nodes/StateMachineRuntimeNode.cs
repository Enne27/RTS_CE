using System.Collections.Generic;
using System;
namespace StateMachine.Runtime
{
    public enum ConectionType
    {
        Condition,
        And,
        Or
    }

    [Serializable]
    public abstract class StateMachineRuntimeNode
    {
        public List<int> NextNodeIndices = new();
    }

    [Serializable]
    public class StartRuntimeNode : StateMachineRuntimeNode { }

    [Serializable]
    public class StateRuntimeNode : StateMachineRuntimeNode
    {
        public string StateName;
        public List<ScriptableAction> actions;
    }

    [Serializable]
    public class IfRuntimeNode : StateMachineRuntimeNode
    {
        public bool Condition;

        public ConditionRuntimeNode ConditionRuntimeNode;
        public AndRuntimeNode AndRuntimeNode;
        public OrRuntimeNode OrRuntimeNode;

        public bool hasTrueOutput;
        public bool hasFalseOutput;

        public ConectionType ConectionType;
        
    }

    [Serializable]
    public class AndRuntimeNode : StateMachineRuntimeNode
    {
        public List<BattleCondition> Conditions;
    }

    [Serializable]
    public class OrRuntimeNode : StateMachineRuntimeNode
    {
        public List<BattleCondition> Conditions;
    }

    [Serializable]
    public class ConditionRuntimeNode : StateMachineRuntimeNode
    {
        public BattleCondition BattleCondition;
    }
}


