using System.Collections;
using UnityEngine;

public class RevealResetLevels : MonoBehaviour
{
    public GameObject resetPrompt; // The GameObject to display as the reset confirmation
    private bool isPromptActive = false; // Tracks if the prompt is currently displayed

    void Start()
    {
        // Ensure the prompt is hidden at the start
        if (resetPrompt != null)
        {
            resetPrompt.SetActive(false);
        }
    }

    public void ShowResetPrompt()
    {
        if (isPromptActive)
        {
            Debug.Log("The reset prompt is already being displayed.");
            return;
        }

        if (resetPrompt != null)
        {
            StartCoroutine(DisplayPrompt());
        }
        else
        {
            Debug.LogError("Reset prompt GameObject is not assigned in the inspector.");
        }
    }

    private IEnumerator DisplayPrompt()
    {
        isPromptActive = true;

        // Show the prompt
        resetPrompt.SetActive(true);

        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Hide the prompt after 5 seconds
        resetPrompt.SetActive(false);
        isPromptActive = false;
    }
}
