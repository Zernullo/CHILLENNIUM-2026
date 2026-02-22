using UnityEngine;
using TMPro;


public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    void UpdateTimerUI () {
        int minutes = Mathf.FloorToInt(elapsedTime/60F);
        int seconds = Mathf.FloorToInt(elapsedTime%60F);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}