
using UnityEngine;
using StateMachine.Runtime;
using Unity.Cinemachine;
using System;

[CreateAssetMenu(fileName = "AntScript", menuName = "Tools/IA/Actions/AntScript")]
public class AntScript : ScriptableAction
{
    public override void Execute(StateMachineManager manager)
    { 
      void Attack()
      {
        Ant ant;
        //ant.Attack(ant.GetTarget);      
      }
    }
}