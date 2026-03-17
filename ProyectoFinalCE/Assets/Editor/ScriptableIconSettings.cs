using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableIconSettings", menuName = "Tools/Scriptable Icon Settings", order = 0)]
public class ScriptableIconSettings : ScriptableObject
{
    public List<ScriptableIconRule> rules = new List<ScriptableIconRule>();
}
