using UnityEngine;

public class WheelColliders : MonoBehaviour
{
    [Header("Collider Settings")]
    public float radius = 0.35f;
    public float height = 0.4f;

    void Awake()
    {
        // Find wheel transforms from WheelVisuals
        WheelVisuals wheelVisuals = GetComponent<WheelVisuals>();
        Transform[] wheels = null;

        if (wheelVisuals != null)
        {
            wheels = new Transform[] {
                wheelVisuals.frontLeftWheel,
                wheelVisuals.frontRightWheel,
                wheelVisuals.rearLeftWheel,
                wheelVisuals.rearRightWheel
            };
        }

        // Fallback: search for Tire-named transforms
        if (wheels == null || wheels.Length == 0)
        {
            var found = new System.Collections.Generic.List<Transform>();
            foreach (var child in GetComponentsInChildren<Transform>())
            {
                if (child.name.ToLower().Contains("tire") || child.name.ToLower().Contains("wheel"))
                    found.Add(child);
            }
            wheels = found.ToArray();
        }

        foreach (var wheel in wheels)
        {
            if (wheel == null) continue;
            if (wheel.GetComponent<CapsuleCollider>() != null) continue;

            CapsuleCollider col = wheel.gameObject.AddComponent<CapsuleCollider>();
            col.radius = radius;
            col.height = height;
            col.direction = 0; // X-axis (sideways, matching wheel orientation)
            col.center = Vector3.zero;
        }

        Debug.Log("WheelColliders: Added capsule colliders to " + wheels.Length + " wheels");
    }
}
