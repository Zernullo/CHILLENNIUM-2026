using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SpecialAttack : MonoBehaviour
{
    [Header("Trigger")]
    public int consecutiveHitsRequired = 10;

    [Header("Special Attack Settings")]
    public float duration = 15f;
    public float speedMultiplier = 2f;
    public float intervalDivisor = 2f;
    public TextMeshProUGUI specialAttackText;

    private bool isSpecialActive = false;
    public bool IsSpecialActive => isSpecialActive;

    public void TriggerSpecial(NoteSpawner[] spawners)
    {
        if (isSpecialActive) return;
        StartCoroutine(RunSpecialAttack(spawners));
    }

    private IEnumerator RunSpecialAttack(NoteSpawner[] spawners)
    {
        isSpecialActive = true;
        Debug.Log("<color=yellow>⚡ SPECIAL ATTACK INCOMING</color>");

        // Show special attack text
        if (specialAttackText != null)
        {
            specialAttackText.text = "SPECIAL ATTACK!";
            specialAttackText.gameObject.SetActive(true);
        }

        // Pause all normal spawners immediately
        foreach (var s in spawners)
            if (s != null) s.SetPaused(true);

        // Wait until all notes currently on screen are gone
        yield return new WaitUntil(() => Object.FindAnyObjectByType<NoteMover>() == null);

        Debug.Log("<color=yellow>⚡ SPECIAL ATTACK START</color>");

        // Save originals and apply harder settings
        float[] originalMinIntervals = new float[spawners.Length];
        float[] originalMaxIntervals = new float[spawners.Length];
        float[] originalSpeeds = new float[spawners.Length];

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] == null) continue;
            originalMinIntervals[i] = spawners[i].minSpawnInterval;
            originalMaxIntervals[i] = spawners[i].maxSpawnInterval;
            originalSpeeds[i] = spawners[i].noteSpeed;

            spawners[i].minSpawnInterval = Mathf.Max(0.1f, spawners[i].minSpawnInterval / intervalDivisor);
            spawners[i].maxSpawnInterval = Mathf.Max(0.2f, spawners[i].maxSpawnInterval / intervalDivisor);
            spawners[i].noteSpeed = spawners[i].noteSpeed * speedMultiplier;
        }

        // Resume spawners with harder settings
        foreach (var s in spawners)
            if (s != null) s.SetPaused(false);

        yield return new WaitForSeconds(duration);

        // Pause again to cleanly end the special phase
        foreach (var s in spawners)
            if (s != null) s.SetPaused(true);

        // Wait for special notes to clear too
        yield return new WaitUntil(() => Object.FindAnyObjectByType<NoteMover>() == null);

        // Restore original values
        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] == null) continue;
            spawners[i].minSpawnInterval = originalMinIntervals[i];
            spawners[i].maxSpawnInterval = originalMaxIntervals[i];
            spawners[i].noteSpeed = originalSpeeds[i];
        }

        // Resume normal spawners
        foreach (var s in spawners)
            if (s != null) s.SetPaused(false);

        isSpecialActive = false;
        Debug.Log("<color=cyan>⚡ SPECIAL ATTACK END — Back to normal</color>");

        // Hide special attack text
        if (specialAttackText != null)
        {
            specialAttackText.gameObject.SetActive(false);
        }
    }
}