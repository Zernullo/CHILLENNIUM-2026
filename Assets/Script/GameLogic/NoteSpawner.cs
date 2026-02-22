using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [Header("Speed Up Settings")]
public float SpeedupMultiplier = 1.5f;
public float SpeedupDuration = 2.0f;
public float currentSpeedMultiplier = 1f; 

 [Header("Slow Down Settings")]
public float SlowdownMultiplier = 0.5f;
public float SlowdownDuration = 2.0f;


public void ActivateSpeedUp()
{
    StopCoroutine(nameof(SpeedUpRoutine));
    StartCoroutine(nameof(SpeedUpRoutine));
}

private System.Collections.IEnumerator SpeedUpRoutine()
{
    currentSpeedMultiplier = SpeedupMultiplier;
    yield return new WaitForSeconds(SpeedupDuration);
    currentSpeedMultiplier = 1f;
}

public void ActivateSlowDown()
{
    StopCoroutine(nameof(SlowDownRoutine));
    StartCoroutine(nameof(SlowDownRoutine));
}

private System.Collections.IEnumerator SlowDownRoutine()
{
    currentSpeedMultiplier = SlowdownMultiplier;
    yield return new WaitForSeconds(SlowdownDuration);
    currentSpeedMultiplier = 1f;
}

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
        Vector3 spawnPos = new Vector3(lanePos.x, spawnPoint.position.y + yOffset, spawnPoint.position.z);
    GameObject note = Instantiate(notePrefab, spawnPos, Quaternion.identity);
    NoteMover mover = note.GetComponent<NoteMover>();

    if (mover != null)
    {
        // Instead of the general hitZone, give it the lane's specific X position
        // so it moves straight down the lane.
        mover.targetPosition = new Vector3(lanePos.x, hitZone.position.y, hitZone.position.z);
        mover.speed = noteSpeed;
    }
    }
}