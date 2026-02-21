using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class HitZone : MonoBehaviour
{
    [Header("Input")]
    public Key keyToPress = Key.Q;

    [Header("Scoring")]
    public int scorePerHit = 10;
    public int score = 0;

    [Header("Timing Window")]
    public float maxHitDistance = 0.4f;

    [Header("Debug")]
    public bool debugLogs;

    private static readonly Dictionary<Key, HitZone> keyOwners = new Dictionary<Key, HitZone>();
    private static readonly Dictionary<Key, int> lastScoredFrameByKey = new Dictionary<Key, int>();
    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        if (keyOwners.TryGetValue(keyToPress, out HitZone owner) && owner != null && owner != this)
        {
            Debug.LogWarning($"HitZone '{name}' disabled because key {keyToPress} is already used by '{owner.name}'.");
            enabled = false;
            return;
        }

        keyOwners[keyToPress] = this;
    }

    private void OnDisable()
    {
        if (keyOwners.TryGetValue(keyToPress, out HitZone owner) && owner == this)
        {
            keyOwners.Remove(keyToPress);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        var keyControl = Keyboard.current[keyToPress];
        if (keyControl == null || !keyControl.wasPressedThisFrame)
            return;

        if (lastScoredFrameByKey.TryGetValue(keyToPress, out int lastFrame) && lastFrame == Time.frameCount)
            return;

        NoteMover note = FindNoteInsideZone();
        if (note == null)
        {
            if (debugLogs)
                Debug.Log($"{name} Miss! {keyToPress}");
            return;
        }

        if (debugLogs)
        {
            Debug.Log($"{name} Hit! {keyToPress} -> {note.name}");
        }

        Destroy(note.gameObject);
        score += scorePerHit;
        lastScoredFrameByKey[keyToPress] = Time.frameCount;
    }

    private NoteMover FindNoteInsideZone()
    {
        Vector3 center = zoneCollider.bounds.center;
        Vector3 halfExtents = zoneCollider.bounds.extents;
        float maxHitDistanceSqr = maxHitDistance * maxHitDistance;
        Collider[] overlaps = Physics.OverlapBox(
            center,
            halfExtents,
            zoneCollider.transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        NoteMover best = null;
        float bestSqrDistance = float.MaxValue;

        for (int i = 0; i < overlaps.Length; i++)
        {
            Collider candidate = overlaps[i];
            if (candidate == null || candidate == zoneCollider)
                continue;

            NoteMover mover = candidate.GetComponentInParent<NoteMover>();
            if (mover == null)
                continue;

            if (mover.target != transform)
                continue;

            float sqrDistance = (mover.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance > maxHitDistanceSqr)
                continue;

            if (sqrDistance < bestSqrDistance)
            {
                best = mover;
                bestSqrDistance = sqrDistance;
            }
        }

        return best;
    }
}