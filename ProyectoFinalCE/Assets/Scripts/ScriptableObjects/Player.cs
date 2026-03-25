using Unity.VisualScripting;
using UnityEngine;
using static PlayerConstants;

public class Player : MonoBehaviour
{
    #region Variables
    public int id;
    public string playerName;
    [Tooltip("The outline color that this player's ants will have")]
    public Color playerColor;
    [Tooltip("An inventory GameObject will be created at runtime")]
    public Inventory inventory;
    [Tooltip("The era at wich the player's hive is at")]
    public HIVE_ERAS currentEra;
    #endregion

    #region Methods
    private void Awake()
    {
        //Instanciamos el inventario
        inventory = this.AddComponent<Inventory>();
    }
    #endregion

}
