using UnityEngine;

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
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        AntExlporer.OnExplorationCompleted += AddExplorationProgress;
    }

    private void OnDisable()
    {
        AntExlporer.OnExplorationCompleted -= AddExplorationProgress;
    }

    private void AddExplorationProgress()
    {
        explorationProgress++;

        Debug.Log("Progreso exploración: " + explorationProgress);

        EraManager.instance.AddProgress(RequirementID.EXPLORATION, 1);
    }
}
