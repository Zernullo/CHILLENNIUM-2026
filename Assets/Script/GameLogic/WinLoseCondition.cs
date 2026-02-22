using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseCondition : MonoBehaviour
{
    [Header("References")]
    public Health health;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    public static WinLoseCondition Instance;

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void Update()
    {
        if (gameOver) return;

        if (health.playerCurrentHealth <= 0)
        {
            gameOver = true;
            Time.timeScale = 0;
            Debug.Log("<color=red>Player Lost!</color>");
            if (losePanel != null) losePanel.SetActive(true);
        }

        if (health.bossCurrentHealth <= 0)
        {
            gameOver = true;
            Time.timeScale = 0;
            Debug.Log("<color=green>Player Won!</color>");
            if (winPanel != null) winPanel.SetActive(true);
        }
    }

    public void ResetRound()
    {
        gameOver = false;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void OnNextButton()
    {
        Time.timeScale = 1;
        winPanel.SetActive(false);
        RoundManager.Instance?.OnRoundWon();
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1;
        losePanel.SetActive(false);
        RoundManager.Instance?.ResetForNewRound();
    }

    public void OnHit()
    {
        if (gameOver) return;
        health.DamageBoss(RoundManager.Instance?.GetBossDamage() ?? 20);
    }

    public void OnMiss()
    {
        if (gameOver) return;
        health.DamagePlayer(20);
    }
}