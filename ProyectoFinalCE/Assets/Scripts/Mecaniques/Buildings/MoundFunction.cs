using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoundFunction : StructuresPlayer
{
    const int MAX_INTERVAL = 30;

    #region VARIABLES
    [Header("Building parameters")]
    [SerializeField] int moundHealthPoints;
    [SerializeField] int regenerationPower;
    [SerializeField] int intervalToRegenerateAfterDamage = 0;

    [SerializeField] BuildingData moundBuildingScriptable;

    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 0, 10, 20, 40, 60 };

    [Tooltip("Costes en materiales de construcci�n de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 0, 15, 25, 30, 45 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 0, 60, 90, 90, 120 };

    // El l�mite de huevas est� en playerConstants
    [Tooltip("Nivel m�ximo que puede alcanzar la construcci�n por cada era.")]
    int[] maxLevelByEra_ = { 1, 2, 4, 5 };

    [Tooltip("Cantidad de vida que tiene el monticulo por nivel.")]
    int[] maxHealthByUpgrade = { 100, 150, 200, 250, 500 };

    int[] healthRegenPowerByUpgrade = { 1, 1, 1, 1, 2 };

    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;

    public override int[] costsUpgradeMC => costsUpgradeMC_;

    public override int[] timeUpgrade => timeUpgrade_;

    public override int[] maxLevelByEra => maxLevelByEra_;

    [Header("Visual player")]
    GameHUDView hudView;

    private bool isRegenerating = false;
    private bool allowRegeneration = false;
    private bool isDead = false;

    public bool takeDamageDebugButton;
    public int debugDamage;

    [Header("UI References")]
    [SerializeField] Slider sliderHPBar;
    [SerializeField] TextMeshProUGUI textHPLabel;

    [HideInInspector] public Owner owner;
    #endregion

    #region METHODS_STRUCTURES
    private void Awake()
    {
        hudView = FindFirstObjectByType<GameHUDView>();
        moundHealthPoints = maxHealthByUpgrade[currentLevel-1];
        regenerationPower = healthRegenPowerByUpgrade[currentLevel-1];
        UpdateUI();
    }

    public override void OnConstructionFinished()
    {
        moundHealthPoints = maxHealthByUpgrade[currentLevel-1];
        UpdateUI();
    }
    #endregion

    void Update()
    {
        if(isDead) return;

        //if (takeDamageDebugButton) TakeDamage(debugDamage,owner);
        
        //takeDamageDebugButton = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        AntExlporer antExlporer = other.gameObject.GetComponent<AntExlporer>();
        if (antExlporer != null)
        {
            antExlporer.Deposit();
            if (hudView != null) {
                hudView.UpdateMCText();
                hudView.UpdateFoodText();
            }
        }
    }



    public void TakeDamage(int damage, Owner antOwner)
    {
        if (isDead) return;

        if((moundHealthPoints - damage) > 0)
        {
            moundHealthPoints -= damage;
            Debug.Log("La vida del hormiguero: " + moundHealthPoints);
        }
        else
        {
            moundHealthPoints = 0;
            MoundDestruction();
            return;
        }

        // Resetear regeneraci�n
        allowRegeneration = false;
        intervalToRegenerateAfterDamage = 0;
        TimeManager.Instance.Unregister(1, AllowToRegenerate);
        TimeManager.Instance.Unregister(1, Regenerate);
        isRegenerating = false;


        // Empezar contador para poder regenerar
        TimeManager.Instance.Register(1, AllowToRegenerate);
        UpdateUI();
        owner = antOwner;
    }

    public void AllowToRegenerate()
    {
        if (allowRegeneration) return;

        if (intervalToRegenerateAfterDamage >= MAX_INTERVAL)
        {
            allowRegeneration = true;

            TimeManager.Instance.Register(1, Regenerate);
            TimeManager.Instance.Unregister(1, AllowToRegenerate);
        }
        else
        {
            intervalToRegenerateAfterDamage++;
        }
    }

    public void Regenerate()
    {

        if (moundHealthPoints >= maxHealthByUpgrade[currentLevel - 1])
        {
            moundHealthPoints = maxHealthByUpgrade[currentLevel - 1];

            isRegenerating = false;
            allowRegeneration = false;
            return;
        }

        isRegenerating = true;

        int healthPointsRegeneration =
            healthRegenPowerByUpgrade[currentLevel - 1] * maxHealthByUpgrade[currentLevel - 1] / 100;

        moundHealthPoints += healthPointsRegeneration;

        // Clamp para no pasarse
        if (moundHealthPoints > maxHealthByUpgrade[currentLevel - 1])
            moundHealthPoints = maxHealthByUpgrade[currentLevel - 1];
        UpdateUI();
    }

    void MoundDestruction()
    {
        isDead = true;
        //ViewManager.Show<EndGameView>();

        if(owner == Owner.Player)
        {

        }
        else
        {

        }

        //Debug.Log("AUAUAU me muero deberia morirme porfavor poned el codigo para que me muera quiero morir ahora matadme no requiero vivir terminad con mi sufrimiento AAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }

    void UpdateUI()
    {
        textHPLabel.text = $"{maxHealthByUpgrade[currentLevel-1]}/{moundHealthPoints}";
        sliderHPBar.value = moundHealthPoints * maxHealthByUpgrade[currentLevel - 1] / 100;
    }
}
