using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HitZone : MonoBehaviour
{
    [Header("Input")]
    public Key keyToPress = Key.Q;

    [Header("Scoring")]
    public int scorePerHit = 10;
    public int localScore = 0;

    [Header("Timing Window")]
    public float hitWindowBefore = 0.3f;
    public float hitWindowAfter = 0.2f;

    [Header("Visual Feedback")]
    public float pulseScale = 1.2f;
    private Vector3 originalScale;

    private HashSet<int> notesHitThisFrame = new HashSet<int>();

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10f);

        if (Keyboard.current == null) return;
        if (Keyboard.current[keyToPress].wasPressedThisFrame)
        {
            ProcessHit();
        }
    }

    private void LateUpdate()
    {
        notesHitThisFrame.Clear();
    }

    private void ProcessHit()
    {
        NoteMover note = FindNoteInWindow();

        if (note != null)
        {
            int noteID = note.gameObject.GetInstanceID();
            if (notesHitThisFrame.Contains(noteID)) return;
            notesHitThisFrame.Add(noteID);

            transform.localScale = originalScale * pulseScale;
            localScore += scorePerHit;

            Debug.Log($"<color=green>Hit!</color> {keyToPress}");
            Destroy(note.gameObject);
        }
        else
        {
            Debug.Log($"<color=red>Miss!</color> {keyToPress}");
        }
    }

    private NoteMover FindNoteInWindow()
    {
        NoteMover best = null;
        float bestDistance = float.MaxValue;

        // Notes now travel through the hit zone center
        Vector3 center = transform.position;

        NoteMover[] allNotes = Object.FindObjectsByType<NoteMover>(FindObjectsSortMode.None);

        foreach (var mover in allNotes)
        {
            if (mover == null || !mover.gameObject.activeSelf || mover.target != transform) continue;

            // Use renderer bounds to check if ANY part of the note overlaps the window
            Renderer rend = mover.GetComponent<Renderer>();
            if (rend == null) continue;

            // Half-extent of the note along the Z (travel) axis
            float noteHalfExtentZ = rend.bounds.extents.z;

            // Signed distance from hit zone center to note center along Z
            float signedDistance = Vector3.Dot(mover.transform.position - center, Vector3.forward);

            // The note's leading and trailing edges along Z
            float noteMin = signedDistance - noteHalfExtentZ;
            float noteMax = signedDistance + noteHalfExtentZ;

            // Hit if any part of the note overlaps [-hitWindowBefore, +hitWindowAfter]
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