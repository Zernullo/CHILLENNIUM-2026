using UnityEngine;
using UnityEngine.InputSystem;

public class DebuffKey : MonoBehaviour
{
    public BlindDebuff blindScript;

    [Header("Speed Boost - S Key")]
    public float boostedSpeedMultiplier = 2f;
    public float boostDuration = 3f;

    [Header("Speed Slow - D Key")]
    public float slowedSpeedMultiplier = 0.5f;
    public float slowDuration = 3f;

    public float normalSpeedMultiplier = 1f;

    private float boostTimer = 0f;
    private float slowTimer = 0f;

    private void Update()
    {
        if (Keyboard.current == null) return;

        // --- 'A' blind debuff ---
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            if (blindScript != null)
                blindScript.ActivateFlash();
            else
                Debug.LogWarning("DebuffKey: No BlindDebuff script assigned in the Inspector!");
        }

        // --- 'S' speed boost ---
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            boostTimer = boostDuration;
            slowTimer = 0f; // cancel any active slow
        }

        // --- 'D' speed slow ---
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            slowTimer = slowDuration;
            boostTimer = 0f; // cancel any active boost
        }

        // --- Apply whichever effect is active (boost takes priority if somehow both are running) ---
        if (boostTimer > 0f)
        {
            NoteMover.SpeedMultiplier = boostedSpeedMultiplier;
            boostTimer -= Time.deltaTime;
        }
        else if (slowTimer > 0f)
        {
            NoteMover.SpeedMultiplier = slowedSpeedMultiplier;
            slowTimer -= Time.deltaTime;
        }
        else
        {
            NoteMover.SpeedMultiplier = normalSpeedMultiplier;
        }
    }
}