
using UnityEngine;
using StateMachine.Runtime;
using Unity.Cinemachine;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "AntScript", menuName = "Tools/IA/Actions/AntScript")]
public class AntScript : ScriptableAction
{
    public override void Execute(StateMachineManager manager)
    { 
      void Attack(Ant target)
      {
            Ant ant = this.GetComponent<Ant>();
            ant.Attack(target);
      }

      void Die()
      {
            Ant ant = this.GetComponent<Ant>();
            ant.Die();
      }

      void MoveTo(Vector3 _objective)
      {
            Ant ant = this.GetComponent<Ant>();
            ant.MoveTo(_objective);
      }
        
      void AttackStructure(float strength)
      {
            Ant ant = this.GetComponent<Ant>();
            ant.AttackStructure(strength);
      }
    }
}