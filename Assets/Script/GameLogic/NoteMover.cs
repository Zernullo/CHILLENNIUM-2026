using UnityEngine;

public class NoteMover : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public float speed = 5f;
    public float destroyPastDistance = 5f;

    private Vector3 moveDirection;
    private bool directionSet = false;
    private bool missRegistered = false;
    private Renderer noteRenderer;

    public static float SpeedMultiplier = 1f;

    void Start()
    {
        noteRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (target == null) return;

        if (!directionSet)
        {
            moveDirection = (target.position - transform.position).normalized;
            directionSet = true;
        }

        transform.position += moveDirection * speed * SpeedMultiplier * Time.deltaTime;

        float distancePast = Vector3.Dot(
            transform.position - target.position,
            moveDirection
        );

        // Register miss and damage player ONCE when note passes
        if (distancePast > 0f && !missRegistered)
        {
            missRegistered = true;
            HitZone hz = target.GetComponent<HitZone>();
            if (hz != null)
            {
                Debug.Log($"<color=red>Missed note!</color> passed {target.name}");
            }
            ComboCounter.Instance?.ResetCombo();
            PerfectComboCounter.Instance?.ResetPerfectCombo();    // ← add this
            Health.Instance?.DamagePlayer(20);
            BossAnimController.Instance?.OnPlayerMiss();
        }

        // Fade out after passing
        if (distancePast > 0f && noteRenderer != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, distancePast / destroyPastDistance);
            Color c = noteRenderer.material.color;
            noteRenderer.material.color = new Color(c.r, c.g, c.b, alpha);
        }

        if (distancePast >= destroyPastDistance)
            Destroy(gameObject);
    }
}