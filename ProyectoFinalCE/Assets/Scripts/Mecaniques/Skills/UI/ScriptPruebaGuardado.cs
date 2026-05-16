using UnityEngine;
using UnityEngine.UI;

public class ScriptPruebaGuardado : MonoBehaviour{
    public void Guardar()
    {
        SaveSystem.SaveGame();
    }
}