using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
public class ScreenDropdown : MonoBehaviour
{
    public GameObject stackScreen;
    public GameObject briefcaseScreen;
    public GameObject unstackScreen;
    public GameObject workshopScreen;
    public GameObject diamondshinyObject;  // Changed to GameObject
    public GameObject goldenshinyObject;   // Changed to GameObject

    public Dropdown dropdown;
    public Sprite stackScreenSprite; // Assign in Inspector
    public Sprite briefcaseShopSprite; // Assign in Inspector
    public Sprite unstackShopSprite; // Assign in Inspector
    public Sprite workshopSprite; // Assign in Inspector

    private void Start()
    {
        // Initialize dropdown options with images
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Combine Screen", stackScreenSprite),
            new Dropdown.OptionData("Briefcase Shop", briefcaseShopSprite),
            new Dropdown.OptionData("Unstack Screen", unstackShopSprite),
            new Dropdown.OptionData("Steam Workshop", workshopSprite)

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
    public void OpenFolderInExplorer()
    {
        // Define the path to the WorkshopImageFolder
        string folderPath = Path.Combine(Application.persistentDataPath, "WorkshopImageFolder");

        // Normalize the path for consistency
        folderPath = Path.GetFullPath(folderPath).Replace("\\", "/");

        // Log the folder path for debugging
        Debug.Log("Normalized WorkshopImageFolder path: " + folderPath);

        try
        {
            // Ensure the folder exists, creating it if necessary
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Folder did not exist, created: " + folderPath);
            }
            else
            {
                Debug.Log("Folder already exists: " + folderPath);
            }

            // Open the folder in File Explorer
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true, // Use the OS shell to execute
                Verb = "open"           // Open the folder
            });
            Debug.Log("Opened folder in File Explorer: " + folderPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error handling folder: " + folderPath + "\nException: " + ex.Message);
        }
    }

    public void OpenThumbnailFolderInExplorer()
    {
        // Define the path to the WorkshopThumbnailFolder
        string folderPath = Path.Combine(Application.persistentDataPath, "WorkshopThumbnailFolder");
        folderPath = folderPath.Replace("\\", "/"); // Normalize to forward slashes

        // Log the folder path for debugging
        Debug.Log("Normalized WorkshopImageFolder path: " + folderPath);

        try
        {
            // Ensure the folder exists, creating it if necessary
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Successfully created folder: " + folderPath);
            }
            else
            {
                Debug.Log("Folder already exists: " + folderPath);
            }

            // Open the folder in File Explorer
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true, // Use the OS shell to execute
                Verb = "open"           // Open the folder
            });
            Debug.Log("Opened folder in File Explorer: " + folderPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to handle folder operation for: " + folderPath + "\nError: " + ex.Message);
        }
    }



    public void LegalAgreement()
    {
        SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
    }
    public void OpenMyUploadsPage()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized!");
            return;
        }

        // Get the user's SteamID
        CSteamID userSteamID = SteamUser.GetSteamID();
        string steamID = userSteamID.m_SteamID.ToString();

        // Construct the Steam desktop app URL
        string steamUrl = $"steam://openurl/https://steamcommunity.com/profiles/{steamID}/myworkshopfiles/?appid=3069470";

        // Open the Steam desktop app with the specific URL
        try
        {
            System.Diagnostics.Process.Start(steamUrl);
        }
        catch
        {
            // Fallback to open in a browser if the Steam desktop app fails
            Application.OpenURL($"https://steamcommunity.com/profiles/{steamID}/myworkshopfiles/?appid=3069470");
        }
    }
    public void WorkshopPage()
    {
        string steamUrl = $"steam://openurl/https://steamcommunity.com/app/3069470/workshop/";

        try
        {
            System.Diagnostics.Process.Start(steamUrl);
        }
        catch
        {
            // Fallback to open in a browser if the Steam desktop app fails
            SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/app/3069470/workshop/");
        }
    }
    public void OpenAPIKeyPage()
    {
        //Application.OpenURL("https://steamcommunity.com/dev/apikey");
        SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/dev/apikey");
    }
    private void ShowScreen(int index)
    {
        // Disable all screens
        stackScreen.SetActive(false);
        briefcaseScreen.SetActive(false);
        unstackScreen.SetActive(false);
        workshopScreen.SetActive(false);

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
            case 3:
                workshopScreen.SetActive(true);
                diamondshinyObject.SetActive(false);  // Disable GameObject
                goldenshinyObject.SetActive(false);   // Disable GameObject
                break;
        }
    }
}
