using UnityEngine;
using UnityEngine.UI; // Required for using UI components like Toggle

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites;               // Array of sprites to cycle through
    public float frameRate = 0.1f;         // Time between frames (in seconds)
    public ParticleSystem particleEffect;  // Particle system to play when the 3rd element is reached
    public Toggle animationToggle;         // Public UI Toggle to control animation

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;
    private bool animationtoggleon = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0]; // Set the initial sprite
        }

        // Ensure the animation starts in the correct state based on the toggle
        if (animationToggle != null)
        {
            animationToggle.isOn = false;

            animationToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    void Update()
    {
        if (!animationtoggleon || sprites.Length == 0)
            return;

        // Update the timer
        timer += Time.deltaTime;

        // Check if it's time to swap to the next frame
        if (timer >= frameRate)
        {
            timer = 0f; // Reset the timer
            currentFrame = (currentFrame + 1) % sprites.Length; // Move to the next frame
            spriteRenderer.sprite = sprites[currentFrame]; // Update the sprite

            // Check if we've reached the 3rd element (index 2)
            if (currentFrame == 3 && particleEffect != null)
            {
                particleEffect.Play(); // Play the particle effect
            }
        }
    }

    // Called when the toggle value changes
    void OnToggleValueChanged(bool isOn)
    {
        animationtoggleon = isOn;
    }
}
