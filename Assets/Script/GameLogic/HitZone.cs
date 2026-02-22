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
    public float hitWindowBefore = 0.3f;
    public float hitWindowAfter = 0.2f;

    [Header("Special Attack")]
    public HitZoneComboTracker comboTracker;

    [Header("Debug")]
    public bool debugLogs;

    private static readonly Dictionary<Key, HitZone> keyOwners = new Dictionary<Key, HitZone>();
    private static readonly Dictionary<Key, int> lastScoredFrameByKey = new Dictionary<Key, int>();
    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
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
            keyOwners.Remove(keyToPress);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        var keyControl = Keyboard.current[keyToPress];
        if (keyControl == null || !keyControl.wasPressedThisFrame) return;

        if (lastScoredFrameByKey.TryGetValue(keyToPress, out int lastFrame) && lastFrame == Time.frameCount) return;

        NoteMover note = FindNoteInsideZone();

        if (note == null)
        {
            if (debugLogs) Debug.Log($"<color=red>Miss!</color> {keyToPress}");
            comboTracker?.RegisterMiss();
            return;
        }

        // Check for perfect zone
        NoteMover perfect = perfectZone();
        bool isPerfect = (perfect != null && perfect == note);

        if (debugLogs)
        {
            if (isPerfect)
                Debug.Log($"<color=magenta>PERFECT!</color> {keyToPress} -> {note.name}");
            else
                Debug.Log($"<color=green>Hit!</color> {keyToPress} -> {note.name}");
        }

        Destroy(note.gameObject);
        score += scorePerHit;
        lastScoredFrameByKey[keyToPress] = Time.frameCount;
        if (isPerfect)
            comboTracker?.RegisterPerfectHit();
        else
            comboTracker?.RegisterHit();
    }
    public void UpdateKey(Key newKey)
    {
        if (keyOwners.TryGetValue(keyToPress, out HitZone owner) && owner == this)
            keyOwners.Remove(keyToPress);

        keyToPress = newKey;

        if (!keyOwners.ContainsKey(keyToPress))
            keyOwners[keyToPress] = this;
        else
            Debug.LogWarning($"Key {newKey} already owned by another HitZone!");
    }
    private NoteMover perfectZone()
    {
        Vector3 center = zoneCollider.bounds.center;
        Vector3 halfExtents = zoneCollider.bounds.extents;

        Debug.Log($"OverlapBox center: {center}, halfExtents: {halfExtents}");

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
            if (candidate == null || candidate == zoneCollider) continue;

            NoteMover mover = candidate.GetComponentInParent<NoteMover>();
            if (mover == null) continue;
            if (mover.target != transform) continue;

            float sqrDistance = (mover.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < bestSqrDistance)
            {
                best = mover;
                bestSqrDistance = sqrDistance;
            }
        }

        return best;
    }

    private NoteMover FindNoteInsideZone()
    {
        NoteMover best = null;
        float bestDistance = float.MaxValue;

        Vector3 center = transform.position;
        NoteMover[] allNotes = Object.FindObjectsByType<NoteMover>(FindObjectsSortMode.None);

        foreach (var mover in allNotes)
        {
            if (mover == null || !mover.gameObject.activeSelf || mover.target != transform) continue;

            Renderer rend = mover.GetComponent<Renderer>();
            if (rend == null) continue;

            float noteHalfExtentZ = rend.bounds.extents.z;

            float signedDistance = Vector3.Dot(mover.transform.position - center, Vector3.forward);

            float noteMin = signedDistance - noteHalfExtentZ;
            float noteMax = signedDistance + noteHalfExtentZ;

            bool overlaps = noteMax >= -hitWindowBefore && noteMin <= hitWindowAfter;

            if (overlaps)
            {
                float absDist = Mathf.Abs(signedDistance);
                if (absDist < bestDistance)
                {
                    best = mover;
                    bestDistance = absDist;
                }
            }
        }

        return best;
    }
}