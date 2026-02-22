using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAnimationController : MonoBehaviour
{
    public static CharacterAnimationController Instance;
    private Animator animator;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) animator.SetTrigger("Q");
        else if (Keyboard.current.wKey.wasPressedThisFrame) animator.SetTrigger("W");
        else if (Keyboard.current.eKey.wasPressedThisFrame) animator.SetTrigger("E");
        else if (Keyboard.current.rKey.wasPressedThisFrame) animator.SetTrigger("R");
        else if (Keyboard.current.tKey.wasPressedThisFrame) animator.SetTrigger("T");
    }

    public void TriggerSpecialAttackAnim()
    {
        animator.SetTrigger("SpecialAttack");
        Health.Instance?.DamageBoss(50);
    }
}
