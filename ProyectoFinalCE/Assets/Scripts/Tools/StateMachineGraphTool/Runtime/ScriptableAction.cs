using UnityEngine;
namespace StateMachine.Runtime
{
    [CreateAssetMenu(fileName = "New ScriptableAction", menuName = "Tools/IA/New ScriptableAction")]
    public class ScriptableAction : ScriptableObject
    {
        [Header("General Settings")]
        public string actionName;
        public string actionDescription;
        public Sprite actionIcon;
        public int costMP;

        [Header("Calling")]
        [Range(0, 100)] public int successProb;
        public int repeatNum;
        public TarguetType targuet;

        [Header("Damage")]
        public DamageType damageType;
        public string formula;
        public bool couldBeCriticalHit;

    }
}

