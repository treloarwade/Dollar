using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUpgradeButton : MonoBehaviour
{
    public Toggle toggle; // Reference to the UI Toggle
    public GameObject buttonToHide; // Reference to the button GameObject

    void Start()
    {
        // Ensure toggle and button are assigned
        if (toggle == null || buttonToHide == null)
        {
            Debug.LogError("Toggle or buttonToHide is not assigned in the inspector.");
            return;
        }

        // Add a listener to the toggle to call OnToggleChanged when its value changes
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    // Called whenever the toggle value changes
    void OnToggleChanged(bool isOn)
    {
        buttonToHide.SetActive(!isOn);
    }

    void OnDestroy()
    {
        // Remove the listener to avoid potential memory leaks
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}
