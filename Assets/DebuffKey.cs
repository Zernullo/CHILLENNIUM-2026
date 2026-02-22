using UnityEngine;

public class DebuffKey : MonoBehaviour
{
    public InvertKey invertKey; 
    public BlindDebuff blindScript;
    public InvertWarning invertWarning;

    [Header("Speed Boost")]
    public float boostedSpeedMultiplier = 2f;
    public float boostDuration = 3f;

    [Header("Normal Speed")]
    public float normalSpeedMultiplier = 1f;

    [Header("Cooldowns")]
    public float blindCooldown = 10f;
    public float boostCooldown = 10f;
    public float invertCooldown = 15f;

    private float boostTimer = 0f;
    private float blindCooldownTimer = 0f;
    private float boostCooldownTimer = 0f;
    private float invertCooldownTimer = 0f;
    private bool boostWasActive = false;

    // Public read-only for UI and BuffManager
    public float BlindCooldownTimer => blindCooldownTimer;
    public float BoostCooldownTimer => boostCooldownTimer;
    public float InvertCooldownTimer => invertCooldownTimer;
    public float BoostTimer => boostTimer;
    public float BoostedSpeedMultiplier => boostedSpeedMultiplier;
    public float NormalSpeedMultiplier => normalSpeedMultiplier;
    public bool BoostWasActive { get => boostWasActive; set => boostWasActive = value; }

    private void Update()
    {
        if (blindCooldownTimer > 0f) blindCooldownTimer -= Time.deltaTime;
        if (boostCooldownTimer > 0f) boostCooldownTimer -= Time.deltaTime;
        if (invertCooldownTimer > 0f) invertCooldownTimer -= Time.deltaTime;

        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            boostWasActive = true;
        }
        else
        {
            if (boostWasActive)
            {
                boostCooldownTimer = boostCooldown;
                boostWasActive = false;
            }
        }
    }

    public void TriggerDamageDebuff()
    {
        if (boostTimer > 0f || invertCooldownTimer > 0f || blindCooldownTimer > 0f)
        {
            Debug.Log("Debuff already active!");
            return;
        }

        System.Collections.Generic.List<int> available = new System.Collections.Generic.List<int>();
        available.Add(0);
        available.Add(1);
        available.Add(2);

        int roll = available[Random.Range(0, available.Count)];

        switch (roll)
        {
            case 0:
                if (blindScript != null)
                {
                    blindScript.ActivateFlash();
                    blindCooldownTimer = blindCooldown;
                    Debug.Log("<color=white>Debuff: Blind</color>");
                }
                break;
            case 1:
                if (invertWarning != null)
                {
                    invertWarning.StartCountdown();
                    invertCooldownTimer = invertCooldown;
                    Debug.Log("<color=yellow>Debuff: Invert</color>");
                }
                break;
            case 2:
                boostTimer = boostDuration;
                boostWasActive = false;
                Debug.Log("<color=red>Debuff: Speed Boost</color>");
                break;
        }
    }

    public void ClearAllDebuffs()
{
    blindCooldownTimer = 0f;
    boostCooldownTimer = 0f;
    invertCooldownTimer = 0f;
    boostTimer = 0f;
    boostWasActive = false;

    // Revert keys if currently inverted
    if (invertWarning != null)
    {
        invertWarning.StopAll();
       
        if (invertKey != null && invertKey.IsInverted)
            invertKey.TriggerInvert();
    }

    Debug.Log("<color=white>All debuffs cleansed!</color>");
}
}