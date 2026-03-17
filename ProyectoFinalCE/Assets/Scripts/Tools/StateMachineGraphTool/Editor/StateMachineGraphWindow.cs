using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace StateMachine.Editor
{
    [Serializable]
    [Graph(AssetExtension, GraphOptions.Default)]
    public class StateMachineGraphWindow : Graph
    {
        internal const string AssetExtension = "smg";

        [MenuItem("Assets/Create/Tools/IA/New State Machine Graph")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<StateMachineGraphWindow>("NewStateMachineGraph");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);
            //Aqui se puede hacer error checking 
        }
    }
}






