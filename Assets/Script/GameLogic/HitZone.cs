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

        if (debugLogs) Debug.Log($"<color=green>Hit!</color> {keyToPress} -> {note.name}");

        Destroy(note.gameObject);
        score += scorePerHit;
        lastScoredFrameByKey[keyToPress] = Time.frameCount;
        comboTracker?.RegisterHit();
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