using UnityEngine;
using System.Collections;

public class ResizableWindow : MonoBehaviour
{
    private int lastScreenWidth;
    private int lastScreenHeight;
    private bool isResizing = false;
    private float resizeCheckInterval = 0.1f; // Adjust the interval as needed

    void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        StartCoroutine(CheckForResize());
    }

    IEnumerator CheckForResize()
    {
        while (true)
        {
            yield return new WaitForSeconds(resizeCheckInterval);

            if (Screen.fullScreenMode == FullScreenMode.Windowed)
            {
                if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
                {
                    lastScreenWidth = Screen.width;
                    lastScreenHeight = Screen.height;

                    if (!isResizing)
                    {
                        isResizing = true;
                        StartCoroutine(WaitForResizeToEnd());
                    }
                }
            }
        }
    }

    IEnumerator WaitForResizeToEnd()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the wait time as needed

        if (Screen.width == lastScreenWidth && Screen.height == lastScreenHeight)
        {
            AdjustWindowSize();
        }

        isResizing = false;
    }

    void AdjustWindowSize()
    {
        float targetAspectRatio = 16.0f / 9.0f;
        int newWidth = Screen.width;
        int newHeight = (int)(newWidth / targetAspectRatio);

        if (newHeight > Screen.height)
        {
            newHeight = Screen.height;
            newWidth = (int)(newHeight * targetAspectRatio);
        }

        Screen.SetResolution(newWidth, newHeight, FullScreenMode.Windowed);
    }
}
