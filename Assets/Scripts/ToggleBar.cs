using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBar : MonoBehaviour
{
    // Reference to the GameObject you want to toggle
    public GameObject TopBar;

    void Update()
    {
        // Check if the 1 key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Toggle the active state of the target GameObject
            if (TopBar != null)
            {
                TopBar.SetActive(!TopBar.activeSelf);
            }
        }
    }
}

