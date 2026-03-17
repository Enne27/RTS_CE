using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Runtime
{
    public interface IStateMachineNodeExecutor<in TNode> where TNode : StateMachineRuntimeNode
    {
        bool Execute(TNode node, StateMachineManager ctx);
    }

    public class StartNodeExecutor : IStateMachineNodeExecutor<StartRuntimeNode>
    {
        public bool Execute(StartRuntimeNode node, StateMachineManager ctx)
        {
            return true;
        }
    }

    public class StateNodeExecutor : IStateMachineNodeExecutor<StateRuntimeNode>
    {
        public bool Execute(StateRuntimeNode node, StateMachineManager ctx)
        {
            return true;
        }
    }

    public class IfNodeExecutor : IStateMachineNodeExecutor<IfRuntimeNode>
    {
        public bool Execute(IfRuntimeNode node, StateMachineManager ctx)
        {
            var mediator = ctx.GetConditionMediator();

            if (mediator == null)
            {
                Debug.LogError("No Condition Mediator found on State Machine Manager");
                return false;
            }

            bool condition = false;
            List<bool> conditions;

            switch (node.ConectionType)
            {
                case ConectionType.Condition:
                    condition = mediator.GetConditionValue(node.ConditionRuntimeNode.BattleCondition);
                    break;
                case ConectionType.And:
                    conditions = new List<bool>();
                    foreach (BattleCondition battleCondition in node.AndRuntimeNode.Conditions)
                    {
                        bool cond = mediator.GetConditionValue(battleCondition);
                        conditions.Add(cond);
                    }
                    condition = conditions.All(b => b);
                    break;
                case ConectionType.Or:
                    conditions = new List<bool>();
                    foreach (BattleCondition battleCondition in node.OrRuntimeNode.Conditions)
                    {
                        bool cond = mediator.GetConditionValue(battleCondition);
                        conditions.Add(cond);
                    }
                    condition = conditions.Any(b => b);
                    break;
            }

            node.Condition = condition;
            return node.Condition;
        }
    }
}

