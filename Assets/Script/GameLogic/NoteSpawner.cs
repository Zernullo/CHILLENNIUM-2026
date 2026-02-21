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

    void SpawnNote()
    {
        Vector3 spawnPos = spawnPoint.position + new Vector3(0, yOffset, 0);
        GameObject note = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        NoteMover mover = note.GetComponent<NoteMover>();
        if (mover != null)
        {
            mover.target = hitZone;
            mover.speed = noteSpeed;
        }
    }
}