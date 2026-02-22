using UnityEngine;
using System.Collections;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public Health health;
    public DebuffKey debuffKey;

    [Header("Buff Durations")]
    public float slowDuration = 3f;
    public float damageDuration = 3f;
    public float healDuration = 10f;
    public float missImmunityDuration = 5f;

    [Header("Buff Values")]
    public float slowMultiplier = 0.5f;
    public float damageMultiplier = 1.5f;
    public float healAmount = 1f;

    [Header("Cooldowns")]
    public float slowCooldown = 15f;
    public float damageCooldown = 15f;
    public float healCooldown = 15f;
    public float cleanseCooldown = 20f;
    public float missImmunityCooldown = 20f;

    private float slowTimer = 0f;
    private float damageTimer = 0f;
    private float missImmunityTimer = 0f;

    private float slowCooldownTimer = 0f;
    private float damageCooldownTimer = 0f;
    private float healCooldownTimer = 0f;
    private float cleanseCooldownTimer = 0f;
    private float missImmunityCooldownTimer = 0f;

    // Public read-only for UI
    public float SlowCooldownTimer => slowCooldownTimer;
    public float DamageCooldownTimer => damageCooldownTimer;
    public float HealCooldownTimer => healCooldownTimer;
    public float CleanseCooldownTimer => cleanseCooldownTimer;
    public float MissImmunityCooldownTimer => missImmunityCooldownTimer;

    public bool IsSlowActive => slowTimer > 0f;
    public bool IsMissImmune => missImmunityTimer > 0f;
    public bool IsDamageBuffed => damageTimer > 0f;
    public float DamageMultiplier => IsDamageBuffed ? damageMultiplier : 1f;

    private void Awake()
    {
        Instance = this;
        Debug.Log("BuffManager Instance set");
    }

    private void Update()
    {
        if (slowCooldownTimer > 0f) slowCooldownTimer -= Time.deltaTime;
        if (damageCooldownTimer > 0f) damageCooldownTimer -= Time.deltaTime;
        if (healCooldownTimer > 0f) healCooldownTimer -= Time.deltaTime;
        if (cleanseCooldownTimer > 0f) cleanseCooldownTimer -= Time.deltaTime;
        if (missImmunityCooldownTimer > 0f) missImmunityCooldownTimer -= Time.deltaTime;

        if (damageTimer > 0f) damageTimer -= Time.deltaTime;
        if (missImmunityTimer > 0f) missImmunityTimer -= Time.deltaTime;

        // Centralized speed control
        if (debuffKey != null && debuffKey.BoostTimer > 0f)
        {
            NoteMover.SpeedMultiplier = debuffKey.BoostedSpeedMultiplier;
        }
        else if (slowTimer > 0f)
        {
            NoteMover.SpeedMultiplier = slowMultiplier;
            slowTimer -= Time.deltaTime;
        }
        else
        {
            NoteMover.SpeedMultiplier = debuffKey != null ? debuffKey.NormalSpeedMultiplier : 1f;
            if (debuffKey != null && debuffKey.BoostWasActive)
                debuffKey.BoostWasActive = false;
        }
    }

    public void ActivateBuff(BuffType buffType)
    {
        Debug.Log($"ActivateBuff called with: {buffType}");
        switch (buffType)
        {
            case BuffType.SlowNotes:
                if (slowCooldownTimer > 0f) { Debug.Log("Slow on cooldown!"); return; }
                slowTimer = slowDuration;
                slowCooldownTimer = slowCooldown;
                Debug.Log("<color=cyan>Buff: Slow Notes</color>");
                break;

            case BuffType.DamageBoost:
                if (damageCooldownTimer > 0f) { Debug.Log("Damage boost on cooldown!"); return; }
                damageTimer = damageDuration;
                damageCooldownTimer = damageCooldown;
                Debug.Log("<color=orange>Buff: Damage Boost</color>");
                break;

            case BuffType.Heal:
                if (healCooldownTimer > 0f) { Debug.Log("Heal on cooldown!"); return; }
                StartCoroutine(HealOverTime());
                healCooldownTimer = healCooldown;
                Debug.Log("<color=green>Buff: Heal</color>");
                break;

            case BuffType.Cleanse:
                if (cleanseCooldownTimer > 0f) { Debug.Log("Cleanse on cooldown!"); return; }
                CleanseDebuffs();
                cleanseCooldownTimer = cleanseCooldown;
                Debug.Log("<color=white>Buff: Cleanse</color>");
                break;

            case BuffType.MissImmunity:
                if (missImmunityCooldownTimer > 0f) { Debug.Log("Miss immunity on cooldown!"); return; }
                missImmunityTimer = missImmunityDuration;
                missImmunityCooldownTimer = missImmunityCooldown;
                Debug.Log("<color=yellow>Buff: Miss Immunity</color>");
                break;

            case BuffType.None:
                Debug.LogWarning("BuffType is None — did you forget to set it on the ItemData?");
                break;
        }
    }

    private IEnumerator HealOverTime()
    {
        float elapsed = 0f;
        while (elapsed < healDuration)
        {
            elapsed += Time.deltaTime;
            health.HealPlayer(healAmount * Time.deltaTime);
            yield return null;
        }
    }

    private void CleanseDebuffs()
    {
        slowTimer = 0f;
        if (debuffKey != null)
            debuffKey.ClearAllDebuffs();
    }
}