using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace StateMachine.Runtime
{
    [System.Serializable]
    public class StateMachineManager
    {
        private StateMachineController controller;

        private Condition mediator;

        private Dictionary<System.Type, object> executors = new Dictionary<System.Type, object>()
            {
                { typeof(StartRuntimeNode), new StartNodeExecutor() },
                { typeof(AnyStateRuntimeNode), new AnyStateNodeExecutor() },
                { typeof(StateRuntimeNode), new StateNodeExecutor() },
                { typeof(IfRuntimeNode), new IfNodeExecutor() }
            };  

        private StateMachineRuntimeNode currentNode;

        private StateMachineRuntimeNode anyState;

        private bool loadDebugLog;

        public StateMachineManager(StateMachineController controller, Context context)
        {
            this.controller = controller;
            currentNode = this.controller.Nodes[0];
            if (this.controller.AnyStateNodes.Any())
                anyState = this.controller.AnyStateNodes[0];
            mediator = new Condition(context);
            loadDebugLog = false;
        }

        public StateMachineManager(StateMachineController controller, Context context, bool debugLog)
        {
            this.controller = controller;
            currentNode = this.controller.Nodes[0];
            if (this.controller.AnyStateNodes.Any())
                anyState = this.controller.AnyStateNodes[0];
            mediator = new Condition(context);
            loadDebugLog = debugLog;
        }

        public StateMachineController GetController()
        {
            return controller;
        }

        public void SetController(StateMachineController newController)
        {
            controller = newController;
        }

        public Condition GetConditionMediator() => this.mediator;

        public void ActivateStateMachineLog()
        {
            loadDebugLog = true;
        }

        public void DeactivateStateMachineLog()
        {
            loadDebugLog = false;
        }
        

        /// <summary>
        /// Detetcta el tipo de nodo en el que se encuentra una maquina de estados y ejecuta la logica de 
        /// </summary>
        public void StateExecutor()
        { 

            if(!executors.TryGetValue(currentNode.GetType(), out var executor))
            {
                Debug.LogError($"No executor found for node type: {currentNode.GetType()}");
                return;
            }
            
            if(anyState != null)
            {
                foreach (var nextIndex in anyState.NextNodeIndices)
                {
                    var nextNode = controller.AnyStateNodes[nextIndex];

                    if (nextNode is IfRuntimeNode ifNode)
                    {
                        var ifExecutor = (IStateMachineNodeExecutor<IfRuntimeNode>)executors[typeof(IfRuntimeNode)];

                        if (ifExecutor.Execute(ifNode, this) == true)
                            if (ifNode.hasTrueOutput)
                                currentNode = controller.AnyStateNodes[ifNode.NextNodeIndices[0]];
                        else if (ifExecutor.Execute(ifNode, this) == false)
                            if (ifNode.hasFalseOutput)
                            {
                                if (!ifNode.hasTrueOutput)
                                    currentNode = controller.AnyStateNodes[ifNode.NextNodeIndices[0]];
                                else
                                    currentNode = controller.AnyStateNodes[ifNode.NextNodeIndices[1]];
                            }
                    }
                }
            }
            
            if(currentNode is StartRuntimeNode startNode)
            {
                var startExecutor = (IStateMachineNodeExecutor<StartRuntimeNode>)executor;
                startExecutor.Execute(startNode, this);
                foreach (var nextIndex in startNode.NextNodeIndices)
                {
                    var nextNode = controller.Nodes[nextIndex];

                    if (nextNode is IfRuntimeNode ifNode)
                    {
                        var ifExecutor = (IStateMachineNodeExecutor<IfRuntimeNode>)executors[typeof(IfRuntimeNode)];

                        if (ifExecutor.Execute(ifNode, this) == true)
                        {
                            if (ifNode.hasTrueOutput)
                            {
                                currentNode = controller.Nodes[ifNode.NextNodeIndices[0]];
                            }
                        }
                        else if (ifExecutor.Execute(ifNode, this) == false)
                        {
                            if (ifNode.hasFalseOutput)
                            {
                                if (!ifNode.hasTrueOutput)
                                {
                                    currentNode = controller.Nodes[ifNode.NextNodeIndices[0]];
                                }
                                else
                                {
                                    currentNode = controller.Nodes[ifNode.NextNodeIndices[1]];
                                }

                            }

                        }
                    }
                    else
                    {
                        currentNode = startNode.NextNodeIndices.Count > 0
                        ? controller.Nodes[startNode.NextNodeIndices[0]]
                        : null;
                    }
                }
            }
            else if(currentNode is StateRuntimeNode stateNode)
            {
                var stateExecutor = (IStateMachineNodeExecutor<StateRuntimeNode>)executor;
                stateExecutor.Execute(stateNode, this);
                
                foreach (var nextIndex in stateNode.NextNodeIndices)
                {
                    var nextNode = controller.Nodes[nextIndex];

                    if(nextNode is IfRuntimeNode ifNode)
                    {
                        var ifExecutor = (IStateMachineNodeExecutor<IfRuntimeNode>)executors[typeof(IfRuntimeNode)];

                        if (ifExecutor.Execute(ifNode, this) == true)
                        {
                            if (ifNode.hasTrueOutput)
                            {
                                currentNode = controller.Nodes[ifNode.NextNodeIndices[0]];
                            }
                        }
                        else if (ifExecutor.Execute(ifNode, this) == false)
                        {
                            if (ifNode.hasFalseOutput)
                            {
                                if (!ifNode.hasTrueOutput)
                                {
                                    currentNode = controller.Nodes[ifNode.NextNodeIndices[0]];
                                }
                                else
                                {
                                    currentNode = controller.Nodes[ifNode.NextNodeIndices[1]];
                                }

                            }
                            
                        }
                    }
                    else
                    {
                        currentNode = stateNode.NextNodeIndices.Count > 0
                            ? controller.Nodes[stateNode.NextNodeIndices[0]]
                            : null;
                    }
                }
            }

            if(currentNode is StateRuntimeNode nombre)
            {
                if (loadDebugLog)
                {
                    Debug.Log($"The current state is: {nombre.StateName}");
                    foreach(var action in GetCurrentStateActions())
                    {
                        if(action!= null)
                            Debug.Log($"The current state has this action: {action.actionName}");
                        else
                        {
                            Debug.Log($"The current state has no actions");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("El manager no ha terminado en un Estado");
            }
        }

        public void ExecuteActionsBehaviour()
        {
            List<ScriptableAction> actions = GetCurrentStateActions();

            if (actions == null || actions.Count == 0)
                return;

            foreach (var action in actions)
            {
                if (action != null)
                    action.Execute(this);
            }
        }

        /// <summary>
        /// Funcion que te devuelve las acciones del estado en el que se encuentra la IA
        /// </summary>
        /// <returns>Lista de acciones</returns>
        public List<ScriptableAction> GetCurrentStateActions()
        {
            if (currentNode is StateRuntimeNode currentState)
            {
                return currentState.actions;
            }
            return null;
        }

        public string GetCurrentStateName()
        {
            if (currentNode is StateRuntimeNode nombre)
                return nombre.StateName;
            else
                return "";
        }
    }
}

