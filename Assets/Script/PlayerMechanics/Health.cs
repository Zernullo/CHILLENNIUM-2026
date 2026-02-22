using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Player's Health")]
    public int playerMaxHealth = 1000;
    public int playerCurrentHealth;
    public Slider playerHealthSlider;

    [Header("Boss's Health")]
    public int bossMaxHealth = 1000;
    public int bossCurrentHealth;
    public Slider bossHealthSlider;

    [Header("Debuff on Damage")]
    public DebuffKey debuffKey; 
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
    }

    public void DamagePlayer(int amount)
    {
        playerCurrentHealth = Mathf.Max(0, playerCurrentHealth - amount);
        if (playerHealthSlider != null)
            playerHealthSlider.value = playerCurrentHealth;

        
        if (debuffKey != null)
            debuffKey.TriggerDamageDebuff();

        if (playerCurrentHealth <= 0)
            Debug.Log("Player is dead!");
    }

    public void HealPlayer(float amount)
{
    playerCurrentHealth = Mathf.Min(playerMaxHealth, playerCurrentHealth + (int)amount);
    if (playerHealthSlider != null)
        playerHealthSlider.value = playerCurrentHealth;
}

public void DamageBoss(int amount)
{
    float multiplier = BuffManager.Instance != null ? BuffManager.Instance.DamageMultiplier : 1f;
    int finalAmount = Mathf.RoundToInt(amount * multiplier);
    bossCurrentHealth = Mathf.Max(0, bossCurrentHealth - finalAmount);
    if (bossHealthSlider != null)
        bossHealthSlider.value = bossCurrentHealth;
    if (bossCurrentHealth <= 0)
        Debug.Log("Boss is dead!");
}
}