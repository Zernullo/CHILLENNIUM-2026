using UnityEngine;

public class BossAnimController : MonoBehaviour
{
    public static BossAnimController Instance;
    private Animator animator;
    private int missCount = 0;
    private const int missesForSpecial = 5;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void OnPlayerMiss()
    {
        missCount++;

        if (missCount >= missesForSpecial)
        {
            missCount = 0;
            TriggerSpecialAttack();
        }
        else
        {
            TriggerRandomAttack();
        }
    }
    private void TriggerRandomAttack()
    {
        int randomAttack = Random.Range(1, 5); // picks 1, 2, 3, or 4
        animator.SetTrigger("Attack" + randomAttack);
    }
    private void TriggerSpecialAttack()
    {
        animator.SetTrigger("SpecialAttack");
    }
}
