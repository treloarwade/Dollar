using UnityEngine;

public class AngularDragAdjuster : MonoBehaviour
{
    public Rigidbody2D rb2D; // Reference to the Rigidbody2D

    void Update()
    {
        // Get the current Z rotation of the object
        float zRotation = transform.eulerAngles.z;

        // Convert the Z rotation to a range of -180 to 180 for easier comparison
        if (zRotation > 180)
        {
            zRotation -= 360;
        }

        // Check if the Z rotation is greater than 50 or less than -50
        if (zRotation > 50 || zRotation < -50)
        {
            rb2D.drag = 1f; // Set angular drag to 0.05
        }
        else
        {
            rb2D.drag = 0f; // Reset angular drag to 0 (or any default value you prefer)
        }
    }
}
