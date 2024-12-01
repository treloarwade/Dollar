using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.IO;

public class WorkshopUploader : MonoBehaviour
{
    public InputField titleInputField;
    public InputField descriptionInputField;
    public Text filePathText;
    public Text statusText;
    public Button uploadButton;

    private CallResult<CreateItemResult_t> createItemResult;
    private CallResult<SubmitItemUpdateResult_t> submitItemUpdateResult;

    void Start()
    {
        uploadButton.onClick.AddListener(UploadToWorkshop);
        ResetUI();
    }

    private void ResetUI()
    {
        filePathText.text = "No file selected.";
        statusText.text = string.Empty;
    }

    public void UploadToWorkshop()
    {
        if (!SteamManager.Initialized)
        {
            SetStatusText("Steam is not initialized!", Color.red);
            Debug.LogError("Steam not initialized!");
            return;
        }

        string title = titleInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            SetStatusText("Title cannot be empty!", Color.red);
            Debug.LogError("Title cannot be empty!");
            return;
        }

        // Create the workshop item
        createItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItem);
        SteamAPICall_t handle = SteamUGC.CreateItem(new AppId_t(3069470), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        createItemResult.Set(handle);
    }

    private void OnCreateItem(CreateItemResult_t result, bool ioFailure)
    {
        if (ioFailure)
        {
            SetStatusText("Network error. Please try again.", Color.red);
            Debug.LogError("Network error during item creation.");
            return;
        }

        if (result.m_eResult == EResult.k_EResultAccessDenied)
        {
            SetStatusText("Access Denied: Ensure you have accepted the Workshop legal agreement and have proper permissions.", Color.red);
            Debug.LogError("Access Denied: " + result.m_eResult);
            return;
        }

        if (result.m_eResult != EResult.k_EResultOK)
        {
            SetStatusText("Failed to create item: " + result.m_eResult.ToString(), Color.red);
            Debug.LogError("Failed to create item: " + result.m_eResult.ToString());
            return;
        }

        if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
        {
            SetStatusText("User needs to accept the Workshop legal agreement.", Color.red);
            SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
            return;
        }

        // Start updating the item
        UGCUpdateHandle_t updateHandle = SteamUGC.StartItemUpdate(new AppId_t(3069470), result.m_nPublishedFileId);

        // Set item title
        if (!SteamUGC.SetItemTitle(updateHandle, titleInputField.text))
        {
            SetStatusText("Failed to set item title!", Color.red);
            Debug.LogError("Failed to set item title!");
            return;
        }

        // Set item description
        string description = descriptionInputField.text.Trim();
        if (!SteamUGC.SetItemDescription(updateHandle, description))
        {
            SetStatusText("Failed to set item description!", Color.red);
            Debug.LogError("Failed to set item description!");
            return;
        }

        // Define the path to the WorkshopThumbnailFolder
        string thumbnailFolderPath = Path.Combine(Application.persistentDataPath, "WorkshopThumbnailFolder");
        thumbnailFolderPath = thumbnailFolderPath.Replace("\\", "/"); // Normalize to forward slashes

        // Check for thumbnail file with various extensions
        string[] thumbnailExtensions = { ".jpg", ".png", ".gif" };
        string thumbnailFilePath = null;

        if (Directory.Exists(thumbnailFolderPath))
        {
            foreach (string ext in thumbnailExtensions)
            {
                string potentialPath = Path.Combine(thumbnailFolderPath, "thumbnail" + ext).Replace("\\", "/");
                if (File.Exists(potentialPath))
                {
                    thumbnailFilePath = potentialPath;
                    break;
                }
            }

            if (thumbnailFilePath != null)
            {
                Debug.Log("Thumbnail found: " + thumbnailFilePath);
                SteamUGC.SetItemPreview(updateHandle, thumbnailFilePath);
            }
            else
            {
                Debug.LogWarning("No valid thumbnail file found in folder: " + thumbnailFolderPath);
            }
        }
        else
        {
            Debug.LogWarning("Thumbnail folder does not exist: " + thumbnailFolderPath);
        }

        // Define the folder path for WorkshopImageFolder
        string contentFolderPath = Path.Combine(Application.persistentDataPath, "WorkshopImageFolder").Replace("\\", "/");

        // Check if the content folder exists and has files before setting it
        if (Directory.Exists(contentFolderPath))
        {
            if (Directory.GetFiles(contentFolderPath).Length > 0)
            {
                if (!SteamUGC.SetItemContent(updateHandle, contentFolderPath))
                {
                    Debug.LogError("Failed to set item content!");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Content folder is empty: " + contentFolderPath);
            }
        }
        else
        {
            Debug.LogWarning("Content folder does not exist: " + contentFolderPath);
        }

        // Submit the item update
        SteamAPICall_t submitHandle = SteamUGC.SubmitItemUpdate(updateHandle, "Initial upload");
        submitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
        submitItemUpdateResult.Set(submitHandle);

        SetStatusText("Uploading title, description, and preview...", Color.yellow);
    }


    private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t result, bool ioFailure)
    {
        if (ioFailure)
        {
            SetStatusText("Network error during submission.", Color.red);
            Debug.LogError("Network error during submission.");
            return;
        }

        if (result.m_eResult == EResult.k_EResultOK)
        {
            SetStatusText("Upload successful! Workshop Item ID: " + result.m_nPublishedFileId, Color.green);
            Debug.Log("Upload successful! Workshop Item ID: " + result.m_nPublishedFileId);
            // Open the Workshop item's page
            string steamUrl = "steam://url/CommunityFilePage/" + result.m_nPublishedFileId;
            System.Diagnostics.Process.Start(steamUrl);
        }
        else
        {
            SetStatusText("Upload failed: " + result.m_eResult, Color.red);
            Debug.LogError("Upload failed: " + result.m_eResult);
        }
    }

    private void SetStatusText(string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }
}
