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
    [HideInInspector] public GameObject antToInstantiate;
    [HideInInspector] public Transform positionInstantiate;

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
    #endregion

    private void Start()
    {
        GameManager.instance.player.ants.Clear();
        GameManager.instance.playerIA.ants.Clear();

        // Creación inicial de hormigas, tanto del jugador como de la IA.
        SystemAntCreation(GameManager.instance.startingExplorerAnts, ANT_TYPES.EXPLORER, antsSpawnPoint, true, !GameManager.instance.tutorialShown);
        SystemAntCreation(GameManager.instance.startingWorkerAnts, ANT_TYPES.WORKER, workersSpawnPoint, true, !GameManager.instance.tutorialShown);

        SystemAntCreation(GameManager.instance.startingExplorerAnts, ANT_TYPES.EXPLORER, antsSpawnPointIA, false, !GameManager.instance.tutorialShown);
                
    }

    /// <summary>
    /// Método para generar hormigas sin involucrar la interfaz.
    /// Añade al inventario del jugador o de la IA las hormigas creadas.
    /// </summary>
    /// <param name="quantity">Cantidad de hormigas a instanciar.</param>
    /// <param name="antType">Tipo de hormiga a instanciar.</param>
    /// <param name="position">Posición donde instanciar.</param>
    /// <param name="isPlayer">Es el jugador = true -> IA = false</param>
    public void SystemAntCreation(int quantity, ANT_TYPES antType, Transform position, bool isPlayer, bool addsQuantity)
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
                        //GameManager.instance.player.ants.Add(newAnt.GetComponentInChildren<Ant>());
                    else if (addsQuantity) GameManager.instance.player.inventory.workerAnts++; 
                    
                    if(antType == ANT_TYPES.WORKER)
                       GameManager.instance.player.workers.Add(newAnt.GetComponentInChildren<AntWorkerBehaviour>());
                    
                    
                    if (antType == ANT_TYPES.EXPLORER)
                        //newAnt.GetComponent<AntExlporer>().antHillPositionOwner = GameManager.instance.player.structures[0].transform.position;
                        newAnt.GetComponent<AntExlporer>().antOwner = Owner.Player;
                }
                else {
                    if (antType != ANT_TYPES.WORKER)
                        GameManager.instance.playerIA.ants.Add(newAnt.GetComponent<Ant>());
                    else if (addsQuantity) GameManager.instance.playerIA.inventory.workerAnts++;
                    
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
    public GameObject AntInstantiation()
    {
        return Instantiate(antToInstantiate, positionInstantiate.position, Quaternion.identity);
    }

    /// <summary>
    /// Posibilidad de instanciación de hormigas según los parámetros necesarios.
    /// </summary>
    /// <param name="foodCosts">Comida necesaria para crearla.</param>
    /// <param name="hvCosts">Huevas necesarias para crearla.</param>
    /// <returns>True si hay suficiente comida y huevas en el inventario del jugador.</returns>
    public bool CanSpawnAnt(int foodCosts, int hvCosts)
    {
        //Debug.Log(GameManager.instance.player.inventory.food >= foodCosts && GameManager.instance.player.inventory.eggs >= hvCosts);
        return (GameManager.instance.player.inventory.food >= foodCosts) && (GameManager.instance.player.inventory.eggs >= hvCosts);
    }

    /// <summary>
    /// Cambiamos el tipo de hormiga a instanciar por el parámetro.
    /// </summary>
    /// <param name="antType">Siguiente tipo de hormiga a instanciar.</param>
    public void ChangeAntTypeToInstantiate(ANT_TYPES antType)
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
