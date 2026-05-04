using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace ScriptableIconManager
{
    [CreateAssetMenu(fileName = "ScriptableIconSettings", menuName = "Tools/Scriptable Icon Settings", order = 0)]
    public class ScriptableIconSettings : ScriptableObject
    {
        public List<ScriptableIconRule> rules = new List<ScriptableIconRule>();
    }
}
#endif