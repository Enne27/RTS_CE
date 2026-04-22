using StateMachine.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace StateMachine.Editor
{
    [ScriptedImporter(1, StateMachineGraphWindow.AssetExtension)]
    internal class StateMachineGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<StateMachineGraphWindow>(ctx.assetPath);

            if (graph == null)
            {
                Debug.LogError($"Failed to load State Machine Graph asset: {ctx.assetPath}");
                return;
            }

            var startNodeModel = graph.GetNodes().OfType<Start>().FirstOrDefault();

            var anyStateNodeModel = graph.GetNodes().OfType<AnyState>().FirstOrDefault();

            if (startNodeModel == null)
            {
                Debug.LogError($"No Start in State Machine Graph asset: {ctx.assetPath}");
                return;
            }


            var runtimeAsset = ScriptableObject.CreateInstance<StateMachineController>();
            var nodeMap = new Dictionary<INode, int>();
            var anyStateNodeMap = new Dictionary<INode, int>();

            if (anyStateNodeModel == null)
            {
                runtimeAsset.AnyStateNodes.Clear();
            }
            else
            {
                if (!ValidateAnyStateConnections(anyStateNodeModel))
                {
                    return;
                }
                CreateRuntimeAnyStateNodes(anyStateNodeModel, runtimeAsset, anyStateNodeMap);
                SetupAnyStateConnections(runtimeAsset, anyStateNodeMap);
            }

            CreateRuntimeNodes(startNodeModel, runtimeAsset, nodeMap);
            SetupConnections(runtimeAsset, nodeMap);

            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
            Debug.Log("State Machine Import Successful!");
        }

        void CreateRuntimeNodes(INode startNode, StateMachineController runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            var nodesToProcess = new Queue<INode>();
            nodesToProcess.Enqueue(startNode);

            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();

                if (nodeMap.ContainsKey(currentNode)) continue;

                var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);

                foreach (var runtimeNode in runtimeNodes)
                {
                    nodeMap[currentNode] = runtimeGraph.Nodes.Count;
                    runtimeGraph.Nodes.Add(runtimeNode);
                }

                for (int i = 0; i < currentNode.outputPortCount; i++)
                {
                    var port = currentNode.GetOutputPort(i);
                    if (port.isConnected)
                    {
                        nodesToProcess.Enqueue(port.firstConnectedPort.GetNode());
                    }
                }
            }
        }

        void SetupConnections(StateMachineController runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            foreach (var kvp in nodeMap)
            {
                var editorNode = kvp.Key;
                var runtimeIndex = kvp.Value;
                var runtimeNode = runtimeGraph.Nodes[runtimeIndex];

                for (int i = 0; i < editorNode.outputPortCount; i++)
                {
                    var port = editorNode.GetOutputPort(i);

                    if (port.isConnected && nodeMap.TryGetValue(port.firstConnectedPort.GetNode(), out int nextIndex))
                    {
                        runtimeNode.NextNodeIndices.Add(nextIndex);
                    }
                }
            }
        }

        void SetupAnyStateConnections(StateMachineController runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            foreach (var kvp in nodeMap)
            {
                var editorNode = kvp.Key;
                var runtimeIndex = kvp.Value;
                var runtimeNode = runtimeGraph.AnyStateNodes[runtimeIndex];

                for (int i = 0; i < editorNode.outputPortCount; i++)
                {
                    var port = editorNode.GetOutputPort(i);

                    if (port.isConnected && nodeMap.TryGetValue(port.firstConnectedPort.GetNode(), out int nextIndex))
                    {
                        runtimeNode.NextNodeIndices.Add(nextIndex);
                    }
                }
            }
        }

        void CreateRuntimeAnyStateNodes(INode anyStateNode, StateMachineController runtimeGraph, Dictionary<INode, int> nodeMap)
        {
            var nodesToProcess = new Queue<INode>();
            nodesToProcess.Enqueue(anyStateNode);

            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();

                if (nodeMap.ContainsKey(currentNode)) continue;

                var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);

                foreach (var runtimeNode in runtimeNodes)
                {
                    nodeMap[currentNode] = runtimeGraph.AnyStateNodes.Count;
                    runtimeGraph.AnyStateNodes.Add(runtimeNode);
                }

                for (int i = 0; i < currentNode.outputPortCount; i++)
                {
                    var port = currentNode.GetOutputPort(i);
                    if (port.isConnected)
                    {
                        nodesToProcess.Enqueue(port.firstConnectedPort.GetNode());
                    }
                }
            }
        }

        static List<StateMachineRuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodemodel)
        {
            const string CONDITION_NODE = "StateMachine.Editor.Condition";
            const string AND_NODE = "StateMachine.Editor.And";
            const string OR_NODE = "StateMachine.Editor.Or";
            var returnedNodes = new List<StateMachineRuntimeNode>();

            switch (nodemodel)
            {
                case Start:
                    returnedNodes.Add(new StartRuntimeNode());
                    break;
                case AnyState anyState:
                    returnedNodes.Add(new AnyStateRuntimeNode());
                    break;
                case State stateNode:
                    string stateName = "New State";
                    int skillQuantity = 1;
                    stateNode.GetNodeOptionByName("StateName")?.TryGetValue(out stateName);
                    stateNode.GetNodeOptionByName("SkillQuantity")?.TryGetValue(out skillQuantity);
                    List<ScriptableAction> portActions = new();
                    for(int i = 0; i < skillQuantity; i++)
                    {
                        ScriptableAction action = null;
                        stateNode.GetInputPortByName($"Skill{i}")?.TryGetValue(out action);
                        portActions.Add(action);
                    }
                    returnedNodes.Add(new StateRuntimeNode
                    {
                        StateName = stateName,
                        actions = portActions
                    });
                    break;
                case If ifNode:
                    var ifConditionPort = ifNode.GetInputPortByName("Condition");
                    bool hasTrueOutput = ifNode.GetOutputPortByName("True").isConnected;
                    bool hasFalseOutput = ifNode.GetOutputPortByName("False").isConnected;
                    if (ifConditionPort.isConnected)
                    {
                       if (ifConditionPort.firstConnectedPort.GetNode().ToString() == CONDITION_NODE) {
                            Conditions condition = 0;
                            ifConditionPort.firstConnectedPort.GetNode().GetInputPort(0)?.TryGetValue(out condition);
                            returnedNodes.Add(new IfRuntimeNode
                            {
                                ConditionRuntimeNode = new ConditionRuntimeNode { BattleCondition = condition },
                                hasTrueOutput = hasTrueOutput,
                                hasFalseOutput = hasFalseOutput,
                                ConectionType = ConectionType.Condition 
                            });
                       }
                       else if(ifConditionPort.firstConnectedPort.GetNode().ToString() == AND_NODE)
                       {
                            int condQuantity = 2;
                            ifNode.GetNodeOptionByName("Links")?.TryGetValue(out condQuantity);
                            List<Conditions> conditions = new();
                            for(int i = 0; i < condQuantity; i++)
                            {
                                Conditions condition = 0;
                                ifConditionPort.firstConnectedPort.GetNode().GetInputPort(i)?.TryGetValue(out condition);
                                conditions.Add(condition);
                            }
                            returnedNodes.Add(new IfRuntimeNode
                            {
                                AndRuntimeNode = new  AndRuntimeNode{ Conditions = conditions },
                                hasTrueOutput = hasTrueOutput,
                                hasFalseOutput = hasFalseOutput,
                                ConectionType = ConectionType.And
                            });
                       }
                       else if (ifConditionPort.firstConnectedPort.GetNode().ToString() == OR_NODE)
                       {
                            int condQuantity = 2;
                            ifNode.GetNodeOptionByName("Links")?.TryGetValue(out condQuantity);
                            List<Conditions> conditions = new();
                            for (int i = 0; i < condQuantity; i++)
                            {
                                Conditions condition = 0;
                                ifConditionPort.firstConnectedPort.GetNode().GetInputPort(i)?.TryGetValue(out condition);
                                conditions.Add(condition);
                            }
                            returnedNodes.Add(new IfRuntimeNode
                            {
                                OrRuntimeNode = new OrRuntimeNode { Conditions = conditions },
                                hasTrueOutput = hasTrueOutput,
                                hasFalseOutput = hasFalseOutput,
                                ConectionType = ConectionType.Or
                            });
                       }
                    }
                    break;
                default:
                    throw new ArgumentException($"Unsupported node type: {nodemodel.GetType()}");
            }

            return returnedNodes;
        }
        bool ValidateAnyStateConnections(INode anyStateNode)
        {
            var visited = new HashSet<INode>();
            var queue = new Queue<(INode node, bool passedIf)>();

            queue.Enqueue((anyStateNode, false));

            while (queue.Count > 0)
            {
                var (currentNode, passedIf) = queue.Dequeue();

                if (!visited.Add(currentNode))
                    continue;

                bool isIfNode = currentNode is If;
                bool newPassedIf = passedIf || isIfNode;
                if (currentNode is State && !newPassedIf)
                {
                    return false;
                }

                for (int i = 0; i < currentNode.outputPortCount; i++)
                {
                    var port = currentNode.GetOutputPort(i);
                    if (port.isConnected)
                    {
                        queue.Enqueue((port.firstConnectedPort.GetNode(), newPassedIf));
                    }
                }
            }

            return true;
        }


        static T GetInputPortValue<T>(IPort port)
        {
            T value = default;

            if (port.isConnected)
            {
                switch (port.firstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.variable.TryGetDefaultValue<T>(out value);
                        return value;
                    case IConstantNode constantNode:
                        constantNode.TryGetValue<T>(out value);
                        return value;
                    default:
                        break;
                }
            }
            else
            {
                port.TryGetValue(out value);
            }

            return value;
        }
    }
}




