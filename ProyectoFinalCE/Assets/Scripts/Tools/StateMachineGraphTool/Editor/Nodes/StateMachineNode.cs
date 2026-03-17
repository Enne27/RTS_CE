using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using StateMachine.Runtime;
namespace StateMachine.Editor
{
    [Serializable]
    internal abstract class StateMachineNode : Node, INode
    {
        public const string EXECUTION_PORT_DEFAULT_NAME = "ExecutionPort";
    }

    [Serializable]
    internal class Start : StateMachineNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName("Start")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }

    [Serializable]
    internal class State : StateMachineNode
    {
        private const string TRANSITION_PORT_PREFIX = "Transition";

        private int inputs = 1;

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            // Nombre del estado
            context.AddOption("StateName", typeof(string)).WithDisplayName("State Name").WithDefaultValue("New State").Build();

            context.AddOption<int>("SkillQuantity").WithDisplayName("Action Quantity").WithDefaultValue(inputs).Build().TryGetValue<int>(out inputs);

            if (inputs < 1)
            {
                inputs = 1;
            }
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Puerto de entrada principal
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
           
            for (int i = 0; i < inputs; i++)
            {
                context.AddInputPort<ScriptableAction>($"Skill{i}")
                    .WithDisplayName($"Action {i}")
                    .WithConnectorUI(PortConnectorUI.Circle)
                    .Build();
            }
           

            context.AddOutputPort(TRANSITION_PORT_PREFIX)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }



    [Serializable]
    internal class If : StateMachineNode
    {

        public const string CONDITION_PORT = "Condition";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {

            // Puerto de entrada principal

            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("True")
                .WithDisplayName("True")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("False")
                .WithDisplayName("False")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<bool>(CONDITION_PORT)
                .WithDisplayName(CONDITION_PORT)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }


    [Serializable]
    internal class And : StateMachineNode
    {
        [SerializeField]
        private int links = 2;

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>("Links").WithDefaultValue(2).WithDisplayName("Conditions").Build().TryGetValue<int>(out links);

            if (links < 2)
            {
                links = 2;
            }
        }
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            for (int i = 0; i < links; i++)
            {
                context.AddInputPort<bool>($"Condition {i}")
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            }
            context.AddOutputPort<bool>("Result")
                .WithDisplayName("Result")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class Or : StateMachineNode
    {
        private int links = 2;

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>("Links").WithDefaultValue(2).WithDisplayName("Conditions").Build().TryGetValue<int>(out links);

            if (links < 2)
            {
                links = 2;
            }
        }
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            for (int i = 0; i < links; i++)
            {
                context.AddInputPort<bool>($"Condition {i}")
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            }
            context.AddOutputPort<bool>("Result")
                .WithDisplayName("Result")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }


    }

    [Serializable]
    internal class Condition : StateMachineNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            
            context.AddInputPort<BattleCondition>("Condition")
                .WithDisplayName("Condition")
                .WithDefaultValue(0)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
            
            context.AddOutputPort<bool>("Result")
                .WithDisplayName("Result")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}

