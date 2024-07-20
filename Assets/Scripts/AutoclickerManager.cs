using System.Collections;
using UnityEngine;

public class AutoclickerManager : MonoBehaviour
{
    public UpgradeMenu upgradeMenu; // Assign this in the Inspector
    public DiscordController discordController; // Assign this in the Inspector

    private void Start()
    {
        // Start the coroutine to increase click count periodically
        StartCoroutine(IncreaseClickCount());
    }

    private IEnumerator IncreaseClickCount()
    {
        while (true)
        {
            float autoclickerLevel = (float)upgradeMenu.GetAutoclickerLevel();
            discordController.clickCount += autoclickerLevel * 0.2f;
            yield return new WaitForSeconds(1f);
        }
    }
}