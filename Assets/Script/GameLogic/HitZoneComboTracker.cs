using UnityEngine;

public class HitZoneComboTracker : MonoBehaviour
{
    public SpecialAttack specialAttack;
    public NoteSpawner[] spawners;

    private int consecutiveHits = 0;

    public void RegisterHit()
    {
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits++;
        if (consecutiveHits >= specialAttack.consecutiveHitsRequired)
        {
            consecutiveHits = 0;
            specialAttack.TriggerSpecial(spawners);
            CharacterAnimationController.Instance?.TriggerSpecialAttackAnim();
        }
    }

    public void RegisterPerfectHit()
    {
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits += 2; // perfect hits count double toward special
        if (consecutiveHits >= specialAttack.consecutiveHitsRequired)
        {
            consecutiveHits = 0;
            specialAttack.TriggerSpecial(spawners);
        }
    }

    public void RegisterMiss()
    {
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits = 0;
    }

}