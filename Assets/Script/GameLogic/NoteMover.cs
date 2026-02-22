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

    public static float SpeedMultiplier = 1f; // <-- ADD THIS

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

        transform.position += moveDirection * speed * SpeedMultiplier * Time.deltaTime; // <-- MODIFIED

        float distancePast = Vector3.Dot(
            transform.position - target.position,
            moveDirection
        );

        if (distancePast > 0f && !missRegistered)
        {
            missRegistered = true;
            HitZone hz = target.GetComponent<HitZone>();
            if (hz != null)
            {
                hz.comboTracker?.RegisterMiss();
                Debug.Log($"<color=red>Missed note!</color> passed {target.name}");
            }
        }

        if (distancePast > 0f && noteRenderer != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, distancePast / destroyPastDistance);
            Color c = noteRenderer.material.color;
            noteRenderer.material.color = new Color(c.r, c.g, c.b, alpha);
        }

        if (distancePast >= destroyPastDistance){
            HitZone hitZone = target?.GetComponentInParent<HitZone>();
            if (hitZone == null)
                hitZone = target?.GetComponent<HitZone>();            
                hitZone?.comboTracker?.RegisterMiss();
                BossAnimController.Instance?.OnPlayerMiss();
                Destroy(gameObject);
                Health.Instance?.DamagePlayer(20);
        }
    }
}