using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseCondition : MonoBehaviour
{
    [Header("References")]
    public Health health;

    [Header("Scenes")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("Settings")]
    public int damagePerMiss = 10;
    public int damagePerHit = 10;

    public static WinLoseCondition Instance;

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (gameOver) return;

        if (health.playerCurrentHealth <= 0)
        {
            gameOver = true;
            Debug.Log("<color=red>Player Lost!</color>");
            SceneManager.LoadScene(loseScene);
        }

        if (health.bossCurrentHealth <= 0)
        {
            gameOver = true;
            Debug.Log("<color=green>Player Won!</color>");
            SceneManager.LoadScene(winScene);
        }
    }

    public void OnMiss()
    {
        if (gameOver) return;
        health.DamagePlayer(damagePerMiss);
    }

    public void OnHit()
    {
        if (gameOver) return;
        health.DamageBoss(damagePerHit);
    }
}