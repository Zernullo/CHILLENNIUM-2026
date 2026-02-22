using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

public class InvertKey : MonoBehaviour
{
    public Key[] keyToInvert = new Key[] { Key.Q, Key.W, Key.E, Key.R };

    [Header("References")]
    public InvertWarning invertWarning;

    private bool isInverted = false;

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current[Key.Space].wasPressedThisFrame) return;

        if (!isInverted)
        {
            invertWarning?.StartCountdown();
        }
        else
        {
            // Manual revert — stop everything and revert
            TriggerInvert();
            invertWarning?.StopAll();
        }
    }

    public void TriggerInvert()
    {
        HitZone[] hitZones = FindObjectsByType<HitZone>(FindObjectsSortMode.None);

        Dictionary<HitZone, Key> pendingSwaps = new Dictionary<HitZone, Key>();
        foreach (var hz in hitZones)
        {
            if (keyToInvert.Contains(hz.keyToPress) || GetInvertedKey(hz.keyToPress) != hz.keyToPress)
                pendingSwaps[hz] = GetInvertedKey(hz.keyToPress);
        }

        foreach (var kvp in pendingSwaps)
        {
            Key original = kvp.Key.keyToPress;
            kvp.Key.UpdateKey(kvp.Value);
            Debug.Log($"Inverted <color=green>{original}</color> to <color=red>{kvp.Value}</color> for HitZone '{kvp.Key.name}'");
        }

        isInverted = !isInverted;
        Debug.Log(isInverted ? "<color=yellow>Keys Inverted</color>" : "<color=white>Keys Restored</color>");
    }

    private Key GetInvertedKey(Key key)
    {
        return key switch
        {
            Key.Q => Key.R,
            Key.R => Key.Q,
            Key.W => Key.E,
            Key.E => Key.W,
            _ => key
        };
    }
}