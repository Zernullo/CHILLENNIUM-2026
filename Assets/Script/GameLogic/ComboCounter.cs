using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    public static ComboCounter Instance;

    [Header("UI")]
    public TextMeshProUGUI comboText;

    [Header("Settings")]
    public int comboCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterHit()
    {
        comboCount++;
        UpdateUI();
    }

    public void ResetCombo()
    {
        if (comboCount == 0) return; // avoid redundant resets
        comboCount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (comboText == null) return;
        comboText.text = $"Combo x{comboCount}";
    }
}