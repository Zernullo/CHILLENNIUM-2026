using UnityEngine;

public class NoteMover : MonoBehaviour
{
    // 1. Change this from Transform to Vector3 to match the Spawner's new logic
    [HideInInspector] public Vector3 targetPosition; 
    [HideInInspector] public float speed = 5f;

    public float destroyPastDistance = 1f;

    private Vector3 moveDirection;
    private bool directionSet = false;
    private Renderer noteRenderer;
    
    private NoteSpawner spawner;

    void Start()
    {
        noteRenderer = GetComponent<Renderer>();
        spawner = Object.FindFirstObjectByType<NoteSpawner>();
    }

    void Update()
    {
        // 2. Updated check for the Vector3 target
        // Since Vector3 is a value type, we check if it's set or if we've reached it
        if (!directionSet)
        {
            // Calculate direction toward the specific lane coordinate
            Vector3 heading = targetPosition - transform.position;
            if (heading.sqrMagnitude > 0.001f)
            {
                moveDirection = heading.normalized;
                directionSet = true;
            }
            return; // Wait for direction to be set before moving
        }

        float multiplier = (spawner != null) ? spawner.currentSpeedMultiplier : 1f;
        float effectiveSpeed = speed * multiplier;

        transform.position += moveDirection * effectiveSpeed * Time.deltaTime;

        // 3. Distance check now uses targetPosition instead of target.position
        float distancePast = Vector3.Dot(
            transform.position - targetPosition,
            moveDirection
        );

        if (distancePast > 0)
        {
            if (noteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, distancePast / destroyPastDistance);
                Color c = noteRenderer.material.color;
                noteRenderer.material.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        if (distancePast >= destroyPastDistance)
        {
            Destroy(gameObject);
        }
    }
}