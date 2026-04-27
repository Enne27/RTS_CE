using UnityEngine;

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

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 0, 15, 25, 30, 45 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 0, 60, 90, 90, 120 };

    // El límite de huevas está en playerConstants
    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
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
    #endregion

    #region METHODS_STRUCTURES
    private void Awake()
    {
        hudView = FindFirstObjectByType<GameHUDView>();
    }

    public override void OnConstructionFinished()
    {
        moundHealthPoints = maxHealthByUpgrade[currentLevel];
        TakeDamage(50);
    }
    #endregion

    void Update()
    {
        if(isDead) return;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if((damage - moundHealthPoints) > 0)
        {
            moundHealthPoints -= damage;
            intervalToRegenerateAfterDamage = 0;
        }
        else
        {
            moundHealthPoints = 0;
            MoundDestruction();
        }

        // Resetear regeneración
        allowRegeneration = false;
        intervalToRegenerateAfterDamage = 0;

        // Parar regeneración si estaba activa
        if (isRegenerating)
        {
            TimeManager.Instance.Unregister(1, Regenerate);
            isRegenerating = false;
        }

        // Empezar contador para poder regenerar
        TimeManager.Instance.Register(1, AllowToRegenerate);
    }

    public void AllowToRegenerate()
    {
        if (allowRegeneration) return;

        if (intervalToRegenerateAfterDamage >= MAX_INTERVAL)
        {
            allowRegeneration = true;

            TimeManager.Instance.Unregister(1, AllowToRegenerate);
            TimeManager.Instance.Register(1, Regenerate);
        }
        else
        {
            intervalToRegenerateAfterDamage++;
        }
    }

    public void Regenerate()
    {
        if (moundHealthPoints >= maxHealthByUpgrade[currentLevel])
        {
            moundHealthPoints = maxHealthByUpgrade[currentLevel];

            TimeManager.Instance.Unregister(1, Regenerate);
            isRegenerating = false;
            allowRegeneration = false;

            return;
        }

        isRegenerating = true;

        int healthPointsRegeneration =
            healthRegenPowerByUpgrade[currentLevel] * maxHealthByUpgrade[currentLevel] / 100;

        moundHealthPoints += healthPointsRegeneration;

        // Clamp para no pasarse
        if (moundHealthPoints > maxHealthByUpgrade[currentLevel])
            moundHealthPoints = maxHealthByUpgrade[currentLevel];
    }

    void MoundDestruction()
    {
        isDead = true;
        Debug.Log("AUAUAU me muero deberia morirme porfavor poned el codigo para que me muera quiero morir ahora matadme no requiero vivir terminad con mi sufrimiento AAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }
}
