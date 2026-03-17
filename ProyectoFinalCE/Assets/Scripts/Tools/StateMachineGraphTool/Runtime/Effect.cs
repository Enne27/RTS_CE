using UnityEngine;

namespace PooposoQuest.Classes
{
    public class Effect
    {
        public EffectType type;
    }

    public enum EffectType
    {
        None,
        AddStatus,
        RemoveStatus
    }
}
