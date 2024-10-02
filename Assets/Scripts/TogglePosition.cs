using System.Collections;
using UnityEngine;

public class TogglePosition : MonoBehaviour
{
    private bool isMoving = false;  // Flag to prevent multiple animations
    private bool isOpen = false;    // Track whether the object is in the open or closed state
    public ParticleSystem fire;
    public GameObject firehitbox;

    // Define positions and rotations
    private Vector3 openPosition = new Vector3(-0.5339f, 0.1582f, 0);
    private float openRotation = 141.95f;

    private Vector3 midwayPosition1 = new Vector3(-0.408f, 0.373f, 0);
    private float midwayRotation1 = 90f;

    private Vector3 midwayPosition2 = new Vector3(-0.189f, 0.412f, 0);
    private float midwayRotation2 = 45f;

    private Vector3 closedPosition = new Vector3(-0.017f, 0.285f, 0);
    private float closedRotation = 0f;

    // Duration of the animation
    public float moveDuration = 1.5f; // Total duration for the entire animation

    void OnMouseDown()
    {
        if (!isMoving)
        {
            // Toggle state and start moving coroutine
            isOpen = !isOpen;
            StartCoroutine(MoveToPosition(isOpen));
        }
    }

    private IEnumerator MoveToPosition(bool opening)
    {
        isMoving = true;

        // Set start and end positions and rotations
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        Vector3 endPosition = opening ? openPosition : closedPosition;
        float endRotation = opening ? openRotation : closedRotation;

        Quaternion endRotationQuat = Quaternion.Euler(0, 0, endRotation);
        Quaternion midwayRotationQuat1 = Quaternion.Euler(0, 0, midwayRotation1);
        Quaternion midwayRotationQuat2 = Quaternion.Euler(0, 0, midwayRotation2);

        // Calculate duration for each segment of the animation
        float segmentDuration = moveDuration / 3;
        if (!isOpen)
        {
            fire.Stop();
            firehitbox.SetActive(false);
        }
        if (opening)
        {
            // Opening: Closed -> Midway2 -> Midway1 -> Open
            yield return MoveSegment(startPosition, midwayPosition2, startRotation, midwayRotationQuat2, segmentDuration, true);
            yield return MoveSegment(midwayPosition2, midwayPosition1, midwayRotationQuat2, midwayRotationQuat1, segmentDuration, true);
            yield return MoveSegment(midwayPosition1, endPosition, midwayRotationQuat1, endRotationQuat, segmentDuration, true);
        }
        else
        {
            // Closing: Open -> Midway1 -> Midway2 -> Closed
            yield return MoveSegment(startPosition, midwayPosition1, startRotation, midwayRotationQuat1, segmentDuration, true);
            yield return MoveSegment(midwayPosition1, midwayPosition2, midwayRotationQuat1, midwayRotationQuat2, segmentDuration, true);
            yield return MoveSegment(midwayPosition2, endPosition, midwayRotationQuat2, endRotationQuat, segmentDuration, true);
        }
        if (isOpen)
        {
            fire.Play();
            firehitbox.SetActive(true);
        }
        isMoving = false;
    }

    private IEnumerator MoveSegment(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float duration, bool useEaseOut)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            if (useEaseOut)
            {
                t = t * t * (3f - 2f * t); // Ease In-Out (SmoothStep)
            }

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the target position and rotation
        transform.localPosition = endPos;
        transform.localRotation = endRot;
    }
}
