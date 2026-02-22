using TMPro;
using UnityEngine;

public class PerfectComboCounter : MonoBehaviour
{
    public static PerfectComboCounter Instance;

    [Header("UI")]
    public TextMeshProUGUI perfectComboText;

    [Header("State")]
    public int perfectComboCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPerfectHit()
    {
        perfectComboCount++;
        UpdateUI();
    }

    public void ResetPerfectCombo()
    {
        if (perfectComboCount == 0) return;
        perfectComboCount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (perfectComboText == null) return;

        if (perfectComboCount <= 0)
            perfectComboText.text = "";
        else if (perfectComboCount == 1)
            perfectComboText.text = "Perfect x1";
        else
            perfectComboText.text = $"Perfect x{perfectComboCount}";
    }
}