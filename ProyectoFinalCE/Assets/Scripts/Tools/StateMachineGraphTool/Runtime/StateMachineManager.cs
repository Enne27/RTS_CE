using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace StateMachine.Runtime
{
    [System.Serializable]
    public class StateMachineManager
    {
        StateMachineController controller;

        Condition mediator;

        Dictionary<System.Type, object> executors = new Dictionary<System.Type, object>()
            {
                { typeof(StartRuntimeNode), new StartNodeExecutor() },
                { typeof(StateRuntimeNode), new StateNodeExecutor() },
                { typeof(IfRuntimeNode), new IfNodeExecutor() } 
            };  

        StateMachineRuntimeNode currentNode;

        public StateMachineManager(StateMachineController controller, BattleContext context)
        {
            this.controller = controller;
            currentNode = this.controller.Nodes[0];

            mediator = new Condition(context);
        }

        public StateMachineController GetController()
        {
            return controller;
        }

        public void SetController(StateMachineController newController)
        {
            controller = newController;
        }

        public Condition GetConditionMediator()
        {
            return this.mediator;
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
                }
            }

            if(currentNode is StateRuntimeNode nombre)
            {
                Debug.Log($"El estado actual es {nombre.StateName}");
            }
            else
            {
                Debug.LogError("El manager no ha terminado en un Estado");
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
    }
}

