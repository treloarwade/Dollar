using UnityEngine;

public class BounceSound : MonoBehaviour
{
    public AudioSource audioSource;    // The audio source that plays the bounce sound
    public float velocityThreshold = 5f; // Minimum speed required to play the sound
    public float playFromTime = 0.1f;   // Time from which the sound will start playing (0.1 seconds)

    private Rigidbody2D rb; // Rigidbody2D component of the object

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void OnMouseDown()
    {

            audioSource.Play();

    }

}
