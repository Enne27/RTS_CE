// Representa una skill en estat del jugador
public class SkillNode
{
    public SkillData data;   // Referència a la definició de la skill
    public bool isUnlocked;  // Indica si està desbloquejada

    public SkillNode(SkillData skillData) //Constructor
    {
        data = skillData;
        isUnlocked = false;
    }
}