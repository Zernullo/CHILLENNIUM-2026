using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Player's Health")]
    public int playerMaxHealth = 1500;
    public int playerCurrentHealth;
    public Slider playerHealthSlider;
    public TextMeshProUGUI healthText;

    [Header("Boss's Health")]
    public int bossMaxHealth = 500;
    public int bossCurrentHealth;
    public Slider bossHealthSlider;
    public TextMeshProUGUI bossHealthText;

    public static Health Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerCurrentHealth = playerMaxHealth;
        bossCurrentHealth = bossMaxHealth;

        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = playerMaxHealth;
            playerHealthSlider.value = playerMaxHealth;
        }

        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = bossMaxHealth;
            bossHealthSlider.value = bossMaxHealth;
        }

        UpdateHealthTexts();
    }

    public void DamagePlayer(int amount)
    {
        playerCurrentHealth = Mathf.Max(0, playerCurrentHealth - amount);
        if (playerHealthSlider != null)
            playerHealthSlider.value = playerCurrentHealth;
        if (healthText != null)
            healthText.text = $"{playerCurrentHealth} / {playerMaxHealth}";
        if (playerCurrentHealth <= 0)
            Debug.Log("Player is dead!");
    }

    public void DamageBoss(int amount)
    {
        bossCurrentHealth = Mathf.Max(0, bossCurrentHealth - amount);
        if (bossHealthSlider != null)
            bossHealthSlider.value = bossCurrentHealth;
        if (bossHealthText != null)
            bossHealthText.text = $"{bossCurrentHealth} / {bossMaxHealth}";
        if (bossCurrentHealth <= 0)
            Debug.Log("Boss is dead!");
    }

    public void UpdateHealthTexts()
    {
        if (healthText != null)
            healthText.text = $"{playerCurrentHealth} / {playerMaxHealth}";
        if (bossHealthText != null)
            bossHealthText.text = $"{bossCurrentHealth} / {bossMaxHealth}";
    }
}