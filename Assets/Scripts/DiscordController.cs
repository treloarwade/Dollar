using System;
using UnityEngine;
using UnityEngine.UI;
using Discord;
using System.Collections;

public class DiscordController : MonoBehaviour
{
    public Discord.Discord discord;
    private long clientId = 1259912177196994591; // Replace with your actual Client ID

    public Text counter;     // The UI Text element to display the count
    public ParticleSystem money;
    public ParticleSystem goldenmoney;
    private bool goldendollarclicked = false;
    public int clickCountOld = 0;
    public float clickCount = 0; // Variable to keep track of the number of clicks
    private const string ClickCountKey = "ClickCount";

    private float updateInterval = 60f; // 1 minute interval
    private float nextUpdateTime = 0f;

    private Discord.ActivityManager activityManager;
    private long startTime;
    private bool discordInitialized = false;
    public Image clicker;
    public UpgradeMenu upgradeMenu;

    public void OnDollarClicked()
    {
        // Increment the click count
        clickCount++;
        SaveClickCount();
        // Update the counter text
        counter.text = clickCount.ToString();
        if(goldendollarclicked )
        {
            goldenmoney.Play();
        }
        else
        {
            money.Play();
        }
    }
    public void OnTwoDollarClicked()
    {
        // Increment the click count
        clickCount += 2;
        SaveClickCount();
        // Update the counter text
        counter.text = clickCount.ToString();
        if (goldendollarclicked)
        {
            goldenmoney.Play();
        }
        else
        {
            money.Play();
        }
    }
    public void OnNotADollarClicked()
    {
        counter.text = "This isn't a dollar";
        StartCoroutine(ClickNotaDollar());
    }
    IEnumerator ClickNotaDollar()
    {
        yield return new WaitForSeconds(2f);
        counter.text = "";
        yield return null;
    }

    private void SaveClickCount()
    {
        PlayerPrefs.SetFloat(ClickCountKey, clickCount);
        PlayerPrefs.Save();
    }

    private void LoadClickCount()
    {
        if (PlayerPrefs.HasKey(ClickCountKey))
        {
            clickCountOld = PlayerPrefs.GetInt(ClickCountKey);
            clickCount = PlayerPrefs.GetFloat(ClickCountKey);
            clickCount = clickCount + clickCountOld;
        }
        counter.text = clickCount.ToString();
    }
    public void OnGoldenDollarClicked()
    {
        // Increment the click count
        clickCount++;

        // Update the counter text
        counter.text = clickCount.ToString();
        goldenmoney.Play();
        goldendollarclicked = true;
        ChangeImageToGoldenDollar();
    }

    private void ChangeImageToGoldenDollar()
    {
        // Load the new sprite from the Resources folder
        Sprite newSprite = Resources.Load<Sprite>("sprite_101");

        // Check if the sprite was successfully loaded
        if (newSprite != null)
        {
            // Assign the new sprite to the target Image component
            clicker.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("The sprite 'goldendollar' could not be found in the Resources folder.");
        }
    }
    void Start()
    {
        LoadClickCount();

        try
        {
            discord = new Discord.Discord(clientId, (ulong)Discord.CreateFlags.NoRequireDiscord);
            activityManager = discord.GetActivityManager();
            startTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            discordInitialized = true;

            UpdateActivity();
        }
        catch (Exception e)
        {
            Debug.LogError("Discord initialization failed: " + e.Message);
        }
    }

    void Update()
    {
        if (!discordInitialized) return;

        try
        {
            discord.RunCallbacks();

            if (Time.time >= nextUpdateTime)
            {
                UpdateActivity();
                nextUpdateTime = Time.time + updateInterval;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Discord run callbacks failed: " + e.Message);
        }

        
    }

    private void UpdateActivity()
    {
        if (!discordInitialized) return;

        var activity = new Discord.Activity
        {
            State = goldendollarclicked ? "Clicking the Golden Dollar" : (clickCount == 0 ? "Hasn't clicked the dollar yet" : "Clicked the Dollar " + clickCount + " times"),
            Details = "Getting some nice drops",
            Timestamps =
        {
            Start = startTime
        },
            Assets =
        {
            LargeImage = goldendollarclicked ? "goldendollar" : "dollar", // Change image based on goldendollarclicked
            LargeText = "Dollar"
        }
        };

        activityManager.UpdateActivity(activity, result =>
        {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Successfully updated activity");
            }
            else
            {
                Debug.LogError("Failed to update activity: " + result);
            }
        });
    }


    void OnApplicationQuit()
    {
        if (discordInitialized)
        {
            discord.Dispose();
        }
    }
}
