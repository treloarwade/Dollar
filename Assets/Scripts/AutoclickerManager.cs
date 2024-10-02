using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AutoclickerManager : MonoBehaviour
{
    public RectTransform giantMouseIcon; // Drag your giant mouse icon here
    public RectTransform fingerPart; // Drag the finger part RectTransform here
    public Button[] targetButtons; // Array of possible target buttons
    public float clickInterval = 2f; // Time between clicks in seconds
    public float moveAmplitude = 10f; // How far the icon moves up and down
    public float moveDuration = 1f; // Duration of the up and down movement

    private Vector2 originalPosition;
    private Coroutine moveCoroutine;
    private Vector2 lastScreenSize;
    public Text onoff;

    void Start()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        originalPosition = giantMouseIcon.anchoredPosition;
    }

    public void ToggleAutoClicker()
    {
        if (moveCoroutine == null)
        {
            // Start the auto-clicker
            moveCoroutine = StartCoroutine(MoveFingerUpAndDown());
            onoff.text = "Turn off";
        }
        else
        {
            // Stop the auto-clicker
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            giantMouseIcon.anchoredPosition = originalPosition; // Reset to the original position
            onoff.text = "Turn on";

        }
    }
    void Update()
    {
        // Check if the screen size has changed
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            OnScreenSizeChanged();
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }

    void OnScreenSizeChanged()
    {
        // Update the original position based on the new screen size
        originalPosition = giantMouseIcon.anchoredPosition;
    }

    public void StartAutoClicker()
    {
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveFingerUpAndDown());
        }
    }

    public void StopAutoClicker()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            giantMouseIcon.anchoredPosition = originalPosition; // Reset to the original position
        }
    }

    IEnumerator MoveFingerUpAndDown()
    {
        while (true)
        {
            // Move up
            yield return MoveToPosition(originalPosition + Vector2.up * moveAmplitude);
            ClickButton();

            // Move down
            yield return MoveToPosition(originalPosition - Vector2.up * moveAmplitude);
        }
    }

    IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition = giantMouseIcon.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            giantMouseIcon.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        giantMouseIcon.anchoredPosition = targetPosition;
    }

    void ClickButton()
    {
        if (targetButtons.Length == 0) return;

        // Check if the finger part is near any of the target buttons
        foreach (Button button in targetButtons)
        {
            // Check if the button's GameObject is active
            if (button.gameObject.activeInHierarchy)
            {
                // Check if the finger part overlaps the active button
                if (RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(), fingerPart.position))
                {
                    // Simulate the button click
                    button.onClick.Invoke();
                    return; // Exit after clicking one button
                }
            }
        }
    }
}
