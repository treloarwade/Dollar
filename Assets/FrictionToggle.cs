using UnityEngine;

public class FrictionToggle : MonoBehaviour
{
    public Rigidbody2D targetRigidbody; // Assign your collider here in the inspector
    public PhysicsMaterial2D lowFrictionMaterial; // PhysicMaterial with friction set to 0
    public PhysicsMaterial2D highFrictionMaterial; // PhysicMaterial with friction set to 0.4

    void Start()
    {
        // Ensure Rigidbody2D and materials are assigned
        if (targetRigidbody == null || lowFrictionMaterial == null || highFrictionMaterial == null)
        {
            Debug.LogError("Please assign all required fields in the inspector.");
        }
    }

    public void AddFriction()
    {
        if (targetRigidbody != null && highFrictionMaterial != null)
        {
            targetRigidbody.sharedMaterial = highFrictionMaterial;
            Debug.Log("High friction applied (0.4).");
        }
    }

    public void RemoveFriction()
    {
        if (targetRigidbody != null && lowFrictionMaterial != null)
        {
            targetRigidbody.sharedMaterial = lowFrictionMaterial;
            Debug.Log("Low friction applied (0).");
        }
    }
}
