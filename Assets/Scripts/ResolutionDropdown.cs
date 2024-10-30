using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public MoveScreens moveScreens;
    public Text fpstext;
    public GameObject fpsmenu;
    public GameObject colormenu;
    private Vector3 originalposition;


    private int[] vsyncValues = { 1, 3 };
    private int currentVSyncIndex = 0; // Default index for VSync count 1

    void Start()
    {
        // Add listener for when the value of the Dropdown changes
        resolutionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        originalposition = colormenu.transform.position;
        // Set default VSync count
        SetVSyncCount(vsyncValues[currentVSyncIndex]);
    }

    void OnDropdownValueChanged(int index)
    {
        ChangeResolution(index);
    }

    void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0:
                // Set resolution to 960x540 windowed
                Screen.SetResolution(960, 540, false);
                moveScreens.resolutionsetting = 960;
                break;
            case 1:
                // Set resolution to 800x600 windowed
                Screen.SetResolution(800, 600, false);
                moveScreens.resolutionsetting = 800;
                break;
            case 2:
                // Set resolution to 1366x768 windowed
                Screen.SetResolution(1366, 768, false);
                moveScreens.resolutionsetting = 1366;
                break;
            case 3:
                // Set resolution to 1920x1080 fullscreen
                Screen.SetResolution(1920, 1080, true);
                moveScreens.resolutionsetting = 1920;
                break;
            case 4:
                fpsmenu.SetActive(!fpsmenu.activeSelf);
                StartCoroutine(FPSmenu());
                break;
            case 5:
                colormenu.SetActive(true);
                StartCoroutine(FPSmenu());
                colormenu.transform.position = originalposition;
                break;
            case 6:
                Debug.Log("Reset");
                break;
            default:
                break;
        }
    }

    private IEnumerator FPSmenu()
    {
        yield return new WaitForSeconds(0.2f);
        resolutionDropdown.value = 7;
        yield return null;
    }

    public void NextVSync()
    {
        CycleVSync(1);
    }

    public void PreviousVSync()
    {
        CycleVSync(-1);
    }
    public QualityToggle qualityToggle;
    public Sprite[] images;              // Array of Image components
    public Image fpsmenuborder;       // Default sprite for index 0
    public Image fpsmenuleft;       // Default sprite for index 1
    public Image fpsmenuright;       // Default sprite for index 2
    public void SetVSyncCount(int vsyncCount)
    {
        QualitySettings.vSyncCount = vsyncCount;
        fpstext.text = GetVSyncLabel(vsyncCount);
        if(vsyncCount != 1)
        {
            qualityToggle.SetHidden();
            fpsmenuborder.enabled = false;
            fpsmenuleft.sprite = images[2];
            fpsmenuright.sprite = images[3];
        }
        else
        {
            qualityToggle.SetVisible();
            fpsmenuborder.enabled = true;
            fpsmenuleft.sprite = images[0];
            fpsmenuright.sprite = images[1];
        }
    }

    void CycleVSync(int direction)
    {
        currentVSyncIndex = (currentVSyncIndex + direction + vsyncValues.Length) % vsyncValues.Length;
        SetVSyncCount(vsyncValues[currentVSyncIndex]);
    }

    string GetVSyncLabel(int vsyncCount)
    {
        switch (vsyncCount)
        {
            case 1:
                return "Normal";
            case 2:
                return "Low";
            case 3:
                return "Lowest";
            default:
                return "Unknown";
        }
    }
}
