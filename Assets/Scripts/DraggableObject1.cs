using UnityEngine;

public class DraggableObject1 : MonoBehaviour
{
    private Vector3 offset;
    private float zCoordinate;
    public float defaultGravity = 1f;
    private Rigidbody2D rb;
    private Vector3 lastPosition; // To store the last position for velocity calculation
    private Vector3 velocity; // To store the calculated velocity

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale; // Store the initial gravity scale
    }

    void OnMouseDown()
    {
        // Record the distance between the object and the camera in the z-axis
        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;

        // Calculate the offset between the object position and the mouse position
        offset = transform.position - GetMouseWorldPosition();


        // Store the initial position
        lastPosition = transform.position;
        if (rb != null)
        {
            // Disable gravity while dragging
            rb.gravityScale = 0;

            // Set Rigidbody to kinematic so that physics don't interfere while dragging
            rb.isKinematic = true;
        }
    }

    void OnMouseDrag()
    {
        // Move the object to follow the mouse, maintaining the offset
        Vector3 newPosition = GetMouseWorldPosition() + offset;

        // Update the position of the object
        transform.position = newPosition;

        // Calculate velocity based on change in position over time
        velocity = (newPosition - lastPosition) / Time.deltaTime;

        // Update last position
        lastPosition = newPosition;
    }

    void OnMouseUp()
    {
        if (rb != null)
        {
            // Re-enable gravity
            rb.gravityScale = defaultGravity;

            // Set Rigidbody to dynamic so it can interact with physics again
            rb.isKinematic = false;

            // Apply the velocity calculated during dragging
            rb.velocity = velocity;
        }

    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the current mouse position in screen coordinates
        Vector3 mousePoint = Input.mousePosition;

        // Set the z coordinate to the recorded z-coordinate from OnMouseDown
        mousePoint.z = zCoordinate;

        // Convert screen coordinates to world coordinates
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
