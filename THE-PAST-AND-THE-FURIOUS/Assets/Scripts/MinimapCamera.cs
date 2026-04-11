using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Settings")]
    public float height = 80f;
    public float size = 50f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            cam.orthographicSize = size;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + Vector3.up * height;
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
