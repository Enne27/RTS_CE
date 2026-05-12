using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private TextMeshProUGUI timer_TMP;

    private float timeRemaining;
    private bool isRunning;
    #endregion

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        isRunning = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            isRunning = false;
            gameObject.SetActive(false);
            return;
        }

        int mins = Mathf.FloorToInt(timeRemaining / 60);
        int secs = Mathf.FloorToInt(timeRemaining % 60);

        timer_TMP.text = string.Format("{0:D2}:{1:D2}", mins, secs);
    }
}
