using UnityEngine;
using TMPro;

public class HitZoneComboTracker : MonoBehaviour
{
    public SpecialAttack specialAttack;
    public NoteSpawner[] spawners;

    [Header("Combo UI")]
    public TextMeshProUGUI comboText;

    [Header("Win/Lose")]
    public WinLoseCondition winLoseCondition;

    private int consecutiveHits = 0;

    public void RegisterHit()
    {
        winLoseCondition?.OnHit();
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits++;
        UpdateComboText();
        if (consecutiveHits >= specialAttack.consecutiveHitsRequired)
        {
            consecutiveHits = 0;
            UpdateComboText();
            specialAttack.TriggerSpecial(spawners);
        }
    }

    public void RegisterPerfectHit()
    {
        winLoseCondition?.OnHit();
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits += 2;
        UpdateComboText();
        if (consecutiveHits >= specialAttack.consecutiveHitsRequired)
        {
            consecutiveHits = 0;
            UpdateComboText();
            specialAttack.TriggerSpecial(spawners);
            CharacterAnimationController.Instance?.TriggerSpecialAttackAnim();
        }
    }

    public void RegisterMiss()
    {
        winLoseCondition?.OnMiss();
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits = 0;
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        if (comboText == null) return;

        if (consecutiveHits <= 0)
        {
            comboText.text = "";
            return;
        }

        comboText.text = $"Combo x{consecutiveHits}";

        if (consecutiveHits >= 8)
            comboText.color = Color.red;
        else if (consecutiveHits >= 5)
            comboText.color = Color.yellow;
        else
            comboText.color = Color.white;
    }
}