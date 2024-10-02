using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDropdown : MonoBehaviour
{
    public GameObject stackScreen;
    public GameObject briefcaseScreen;
    public GameObject unstackScreen;
    public GameObject diamondshinyObject;  // Changed to GameObject
    public GameObject goldenshinyObject;   // Changed to GameObject

    public Dropdown dropdown;
    public Sprite stackScreenSprite; // Assign in Inspector
    public Sprite briefcaseShopSprite; // Assign in Inspector
    public Sprite unstackShopSprite; // Assign in Inspector
    private void Start()
    {
        // Initialize dropdown options with images
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Combine Screen", stackScreenSprite),
            new Dropdown.OptionData("Briefcase Shop", briefcaseShopSprite),
            new Dropdown.OptionData("Unstack Screen", unstackShopSprite)
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // Set up listener for dropdown value change
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set the default screen
        ShowScreen(0);
    }

    private void OnDropdownValueChanged(int index)
    {
        ShowScreen(index);
    }
    public GameObject blocker;
    public void OpenAPIKeyPage()
    {
        Application.OpenURL("https://steamcommunity.com/dev/apikey");
    }
    private void ShowScreen(int index)
    {
        // Disable all screens
        stackScreen.SetActive(false);
        briefcaseScreen.SetActive(false);
        unstackScreen.SetActive(false);

        // Manage ParticleSystem GameObjects
        switch (index)
        {
            case 0:
                stackScreen.SetActive(true);
                diamondshinyObject.SetActive(false);  // Disable GameObject
                goldenshinyObject.SetActive(false);   // Disable GameObject
                break;
            case 1:
                briefcaseScreen.SetActive(true);
                diamondshinyObject.SetActive(true);   // Enable GameObject
                goldenshinyObject.SetActive(true);    // Enable GameObject
                break;
            case 2:
                unstackScreen.SetActive(true);
                diamondshinyObject.SetActive(false);  // Disable GameObject
                goldenshinyObject.SetActive(false);   // Disable GameObject
                break;
        }
    }
}
