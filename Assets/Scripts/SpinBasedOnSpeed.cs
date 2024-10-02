using UnityEngine;

public class SpinBasedOnSpeed : MonoBehaviour
{
    public float speedThreshold = 5f; // The speed required to start spinning
    public float spinMultiplier = 100f; // How fast the object should spin based on speed

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Calculate the current speed
        float speed = rb.velocity.magnitude;

        // Check if the speed is above the threshold
        if (speed > speedThreshold)
        {
            // Determine the direction of movement (-1 for left, 1 for right)
            float direction = Mathf.Sign(rb.velocity.x);

            // Apply rotation based on speed and direction
            float rotationAmount = direction * speed * spinMultiplier * Time.deltaTime;
            transform.Rotate(0f, 0f, -rotationAmount);
        }
    }
}
