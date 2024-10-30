using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObjectSword : MonoBehaviour
{
    private Vector3 offset;
    private float zCoordinate;
    public float defaultGravity;
    private Rigidbody2D rb;
    private Vector3 lastPosition;
    private Vector3 velocity;
    public float spinAcceleration = 10f; // Acceleration rate for spin
    public float maxSpinSpeed = 720f;    // Max spin speed in degrees per second
    private float currentSpinSpeed = 0f; // Current spin speed
    private bool isDragging = false;     // Track if the object is currently being dragged
    private int spinDirection = 1;       // Track spin direction: 1 for clockwise, -1 for counterclockwise

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale; // Store the initial gravity scale
    }

    void OnMouseDown()
    {
        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();

        lastPosition = transform.position;
        isDragging = true; // Start dragging



        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.isKinematic = true;
        }
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = GetMouseWorldPosition() + offset;
        transform.position = newPosition;

        // Calculate velocity based on change in position over time
        velocity = (newPosition - lastPosition) / Time.deltaTime;
        lastPosition = newPosition;
    }

    void OnMouseUp()
    {
        isDragging = false; // Stop dragging
        if (rb != null)
        {
            rb.gravityScale = defaultGravity;
            rb.isKinematic = false;


        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void Update()
    {
        // Only apply rotation if dragging and holding the right mouse button
        if (isDragging && Input.GetMouseButton(1))
        {
            // Capture the existing spin speed and direction from angularVelocity
            currentSpinSpeed = Mathf.Abs(rb.angularVelocity);
            spinDirection = rb.angularVelocity >= 0 ? 1 : -1;
            // Increase spin speed up to the max limit in the captured direction
            currentSpinSpeed = Mathf.Min(currentSpinSpeed + spinAcceleration * Time.deltaTime, maxSpinSpeed);

            // Rotate the object based on the current spin speed and direction
            transform.Rotate(Vector3.forward * currentSpinSpeed * spinDirection * Time.deltaTime);
            // Apply the movement velocity calculated during drag
            rb.velocity = velocity;

            // Apply the current spin speed and direction to the Rigidbody's angular velocity
            rb.angularVelocity = currentSpinSpeed * spinDirection;
        }
        else if (!isDragging)
        {
            // Gradually slow down the spin smoothly in the last captured direction when the drag stops
            currentSpinSpeed = Mathf.Max(currentSpinSpeed - spinAcceleration * Time.deltaTime, 0f);
        }
    }
}
