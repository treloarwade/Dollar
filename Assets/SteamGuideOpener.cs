using Steamworks;
using UnityEngine;

public class SteamGuideOpener : MonoBehaviour
{
    public void OpenSteamGuide()
    {
        string guideURL = "https://steamcommunity.com/sharedfiles/filedetails/?id=3336159022";

        if (SteamManager.Initialized) // Ensure Steamworks is initialized
        {
            SteamFriends.ActivateGameOverlayToWebPage(guideURL);
        }
        else
        {
            Debug.LogError("Steam is not initialized!");
        }
    }
}
