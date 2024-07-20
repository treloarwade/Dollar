using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject Menu; // Assign this in the Inspector
    public Toggle toggle; // Reference to your UI toggle
    public Text autoclickerleveltext;
    public int autoclickerlevel;
    public DiscordController discordController;

    private void Start()
    {
        // Initialize the autoclicker level text
        UpdateAutoclickerLevelText();
    }

    public void BuyAutoclicker()
    {
        if (discordController.clickCount >= 100)
        {
            discordController.clickCount -= 100;
            autoclickerlevel++;
            UpdateAutoclickerLevelText();
        }
    }

    private void UpdateAutoclickerLevelText()
    {
        autoclickerleveltext.text = "Level " + autoclickerlevel.ToString();
    }
    public int GetAutoclickerLevel()
    {
        return autoclickerlevel;
    }
}
