using UnityEngine;
using TMPro;

public class TimerWithTMPro : MonoBehaviour
{
    [Header("º∆ ±∆˜…Ë÷√")]
    public float totalTime = 60f;
    public TMP_Text timerText;

    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();
        StartTimer();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                UpdateTimerDisplay();
                TimerEnd();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Exact Time display
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            int milliseconds = Mathf.FloorToInt((currentTime * 1000) % 1000);

            timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
    }

    void TimerEnd()
    {
        isTimerRunning = false;
        UIManager.Instance.UI_EnableLeaderboard();
        Debug.Log("Times up");
    }

    [ContextMenu("StartTimer")]
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    [ContextMenu("StopTimer")]
    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    [ContextMenu("ResetTimer")]
    public void ResetTimer()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();
    }
}
