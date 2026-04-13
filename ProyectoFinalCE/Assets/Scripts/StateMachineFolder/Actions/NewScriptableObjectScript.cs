using StateMachine.Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/IA/Actions/Debug Log")]
public class DebugLogAction : ScriptableAction
{
    [Header("Custom Action Information")]
    public string message;

    public override void Execute(StateMachineManager manager)
    {
        Debug.Log(message);
    }
}