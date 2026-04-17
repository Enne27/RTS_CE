[System.Serializable] //Se aplica cuando no heredan de MonoBehaviour ni ScriptableObject. para guardarlo en escenas, prefabs etc
// Representa un efecte individual d'una skill
public class SkillEffect
{
    public StatType statType;
    public float value;
}