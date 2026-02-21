using UnityEngine;

public class ThirdPersonControl : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    public Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);

    void Start()
    {
        if (GetComponent<Camera>() == null)
        {
            Debug.LogError("ThirdPersonControl must be attached to a Camera object. Disabling script.");
            enabled = false;
            return;
        }

        if (target == null)
        {
            return;
        }

        transform.position = target.position + cameraOffset;
        transform.LookAt(target.position + lookOffset);
    }
}
