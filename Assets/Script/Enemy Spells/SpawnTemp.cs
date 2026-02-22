using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnTemp : MonoBehaviour
{
    public FlashSpell flashScript; 
    public NoteSpawner spawnerScript;
    private void Update()
    {
        // temp a key for flashbang
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            flashScript.ActivateFlash();
        }

        //temp s key for fast
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            spawnerScript.ActivateSpeedUp();
        }

        //temp d key for slow
        
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            spawnerScript.ActivateSlowDown();
        }
    }
}