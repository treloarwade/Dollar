using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public float velocityThreshold = 5f; // Minimum speed required to play the sound
    private Rigidbody2D rb;             // Rigidbody2D component of the object
    public ParticleSystem diamond;
    public ParticleSystem diamond2;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object's velocity before the collision is greater than the threshold
        if (rb.velocity.magnitude > velocityThreshold)
        {
            diamond.Play();
            diamond2.Play();
        }
    }
    void OnMouseDown()
    {

            diamond.Play();
            diamond2.Play();

    }
}
