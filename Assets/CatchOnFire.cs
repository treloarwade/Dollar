using System.Collections;
using UnityEngine;

public class CatchOnFire : MonoBehaviour
{
    public ParticleSystem fire;
    private bool onfire;
    private void Awake()
    {
        // Get the SpriteRenderer component on the same GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the SpriteRenderer is found
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the GameObject.");
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (onfire)
        {
            Debug.Log("onfire = " + onfire);
            return;
        }
        Debug.Log("Entering");
        if (collider.CompareTag("Fire"))
        {
            Debug.Log("Fire Detected");

            BurstIntoFlames();
        }
        else if (collider.CompareTag("GoldenFire"))
        {
            Debug.Log("Fire Detected");

            BurstIntoFlames();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Fire"))
        {
            Debug.Log("Fire Collision");
            BurstIntoFlames();
        }
    }
    private void BurstIntoFlames()
    {
        if (onfire)
        {
            Debug.Log("onfire = " + onfire);

            return;
        }
        fire.Play();
        StartCoroutine(TransitionColor());
        StartCoroutine(Flames());
        onfire = true;

    }
    private IEnumerator Flames()
    {
        StartCoroutine(ScaleFire(fire, new Vector3(0.2f, 0.2f, 0f), new Vector3(2.6f, 2.3f, 0f), 3f));
        yield return new WaitForSeconds(3.1f);

        StartCoroutine(ScaleGameObject(new Vector3(1f, 1f, 0f), new Vector3(0f, 0f, 0f), 1f));
        StartCoroutine(ScaleFire(fire, new Vector3(2.6f, 2.3f, 0f), new Vector3(0.2f, 0.2f, 0f), 1f));
        yield return new WaitForSeconds(1.1f);
        fire.Stop();
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);

        yield return null;

    }
    private float transitionDuration = 4.0f; // Duration of the transition in seconds
    private SpriteRenderer spriteRenderer;
    IEnumerator TransitionColor()
    {
        Color startColor = Color.white;
        Color endColor = Color.black;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor; // Ensure the final color is set
    }
    private IEnumerator ScaleFire(ParticleSystem ps, Vector3 startScale, Vector3 endScale, float duration)
    {
        Vector3 initialScale = startScale;
        Vector3 finalScale = endScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Update elapsed time
            elapsed += Time.deltaTime;

            // Calculate the fraction of time
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp position

            // Lerp scale
            var shape = ps.shape;
            shape.scale = Vector3.Lerp(initialScale, finalScale, t);

            yield return null;
        }

        // Ensure final position and scale are set
        var finalShape = ps.shape;
        finalShape.scale = finalScale;
    }
    private IEnumerator ScaleGameObject(Vector3 initialScale, Vector3 finalScale, float duration)
    {
        float elapsedTime = 0f;

        // Set initial scales for GameObject and ParticleSystem
        transform.localScale = initialScale;

        // Access ParticleSystem's shape module for scaling

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Lerp the GameObject scale
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final scale is applied
        transform.localScale = finalScale;
    }
}
