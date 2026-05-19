using System.Collections;
using UnityEngine;
using static PlayerConstants;

public class ProgressManager : MonoBehaviour
{
    #region SINGLETON
    public static ProgressManager instance { get; private set; }

    #endregion

    #region VARIABLES

    private int explorationProgress = 0;

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        //else Destroy(gameObject);
    }

    private void OnEnable()
    {
        AntExlporer.OnExplorationCompleted -= AddExplorationProgress;
        AntExlporer.OnExplorationCompleted += AddExplorationProgress;

        StartCoroutine(SubscribeWhenReady());
    }

    /// <summary>
    /// EraManager no está listo antes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SubscribeWhenReady()
    {
        while (EraManager.instance == null)
            yield return null;

        AntExlporer.OnExplorationCompleted += EraManager.instance.UpgradeLimitsAndRefreshUpgradeButtonUI;
    }

    private void OnDisable()
    {
        AntExlporer.OnExplorationCompleted -= AddExplorationProgress;
        if (EraManager.instance != null)
            AntExlporer.OnExplorationCompleted -= EraManager.instance.UpgradeLimitsAndRefreshUpgradeButtonUI;
    }

    private void AddExplorationProgress()
    {
        explorationProgress++;

        //Debug.Log("Progreso exploración: " + explorationProgress);

        EraManager.instance.AddProgress(RequirementID.EXPLORATION);
    }

    public void RegisterAntCreation(ANT_TYPES ant)
    {
        EraManager.instance.AddProgress(RequirementID.ANT, 1);

        switch (ant)
        {
            case ANT_TYPES.ACID:
                EraManager.instance.AddProgress(RequirementID.ACID_ANT, 1);
                break;

            case ANT_TYPES.BERSERKER:
                EraManager.instance.AddProgress(RequirementID.BERSERKER_ANT, 1);
                break;

            case ANT_TYPES.EXPLORER:
                EraManager.instance.AddProgress(RequirementID.EXPLORER_ANT, 1);
                break;

            case ANT_TYPES.SOLDIER:
                EraManager.instance.AddProgress(RequirementID.SOLDIER_ANT, 1);
                break;

            case ANT_TYPES.CRAZY:
                EraManager.instance.AddProgress(RequirementID.CRAZY_ANT, 1);
                break;

            case ANT_TYPES.KAMIKAZE:
                EraManager.instance.AddProgress(RequirementID.KAMIKAZE_ANT, 1);
                break;

            case ANT_TYPES.WORKER:
                EraManager.instance.AddProgress(RequirementID.WORKER_ANT, 1);
                break;
        }

    }
}
