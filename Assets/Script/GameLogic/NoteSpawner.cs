using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnPoint;
    public Transform hitZone;

    [Header("Spawn Timing")]
    [Min(1f)] public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;

    public float noteSpeed = 5f;

    [Header("Appearance")]
    public float yOffset = 0.1f;

    [Header("Lanes")]
    public Transform[] lanes;

    private float timer = 0f;
    private float nextSpawnInterval = 1f;
    private bool paused = false;

    private void Start()
    {
        RollNextSpawnInterval();
    }

    void Update()
    {
        if (paused) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawnInterval)
        {
            SpawnNote();
            timer = 0f;
            RollNextSpawnInterval();
        }
    }

    /// <summary>Called by SpecialAttack to pause/resume normal spawning.</summary>
    public void SetPaused(bool value)
    {
        paused = value;
        if (!paused) timer = 0f; // reset timer so we don't get an instant burst on resume
    }

    private void RollNextSpawnInterval()
    {
        float min = Mathf.Max(1f, minSpawnInterval);
        float max = Mathf.Max(min, maxSpawnInterval);
        nextSpawnInterval = Random.Range(min, max);
    }

    void SpawnNote()
    {
        if (lanes == null || lanes.Length == 0)
        {
            SpawnSingleNote(spawnPoint.position);
            return;
        }

        List<Transform> validLanes = new List<Transform>();
        foreach (var lane in lanes)
            if (lane != null) validLanes.Add(lane);

        if (validLanes.Count == 0)
        {
            SpawnSingleNote(spawnPoint.position);
            return;
        }

        int patternType = Random.Range(0, 3);

        if (patternType == 0)
        {
            SpawnSingleNote(validLanes[Random.Range(0, validLanes.Count)].position);
        }
        else if (patternType == 1)
        {
            int burstCount = Random.Range(2, Mathf.Min(4, validLanes.Count + 1));
            HashSet<int> used = new HashSet<int>();
            int attempts = 0;
            for (int i = 0; i < burstCount && attempts < 10 * burstCount; i++)
            {
                int laneIdx;
                do { laneIdx = Random.Range(0, validLanes.Count); attempts++; }
                while (used.Contains(laneIdx) && attempts < 10 * burstCount);

                if (!used.Contains(laneIdx))
                {
                    used.Add(laneIdx);
                    SpawnSingleNote(validLanes[laneIdx].position);
                }
            }
        }
        else
        {
            foreach (var lane in validLanes)
                SpawnSingleNote(lane.position);
        }
    }

    void SpawnSingleNote(Vector3 lanePos)
    {
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