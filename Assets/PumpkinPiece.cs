using UnityEngine;

public class PumpkinPiece : MonoBehaviour
{
    private Rigidbody2D rb2d;

    void Awake()
    {
        // Get or add a Rigidbody2D component to the child object
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }

    }

    // Call this method to make the piece "explode"
    public void ExplodePiece(float explosionForce)
    {
        // Make the Rigidbody2D dynamic when explosion happens
        rb2d.isKinematic = false;

        // Apply a random velocity to simulate an explosion
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb2d.velocity = randomDirection * explosionForce;
    }
}
