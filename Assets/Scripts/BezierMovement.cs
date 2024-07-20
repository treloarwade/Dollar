using System.Collections;
using UnityEngine;
using UnityEngine.UI; // If you're using an Image component

public class BezierMovementWithSpin : MonoBehaviour
{
    public RectTransform imageRectTransform; // Assign the RectTransform of your image in the inspector
    public float minWaitTime = 3f;
    public float maxWaitTime = 480000f;
    public float animationDuration = 5f; // Duration of the animation
    public float spinSpeed = 60f; // Degrees per second

    private void Start()
    {
        StartCoroutine(AnimateImageCoroutine());
    }

    private IEnumerator AnimateImageCoroutine()
    {
        // Wait for a random duration between minWaitTime and maxWaitTime
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // Define start and end positions with random y coordinates
        Vector2 startPos = new Vector2(600f, Random.Range(-300f, 300f));
        Vector2 endPos = new Vector2(-600f, Random.Range(-300f, 300f));
        Vector2 controlPoint = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f)); // Random control point

        yield return StartCoroutine(MoveAlongBezierCurveWithSpin(startPos, endPos, controlPoint, animationDuration));
    }

    private IEnumerator MoveAlongBezierCurveWithSpin(Vector2 start, Vector2 end, Vector2 control, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector2 position = CalculateBezierPoint(t, start, control, end);
            imageRectTransform.anchoredPosition = position;

            // Spin the image
            imageRectTransform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

            yield return null;
        }

        // Ensure the final position is exactly the end position
        imageRectTransform.anchoredPosition = end;
        StartCoroutine(AnimateImageCoroutine());
    }

    private Vector2 CalculateBezierPoint(float t, Vector2 start, Vector2 control, Vector2 end)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * start; // u^2 * start
        p += 2 * u * t * control; // 2 * u * t * control
        p += tt * end; // t^2 * end

        return p;
    }
}
