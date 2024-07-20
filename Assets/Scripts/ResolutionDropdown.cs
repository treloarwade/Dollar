using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public MoveScreens moveScreens;

    void Start()
    {
        // Add listener for when the value of the Dropdown changes, to take action
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(resolutionDropdown.value); });
    }

    void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0:
                // Set resolution to 800x600 windowed
                Screen.SetResolution(960, 540, false);
                moveScreens.resolutionsetting = 960;
                break;
            case 1:
                // Set resolution to 1920x1080 fullscreen
                Screen.SetResolution(800, 600, false);
                moveScreens.resolutionsetting = 800;
                break;
            case 2:
                // Set resolution to 1920x1080 fullscreen
                Screen.SetResolution(1366, 768, false);
                moveScreens.resolutionsetting = 1366;
                break;
            case 3:
                // Set resolution to 1920x1080 fullscreen
                Screen.SetResolution(1920, 1080, true);
                moveScreens.resolutionsetting = 1920;
                break;
            default:
                break;
        }
    }
}

