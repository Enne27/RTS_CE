using UnityEngine;
using static PlayerConstants;

public class AntCreation : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] GameHUDView gameHUDView;

    [Header("Transforms new ants Player")]
    [SerializeField] public Transform antsSpawnPoint;
    [SerializeField] public Transform workersSpawnPoint;

    [Header("Transforms new ants IA")]
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

    // IMPORTANTE -> static
    private static bool loadedFromSave = false;

    public static void MarkLoaded()
    {
        loadedFromSave = true;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        antToInstantiate = workerAnt;
    }

    private void Start()
    {
        // Si venimos de save NO crear hormigas iniciales
        if (loadedFromSave)
            return;

        GameManager.instance.player.ants.Clear();
        GameManager.instance.playerIA.ants.Clear();

        SystemAntCreation(4, ANT_TYPES.EXPLORER, antsSpawnPoint, true);
        SystemAntCreation(4, ANT_TYPES.WORKER, workersSpawnPoint, true);
        SystemAntCreation(4, ANT_TYPES.EXPLORER, antsSpawnPointIA, false);
    }
    #endregion

    public void PlayerAntCreation(ANT_TYPES antType, Transform position)
    {
        if (position == null)
            return;

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

        if (CanSpawnAnt(foodCosts, hvCosts))
        {
            SystemAntCreation(1, antType, position, true);

            if (gameHUDView == null)
                gameHUDView = FindFirstObjectByType<GameHUDView>();

            gameHUDView.UpdateAntText(antType, 1);
        }
        else
        {
            Debug.Log("Insufficient hv or food");
        }
    }

    private void SystemAntCreation(int quantity, ANT_TYPES antType, Transform position, bool isPlayer)
    {
        if (position == null)
            return;

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
                    newAnt.GetComponent<AntExlporer>().antOwner = Owner.Player;
            }
            else
            {
                if (antType != ANT_TYPES.WORKER)
                    GameManager.instance.playerIA.ants.Add(newAnt.GetComponent<Ant>());
                else
                    GameManager.instance.playerIA.inventory.workerAnts++;

                if (antType == ANT_TYPES.EXPLORER)
                    newAnt.GetComponent<AntExlporer>().antOwner = Owner.AI;
            }
        }
    }

    private GameObject AntInstantiation()
    {
        return Instantiate(antToInstantiate, positionInstantiate.position, Quaternion.identity);
    }

    private bool CanSpawnAnt(int foodCosts, int hvCosts)
    {
        return (GameManager.instance.player.inventory.food >= foodCosts)
            && (GameManager.instance.player.inventory.eggs >= hvCosts);
    }

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