using UnityEngine;
using UnityEngine.InputSystem; // You need this at the top!

public class SpawnTemp : MonoBehaviour
{
    public FlashSpell flashScript; 

    private void Update()
    {
        // This is the "New Input System" way to check a key
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            flashScript.ActivateFlash();
        }
    }
}