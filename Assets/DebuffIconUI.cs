using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebuffIconUI : MonoBehaviour
{
    [Header("Blind Icon")]
    public RawImage blindIcon;
    public TextMeshProUGUI blindTimerText;

    [Header("Invert Icon")]
    public RawImage invertIcon;
    public TextMeshProUGUI invertTimerText;

    [Header("Speed Up Icon")]
    public RawImage speedIcon;
    public TextMeshProUGUI speedTimerText;

    [Header("Settings")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private DebuffKey debuffKey;

    private void Start()
    {
        debuffKey = FindObjectOfType<DebuffKey>();
        SetAllInactive();
    }

    private void Update()
    {
        if (debuffKey == null) return;

        if (debuffKey == null) return;

    UpdateIcon(blindIcon, blindTimerText, debuffKey.BlindCooldownTimer);
    UpdateIcon(invertIcon, invertTimerText, debuffKey.InvertCooldownTimer);
    UpdateIcon(speedIcon, speedTimerText, debuffKey.BoostCooldownTimer);
    }

    private void UpdateIcon(RawImage icon, TextMeshProUGUI text, float timer)
    {
        if (icon == null) return;

        if (timer > 0f)
        {
            icon.color = activeColor;
            if (text != null)
            {
                text.gameObject.SetActive(true);
                text.text = Mathf.Ceil(timer).ToString("0");
            }
        }
        else
        {
            icon.color = inactiveColor;
            if (text != null)
                text.gameObject.SetActive(false);
        }
    }

    private void SetAllInactive()
    {
        if (blindIcon != null) blindIcon.color = inactiveColor;
        if (invertIcon != null) invertIcon.color = inactiveColor;
        if (speedIcon != null) speedIcon.color = inactiveColor;

        if (blindTimerText != null) blindTimerText.gameObject.SetActive(false);
        if (invertTimerText != null) invertTimerText.gameObject.SetActive(false);
        if (speedTimerText != null) speedTimerText.gameObject.SetActive(false);
    }
}