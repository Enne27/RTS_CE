using UnityEngine;
using static PlayerConstants;

public class AntCreation : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] GameHUDView gameHUDView;

    [Header("Transforms new ants Player")]
    [SerializeField] public Transform antsSpawnPoint;
    [SerializeField] public Transform workersSpawnPoint;  
    
    [Header("Transforms new ants Player")]
    [SerializeField] public Transform antsSpawnPointIA;


    [Header("Instantiation Values")]
    private GameObject antToInstantiate;
    private Transform positionInstantiate;

    [Header("Ants Prefabs")]
    [SerializeField] GameObject soldierAnt;
    [SerializeField] GameObject explorerAnt;
    [SerializeField] GameObject workerAnt;
    [SerializeField] GameObject berserkerAnt;
    [SerializeField] GameObject acidAnt;
    [SerializeField] GameObject crazyAnt;
    [SerializeField] GameObject kamikazeAnt;
    #endregion

    #region Singleton
    public static AntCreation Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        antToInstantiate = workerAnt;
    }

    private void Start()
    {
        GameManager.instance.player.ants.Clear();
        GameManager.instance.playerIA.ants.Clear();

        // Creación inicial de hormigas, tanto del jugador como de la IA.
        SystemAntCreation(4, ANT_TYPES.EXPLORER, antsSpawnPoint, true);
        SystemAntCreation(4, ANT_TYPES.WORKER, workersSpawnPoint, true);

        SystemAntCreation(4, ANT_TYPES.EXPLORER, antsSpawnPointIA, false);
    }
    #endregion

    /// <summary>
    /// Creación de hormigas del jugador mediante el uso de recursos y actualización de la interfaz.
    /// </summary>
    /// <param name="antType">Tipo de hormiga a instanciar.</param>
    /// <param name="position">Transform de la posición donde instanciar.</param>
    public void PlayerAntCreation(ANT_TYPES antType, Transform position)
    {
        //Debug.Log(antType);
        if (position != null)
        {
            ChangeAntTypeToInstantiate(antType);
            positionInstantiate = position;


            Ant antScript = antToInstantiate.GetComponent<Ant>();
            int foodCosts = 0;
            int hvCosts = 0;
            if (antScript != null)
            {
                foodCosts = antScript.GetBreedingCost()[0];
                hvCosts = antScript.GetBreedingCost()[1];
            }


            // FALTARÍA AÑADIR LO DE QUE SI HAY UNA HORMIGA DE ESE TIPO DESACTIVADA, USARLA, NO CREAR.
            // FALTARÍA AÑADIR EL TIEMPO DE CONSTRUCCIÓN DE ESA HORMIGA, SIMPLEMENTE USAR EL REGISTER DEL TIME MANAGER Y LUEGO UNREGISTER, PERO CUANDO SE TENGA FEEDBACK
            if (CanSpawnAnt(foodCosts, hvCosts))
            {
                SystemAntCreation(1, antType, position, true);

                if (gameHUDView != null)
                    gameHUDView.UpdateAntText(antType, 1);
                else
                {
                    gameHUDView = FindFirstObjectByType<GameHUDView>();
                    gameHUDView.UpdateAntText(antType, 1);
                }
            }
            else
            {
                Debug.Log("Insuficient hv or food");
            }
        }
    }

    /// <summary>
    /// Método para generar hormigas sin involucrar la interfaz.
    /// Añade al inventario del jugador o de la IA las hormigas creadas.
    /// </summary>
    /// <param name="quantity">Cantidad de hormigas a instanciar.</param>
    /// <param name="antType">Tipo de hormiga a instanciar.</param>
    /// <param name="position">Posición donde instanciar.</param>
    /// <param name="isPlayer">Es el jugador = true -> IA = false</param>
    private void SystemAntCreation(int quantity, ANT_TYPES antType, Transform position, bool isPlayer)
    {
        if (position != null)
        {
            for (int i = 0; i < quantity; i++)
            {
                ChangeAntTypeToInstantiate(antType);

                positionInstantiate = position;
                GameObject newAnt = AntInstantiation();

                if (isPlayer)
                {
                    if (antType != ANT_TYPES.WORKER)
                        GameManager.instance.player.ants.Add(newAnt.GetComponent<Ant>());
                    else
                        GameManager.instance.player.inventory.workerAnts++;
                    if (antType == ANT_TYPES.EXPLORER)
                        //newAnt.GetComponent<AntExlporer>().antHillPositionOwner = GameManager.instance.player.structures[0].transform.position;
                        newAnt.GetComponent<AntExlporer>().antOwner = Owner.Player;
                }
                else {
                    if (antType != ANT_TYPES.WORKER)
                        GameManager.instance.playerIA.ants.Add(newAnt.GetComponent<Ant>());
                    else
                        GameManager.instance.playerIA.inventory.workerAnts++;
                    if (antType == ANT_TYPES.EXPLORER)
                        //newAnt.GetComponent<AntExlporer>().antHillPositionOwner = GameManager.instance.playerIA.structures[0].transform.position;
                        newAnt.GetComponent<AntExlporer>().antOwner = Owner.AI;
                }
            }
        }
    }

    /// <summary>
    /// Instancia la hormiga en la posición indicada.
    /// </summary>
    /// <returns>GameObject instanciado.</returns>
    private GameObject AntInstantiation()
    {
        return Instantiate(antToInstantiate, positionInstantiate.position, Quaternion.identity);
    }

    /// <summary>
    /// Posibilidad de instanciación de hormigas según los parámetros necesarios.
    /// </summary>
    /// <param name="foodCosts">Comida necesaria para crearla.</param>
    /// <param name="hvCosts">Huevas necesarias para crearla.</param>
    /// <returns>True si hay suficiente comida y huevas en el inventario del jugador.</returns>
    private bool CanSpawnAnt(int foodCosts, int hvCosts)
    {
        Debug.Log(GameManager.instance.player.inventory.food >= foodCosts && GameManager.instance.player.inventory.eggs >= hvCosts);
        return (GameManager.instance.player.inventory.food >= foodCosts) && (GameManager.instance.player.inventory.eggs >= hvCosts);
    }

    /// <summary>
    /// Cambiamos el tipo de hormiga a instanciar por el parámetro.
    /// </summary>
    /// <param name="antType">Siguiente tipo de hormiga a instanciar.</param>
    private void ChangeAntTypeToInstantiate(ANT_TYPES antType)
    {
        switch (antType)
        {
            case ANT_TYPES.ACID:
                antToInstantiate = acidAnt;
                break;
            case ANT_TYPES.BERSERKER:
                antToInstantiate = berserkerAnt;
                break;
            case ANT_TYPES.EXPLORER:
                antToInstantiate = explorerAnt;
                break;
            case ANT_TYPES.SOLDIER:
                antToInstantiate = soldierAnt;
                break;
            case ANT_TYPES.CRAZY:
                antToInstantiate = crazyAnt;
                break;
            case ANT_TYPES.KAMIKAZE:
                antToInstantiate = kamikazeAnt;
                break;
            case ANT_TYPES.WORKER:
                antToInstantiate = workerAnt;
                break;
        }
    }

}
