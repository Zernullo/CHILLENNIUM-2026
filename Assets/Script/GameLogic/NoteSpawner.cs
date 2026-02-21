using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    [Header("Spawn Timing")]
    [Min(1f)] public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;

    public float noteSpeed = 0.4f;

    [Header("Appearance")]
    public float yOffset = 0.1f;

    private float timer = 0f;
    private float nextSpawnInterval = 1f;

    private void Start()
    {
        RollNextSpawnInterval();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnInterval)
        {
            SpawnNote();
            timer = 0f;
            RollNextSpawnInterval();
        }
    }

    private void RollNextSpawnInterval()
    {
        float min = Mathf.Max(1f, minSpawnInterval);
        float max = Mathf.Max(min, maxSpawnInterval);
        nextSpawnInterval = Random.Range(min, max);
    }

    // Lanes for notes (set in inspector or auto-detect)
    [Header("Lanes")]
    public Transform[] lanes; // Assign lane positions in inspector

    void SpawnNote()
    {
        // Safety: check lanes array
        if (lanes == null || lanes.Length == 0)
        {
            SpawnSingleNote(spawnPoint.position);
            return;
        }

        // Filter out null lanes
        System.Collections.Generic.List<Transform> validLanes = new System.Collections.Generic.List<Transform>();
        foreach (var lane in lanes)
        {
            if (lane != null) validLanes.Add(lane);
        }
        if (validLanes.Count == 0)
        {
            SpawnSingleNote(spawnPoint.position);
            return;
        }

        int patternType = Random.Range(0, 3); // 0: single, 1: burst, 2: wave
        if (patternType == 0)
        {
            // Single note in random lane
            int laneIdx = Random.Range(0, validLanes.Count);
            SpawnSingleNote(validLanes[laneIdx].position);
        }
        else if (patternType == 1)
        {
            // Burst: 2-3 notes in random lanes
            int burstCount = Random.Range(2, Mathf.Min(4, validLanes.Count + 1));
            System.Collections.Generic.HashSet<int> used = new System.Collections.Generic.HashSet<int>();
            int attempts = 0;
            for (int i = 0; i < burstCount && attempts < 10 * burstCount; i++)
            {
                int laneIdx;
                do {
                    laneIdx = Random.Range(0, validLanes.Count);
                    attempts++;
                } while (used.Contains(laneIdx) && attempts < 10 * burstCount);
                if (!used.Contains(laneIdx))
                {
                    used.Add(laneIdx);
                    SpawnSingleNote(validLanes[laneIdx].position);
                }
            }
        }
        else
        {
            // Wave: notes in all lanes
            foreach (var lane in validLanes)
            {
                SpawnSingleNote(lane.position);
            }
        }
    }

    void SpawnSingleNote(Vector3 lanePos)
    {
        // Use lane x, spawnPoint y/z
        Vector3 spawnPos = new Vector3(
            lanePos.x,
            spawnPoint.position.y + yOffset,
            spawnPoint.position.z
        );
        GameObject note = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        NoteMover mover = note.GetComponent<NoteMover>();
        if (mover != null)
        {
            mover.target = hitZone;
            mover.speed = noteSpeed;
        }
    }
}