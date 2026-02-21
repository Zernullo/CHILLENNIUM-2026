using UnityEngine;

/// <summary>
/// Tracks consecutive hits across all HitZones and triggers SpecialAttack.
/// Attach to the same GameObject as SpecialAttack.
/// </summary>
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
        }
    }

    public void RegisterMiss()
    {
        if (specialAttack.IsSpecialActive) return;
        consecutiveHits = 0;
    }
}