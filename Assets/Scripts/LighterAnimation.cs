using System.Collections;
using UnityEngine;

public class LighterAnimation : MonoBehaviour
{
    public Vector3 midpoint = new Vector3(-38.1f, 55.2f, 0);
    public Vector3 openLocalPositionOffset = new Vector3(-30.5f, -25.8f, 0); // Relative to midpoint
    public Vector3 closeLocalPositionOffset = new Vector3(35.9f, -27.3f, 0); // Relative to midpoint
    public float openRotation = 128.91f;
    public float closeRotation = 0f;
    public float animationDuration = 0.5f;

    private bool isOpen = false;
    private Coroutine animationCoroutine;

    void Start()
    {
        transform.localPosition = midpoint + closeLocalPositionOffset;

        transform.localRotation = Quaternion.Euler(0, 0, closeRotation);
    }

    public void ToggleLighter()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateLighter(isOpen));
        isOpen = !isOpen;
    }

    private IEnumerator AnimateLighter(bool opening)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = opening ? (midpoint + openLocalPositionOffset) : (midpoint + closeLocalPositionOffset);
        float startRotation = opening ? closeRotation : openRotation;
        float endRotation = opening ? openRotation : closeRotation;

        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / animationDuration);
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRotation, endRotation, elapsed / animationDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final local position and rotation are set
        transform.localPosition = endPosition;
        transform.localRotation = Quaternion.Euler(0, 0, endRotation);
    }
}
