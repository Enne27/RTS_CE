using UnityEngine;
using StateMachine.Runtime;

[CreateAssetMenu(fileName = "PepitoAction", menuName = "Tools/IA/Actions/PepitoAction")]
public class PepitoAction : ScriptableAction
{
    public string message;
    public override void Execute(StateMachineManager manager)
    {
        // TODO: Implement action logic
        Debug.Log(message);
    }
}