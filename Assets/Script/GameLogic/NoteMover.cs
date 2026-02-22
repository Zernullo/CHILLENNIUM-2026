using UnityEngine;

public class NoteMover : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public float speed = 5f;

    public float destroyPastDistance = 1f;

    private Vector3 moveDirection;
    private bool directionSet = false;
    private Renderer noteRenderer;

    void Start()
    {
        noteRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (target == null) return;

        if (!directionSet)
        {
            // Target the CENTER of the hit zone — note passes through it (half above, half below)
            moveDirection = (target.position - transform.position).normalized;
            directionSet = true;
        }

        transform.position += moveDirection * speed * Time.deltaTime;

        // Distance past the hit zone measured along movement direction
        float distancePast = Vector3.Dot(
            transform.position - target.position,
            moveDirection
        );

        if (distancePast > 0)
        {
            // Fade out after passing
            if (noteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, distancePast / destroyPastDistance);
                Color c = noteRenderer.material.color;
                noteRenderer.material.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        if (distancePast >= destroyPastDistance)
        {
            // Search target and its parents for HitZone component
            HitZone hitZone = target?.GetComponentInParent<HitZone>();
            if (hitZone == null)
                hitZone = target?.GetComponent<HitZone>();            
                hitZone?.comboTracker?.RegisterMiss();
                BossAnimController.Instance?.OnPlayerMiss();
                Destroy(gameObject);
        }

    }
}