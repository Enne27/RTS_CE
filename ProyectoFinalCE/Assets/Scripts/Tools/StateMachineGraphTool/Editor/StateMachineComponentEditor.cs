using StateMachine.Runtime;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachineComponent))]
public class StateMachineComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();

        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (prop.name == "m_Script") continue;

            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();

        StateMachineComponent comp = (StateMachineComponent)target;
        var manager = comp.GetMachineManager();

        EditorGUILayout.Space();
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("The State Machine Manager is only available in Play Mode.", MessageType.Info);
            return;
        }

        if (manager != null)
        {
            string message = $"\n   State Machine active.\n" +
                     $"   Current State: {manager.GetCurrentStateName()}\n\n";

            var actions = manager.GetCurrentStateActions();

            if (actions == null || actions.Count == 0)
            {
                message += "   Actions:\n   - No hay acciones en este estado.";
            }
            else
            {
                message += "   Actions:\n";

                foreach (var action in actions)
                {
                    message += action != null
                        ? $"   - {action.name}\n"
                        : "   - NULL\n";
                }
            }

            EditorGUILayout.HelpBox(message, MessageType.Info, true);

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box"); 

            EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);

            if (actions == null || actions.Count == 0)
            {
                EditorGUILayout.LabelField("- No hay acciones en este estado.");
            }
            else
            {
                foreach (var action in actions)
                {
                    if (action == null)
                    {
                        EditorGUILayout.LabelField("- NULL");
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(
                        action,
                        typeof(ScriptableAction),
                        false
                    );

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("State Machine Controls", EditorStyles.boldLabel);

        bool execution = comp.execution;
        bool isStopped = comp.isStopped;

        if (!execution && isStopped)
        {
            if (GUILayout.Button("Start"))
            {
                comp.StartStateMachineExecution();
            }
        }
        else if (execution && !isStopped)
        {
            if (GUILayout.Button("Pause"))
            {
                comp.PauseStateMachineExecution();
            }

            if (GUILayout.Button("Stop"))
            {
                comp.StopStateMachineExecution();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset"))
            {
                comp.ResetStateMachineExecution();
            }
        }
        else if (!execution && !isStopped)
        {
            if (GUILayout.Button("Unpause"))
            {
                comp.UnPauseSateMachineExecution();
            }

            if (GUILayout.Button("Stop"))
            {
                comp.StopStateMachineExecution();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset"))
            {
                comp.ResetStateMachineExecution();
            }
        }


        EditorGUILayout.EndVertical();
    }
}