using System;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private float baseNoteSpeed;
    private float baseMinInterval;
    private float baseMaxInterval;

    [Header("Round Settings")]
    public int currentRound = 1;
    public TextMeshProUGUI roundText;

    [Header("Boss Scaling")]
    public int bossBaseHealth = 500;
    public float bossHealthIncreasePerRound = 1.5f;
    public int bossBaseDamage = 10;
    public int bossDamageIncreasePerRound = 5;

    [Header("References")]
    public Health health;
    public InventoryManager inventoryManager;
    public NoteSpawner[] spawners;

    [Header("Spawner Scaling")]
    public float speedIncreasePerRound = 0.5f;
    public float intervalDecreasePerRound = 0.1f;

    private int bossDamage;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateRoundText()
    {
        if (roundText != null) roundText.text = $"Round {currentRound}";
    }

    private void Start()
    {
        // Save base spawner values before any scaling
        if (spawners.Length > 0 && spawners[0] != null)
        {
            baseNoteSpeed = spawners[0].noteSpeed;
            baseMinInterval = spawners[0].minSpawnInterval;
            baseMaxInterval = spawners[0].maxSpawnInterval;
        }
        UpdateRoundText();
        ApplyRoundSettings();
    }

    public int GetBossDamage()
    {
        return bossDamage;
    }

    public void OnRoundWon()
    {
        currentRound++;
        Debug.Log($"<color=yellow>Round {currentRound} starting!</color>");
        inventoryManager?.OpenChoiceMenu();
    }

    public void OnShopClosed()
    {
        ApplyRoundSettings();

        // Boss health already updated in ApplyRoundSettings
        // Player health stays as is from previous round
        health?.UpdateHealthTexts();

        if (WinLoseCondition.Instance != null)
            WinLoseCondition.Instance.ResetRound();
    }

    private void ApplyRoundSettings()
    {
        // Scale boss health per round
        int newBossHealth = bossBaseHealth + (int)(bossBaseHealth * (bossHealthIncreasePerRound - 1f) * (currentRound - 1));

        // Scale boss damage per round
        bossDamage = bossBaseDamage + (bossDamageIncreasePerRound * (currentRound - 1));

        if (health != null)
        {
            health.bossMaxHealth = newBossHealth;
            health.bossCurrentHealth = newBossHealth;
            if (health.bossHealthSlider != null)
            {
                health.bossHealthSlider.maxValue = newBossHealth;
                health.bossHealthSlider.value = newBossHealth;
            }
        }

        // Scale spawner speed and interval
        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;
            spawner.noteSpeed = baseNoteSpeed + (speedIncreasePerRound * (currentRound - 1));
            spawner.minSpawnInterval = Mathf.Max(0.2f, baseMinInterval - (intervalDecreasePerRound * (currentRound - 1)));
            spawner.maxSpawnInterval = Mathf.Max(0.4f, baseMaxInterval - (intervalDecreasePerRound * (currentRound - 1)));
        }
        UpdateRoundText();
        Debug.Log($"<color=cyan>Round {currentRound} | Boss HP: {newBossHealth} | Boss Damage: {bossDamage}</color>");
    }

    public void ResetForNewRound()
    {
        currentRound = 1;

        if (health != null)
        {
            health.bossMaxHealth = bossBaseHealth;
            health.bossCurrentHealth = bossBaseHealth;
            if (health.bossHealthSlider != null)
            {
                health.bossHealthSlider.maxValue = bossBaseHealth;
                health.bossHealthSlider.value = bossBaseHealth;
            }

            // Reset player health on retry
            health.playerCurrentHealth = health.playerMaxHealth;
            if (health.playerHealthSlider != null)
                health.playerHealthSlider.value = health.playerMaxHealth;
        }

        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;
            spawner.noteSpeed = baseNoteSpeed;
            spawner.minSpawnInterval = baseMinInterval;
            spawner.maxSpawnInterval = baseMaxInterval;
        }

        bossDamage = bossBaseDamage;
        health?.UpdateHealthTexts();

        if (WinLoseCondition.Instance != null)
            WinLoseCondition.Instance.ResetRound();
        UpdateRoundText();
        Debug.Log("<color=white>Round reset to 1 — default stats restored</color>");
    }
}