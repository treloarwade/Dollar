using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BreakInfinity;

public class DiscordController : MonoBehaviour
{

    public Discord.Discord discord;
    private long clientId = 1259912177196994591; // Replace with your actual Client ID

    public Text counter;     // The UI Text element to display the count
    public ParticleSystem money;
    public ParticleSystem goldenmoney;
    public ParticleSystem briefcaseparticles1;
    public ParticleSystem briefcaseparticles2;

    private bool goldendollarclicked = false;
    public int clickCountOld = 0;
    public double clickCountOld2 = 0;

    public BigDouble clickCount = 0; // Variable to keep track of the number of clicks
    private const string ClickCountKey = "ClickCount";
    private const string ClickCountKeyNew = "ClickCountNew";
    private float updateInterval = 60f; // 1 minute interval
    private float nextUpdateTime = 0f;

    private Discord.ActivityManager activityManager;
    private long startTime;
    private bool discordInitialized = false;
    public SpriteRenderer clicker;
    public UpgradeMenu upgradeMenu;
    private Coroutine repeatCoroutine;
    private bool isRepeating = false;
    public int dollartype;
    public Toggle particletoggle;
    public bool particletoggleon;
    private Coroutine moneyCoroutine; // Reference to the running coroutine
    private bool isCoroutineRunning = false; // Tracks the state of the coroutine

    void Start()
    {
        LoadClickCount();
        atmLevel = LoadLevel("ATM");
        walletLevel = LoadLevel("Wallet");
        cowLevel = LoadLevel("Cow");
        UpdateLevelCostText();
        UpdateMoneyToAdd();
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
        // Initialize the boolean value based on the initial state of the toggle
        if (particletoggle != null)
        {
            particletoggleon = particletoggle.isOn; // Set toggleValue to true if toggle is checked
            particletoggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }
    public void OnDollarClicked()
    {
        // Increment the click count
        switch (dollartype)
        {
            case 100:
                clickCount++;
                break;
            case 102:
                clickCount += 100;
                break;
            case 103:
                clickCount += 1000;
                break;
            case 104:
                clickCount += 10000;
                break;
            case 105:
                clickCount += 100000;
                break;
            case 106:
                clickCount += 1000000;
                break;
            case 112:
                clickCount += 2;
                break;
            default:
                clickCount++;
                break;
        }
        SaveClickCount();
        // Update the counter text
        // Using "F0" format to display the number as a long number without decimal places
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString(); // Display with thousands separator
        }
        if (particletoggleon)
        {
            if (goldendollarclicked)
            {
                goldenmoney.Play();
            }
            else
            {
                money.Play();
            }
        }

    }
    public void ToggleMoneyCoroutine()
    {
        if (!isCoroutineRunning)
        {
            // Start the coroutine if it's not running
            moneyCoroutine = StartCoroutine(AddMoneyOverTime());
            isCoroutineRunning = true;
        }
        else
        {
            // Stop the coroutine if it's running
            if (moneyCoroutine != null)
            {
                StopCoroutine(moneyCoroutine);
                isCoroutineRunning = false;
            }
        }
    }
    public void SetDollarType(int inputdollartype)
    {
        dollartype = inputdollartype;
    }
    public void ClickDollar()
    {
        switch (dollartype)
        {
            case 100:
                OnDollarClicked();
                break;
            case 108:
                OnFrutigerClicked();
                break;
            case 111:
                OnNotADollarClicked();
                break;
            case 122:
                OnDiamondDollarClicked();
                break;
            case 123:
                OnDiamondDollarClicked();
                break;
            default:
                OnDollarClicked();
                break;
        }
    }
    public float moveDuration = 2f; // Initial move duration
    public int atmLevel = 0; // Starting level
    public int walletLevel = 0; // Starting level
    public int cowLevel = 0; // Starting level
    public Text leveltext;
    public Text walletleveltext;
    public Text cowleveltext;

    public long moneyToAdd = 0; // Adjust this value as needed
    public long moneyFromATM = 0;
    public long moneyFromWallet = 0;
    public long moneyFromCow = 0;
    public Text costText;
    public Text walletCostText;
    public Text cowCostText;
    private IEnumerator AddMoneyOverTime()
    {
        while (true)
        {
            // Check if moneyToAdd is zero to avoid division by zero
            if (moneyToAdd <= 0)
            {
                yield return null; // Do nothing and wait for the next frame
                continue;
            }

            // Determine how often to add money
            // Ensure a minimum delay to prevent too frequent updates
            float timeDelay = Mathf.Max(0.1f, 1f / moneyToAdd);

            // Add money at each interval; ensure it's a whole number
            double moneyToAddThisInterval = Mathf.RoundToInt(moneyToAdd * timeDelay);

            // If rounding leads to zero, ensure at least 1 unit is added
            if (moneyToAddThisInterval < 1)
            {
                moneyToAddThisInterval = 1;
            }

            // Add money according to the calculated amount
            clickCount += moneyToAddThisInterval;

            // Update the counter display
            if (clickCount >= 1_000_000_000_000_000)
            {
                counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
            }
            else
            {
                counter.text = clickCount.ToString(); // Display with thousands separator
            }
            ClickDollar();
            // Wait based on the calculated time delay
            yield return new WaitForSeconds(timeDelay);
        }
    }
    private void UpdateMoneyToAdd()
    {
        moneyFromATM = CalculateMoney(atmLevel);
        moneyFromWallet = 5 * CalculateMoney(walletLevel);
        moneyFromCow = 10 * CalculateMoney(cowLevel);
        moneyToAdd = moneyFromATM + moneyFromWallet + moneyFromCow;
    }
    public void UpdateLevelCostText()
    {
        double cost1 = GetLevelUpCost(atmLevel + 1);
        costText.text = "Cost: " + cost1.ToString();
        double cost2 = 100 * GetLevelUpCost(cowLevel + 1);
        cowCostText.text = "Cost: " + cost2.ToString();
        double cost3 = 50 * GetLevelUpCost(walletLevel + 1);
        walletCostText.text = "Cost: " + cost3.ToString();
        leveltext.text = "Level: " + atmLevel;
        walletleveltext.text = "Level: " + walletLevel;
        cowleveltext.text = "Level: " + cowLevel;
    }
    public void ResetLevels()
    {
        atmLevel = 0;
        walletLevel = 0;
        cowLevel = 0;
        UpdateMoneyToAdd();
        UpdateLevelCostText();
        SaveLevel("ATM", atmLevel);
        SaveLevel("Wallet", walletLevel);
        SaveLevel("Cow", cowLevel);
    }
    private int CalculateMoney(int level)
    {
        // Ensure level is at least 1
        if (level == 0)
        {
            return 0;
        }

        // Use the formula 2^(level - 1)
        return (int)Math.Pow(2, level - 1);
    }


    private IEnumerator RepeatFunctionCoroutine()
    {
        while (true) // Replace isRepeating with your condition if needed
        {

            ClickDollar();

            yield return new WaitForSeconds(moveDuration);
        }
    }
    public void SaveLevel(string itemName, int level)
    {
        PlayerPrefs.SetInt(itemName + "_Level", level);
        PlayerPrefs.Save();  // This ensures the data is saved immediately.
    }

    public int LoadLevel(string itemName)
    {
        return PlayerPrefs.GetInt(itemName + "_Level", 1);  // Default to level 1 if no data exists.
    }

    public void LevelUp()
    {
        if (atmLevel < 100)
        {
            double cost = GetLevelUpCost(atmLevel + 1); // Get the cost for the next level



            if (clickCount >= cost)
            {
                clickCount -= cost;
                if (clickCount >= 1_000_000_000_000_000)
                {
                    counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
                }
                else
                {
                    counter.text = clickCount.ToString(); // Display with thousands separator
                }

                atmLevel++;
                SaveLevel("ATM", atmLevel);
                Debug.Log($"Leveled up to {atmLevel}. New move duration: {moneyFromATM}. Clicks remaining: {clickCount}");
                leveltext.text = "Level: " + atmLevel;
                UpdateMoneyToAdd();
            }
            else
            {
                Debug.Log("Not enough clicks to level up.");
            }

        }
        else
        {
            Debug.Log("Maximum level reached.");
        }
        double cost2 = GetLevelUpCost(atmLevel + 1);
        if (cost2 >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            costText.text = "Cost: " + cost2.ToString();
        }

    }
    public void LevelUpWallet()
    {
        if (walletLevel < 100)
        {
            double cost = 50 * GetLevelUpCost(walletLevel + 1); // Get the cost for the next level



            if (clickCount >= cost)
            {
                clickCount -= cost;
                if (clickCount >= 1_000_000_000_000_000)
                {
                    counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
                }
                else
                {
                    counter.text = clickCount.ToString(); // Display with thousands separator
                }

                walletLevel++;
                SaveLevel("Wallet", walletLevel);
                Debug.Log($"Leveled up to {walletLevel}. New move duration: {moneyFromWallet}. Clicks remaining: {clickCount}");
                walletleveltext.text = "Level: " + walletLevel;
                UpdateMoneyToAdd();
            }
            else
            {
                Debug.Log("Not enough clicks to level up.");
            }

        }
        else
        {
            Debug.Log("Maximum level reached.");
        }
        double cost2 = 50 * GetLevelUpCost(walletLevel + 1);
        if (cost2 >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            walletCostText.text = "Cost: " + cost2.ToString();
        }

    }
    public void LevelUpCow()
    {
        if (cowLevel < 100)
        {
            double cost = 100 * GetLevelUpCost(cowLevel + 1); // Get the cost for the next level



            if (clickCount >= cost)
            {
                clickCount -= cost;
                if (clickCount >= 1_000_000_000_000_000)
                {
                    counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
                }
                else
                {
                    counter.text = clickCount.ToString(); // Display with thousands separator
                }

                cowLevel++;
                SaveLevel("Cow", cowLevel);
                Debug.Log($"Leveled up to {cowLevel}. New move duration: {moneyFromCow}. Clicks remaining: {clickCount}");
                cowleveltext.text = "Level: " + cowLevel;
                UpdateMoneyToAdd();
            }
            else
            {
                Debug.Log("Not enough clicks to level up.");
            }

        }
        else
        {
            Debug.Log("Maximum level reached.");
        }
        double cost2 = 100 * GetLevelUpCost(cowLevel + 1);

        if (cost2 >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            cowCostText.text = "Cost: " + cost2.ToString();
        }

    }

    private float GetLevelUpCost(int targetLevel)
    {
        // Define the cost progression
        if (targetLevel < 2) return 0;
        return Mathf.Pow(5, targetLevel - 2);


    }
    // Function to toggle the coroutine
    public void ToggleRepeatingFunction()
    {
        if (isRepeating)
        {
            StopRepeatingFunction();
        }
        else
        {
            StartRepeatingFunction();
        }
    }

    // Starts the coroutine
    private void StartRepeatingFunction()
    {
        if (!isRepeating)
        {
            isRepeating = true;
            repeatCoroutine = StartCoroutine(RepeatFunctionCoroutine());
        }
    }

    // Stops the coroutine
    private void StopRepeatingFunction()
    {
        if (isRepeating)
        {
            isRepeating = false;
            if (repeatCoroutine != null)
            {
                StopCoroutine(repeatCoroutine);
                repeatCoroutine = null;
            }
        }
    }
    public void OnTwoDollarClicked()
    {
        // Increment the click count
        clickCount += 2;
        SaveClickCount();
        if (clickCount > 9999999999999999)
        {
            clickCount = 0;
        }
        // Update the counter text
        // Using "F0" format to display the number as a long number without decimal places
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString(); // Display with thousands separator
        }
        if (goldendollarclicked)
        {
            goldenmoney.Play();
        }
        else
        {
            money.Play();
        }
    }
    public ParticleSystem fish1;
    public ParticleSystem fish2;
    public ParticleSystem fish3;
    public ParticleSystem diamond1;
    public ParticleSystem diamond2;
    public void OnDiamondDollarClicked()
    {
        // Increment the click count
        clickCount++;
        SaveClickCount();
        if (clickCount > 9999999999999999)
        {
            clickCount = 0;
        }
        // Update the counter text
        // Using "F0" format to display the number as a long number without decimal places
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString(); // Display with thousands separator
        }
        if (particletoggleon)
        {
            diamond1.Play();
            diamond2.Play();
        }


    }
    public void OnFrutigerClicked()
    {
        // Increment the click count
        clickCount++;
        SaveClickCount();
        if (clickCount > 9999999999999999)
        {
            clickCount = 0;
        }
        // Update the counter text
        // Using "F0" format to display the number as a long number without decimal places
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString(); // Display with thousands separator
        }
        if (particletoggleon)
        {
            PlayParticleWithChance(fish1, 0.5f);
            PlayParticleWithChance(fish2, 0.5f);
            PlayParticleWithChance(fish3, 0.5f);
            PlayParticleWithChance(frutigerphone, 0.8f);
        }

    }

    private void PlayParticleWithChance(ParticleSystem particleSystem, float percentchance)
    {
        // 50% chance to play the particle system
        if (UnityEngine.Random.value > percentchance)
        {
            particleSystem.Play();
        }
    }
    public ParticleSystem frutigerphone;
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
        PlayerPrefs.SetString(ClickCountKeyNew, clickCount.ToString());
        PlayerPrefs.Save(); // Make sure to save the PlayerPrefs
    }

    private void LoadClickCount()
    {

        clickCountOld = PlayerPrefs.GetInt(ClickCountKey);
        clickCountOld2 = (double)PlayerPrefs.GetFloat(ClickCountKey);
        string clickCountString = PlayerPrefs.GetString(ClickCountKeyNew, "0");
        clickCount = BigDouble.Parse(clickCountString);
        clickCount = clickCount + clickCountOld + clickCountOld2;
        PlayerPrefs.SetFloat(ClickCountKey, 0);
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString(); // Display with thousands separator
        }
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
        briefcaseparticles1.Play();
        briefcaseparticles2.Play();
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
    private void OnToggleChanged(bool isOn)
    {
        particletoggleon = isOn; // Update toggleValue based on the toggle's state
        Debug.Log($"Toggle value changed to: {particletoggleon}");
    }

    // Method to get the current value of the boolean
    public bool GetToggleValue()
    {
        return particletoggleon;
    }


    public void TryToFix()
    {
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
    private bool briefcaseclicked = false;
    private bool goldenbriefcaseclicked = false;
    private string mostrecentitem;
    public void BriefcaseActivity(string itemname, int lootboxtype)
    {
        mostrecentitem = itemname;
        if (lootboxtype == 1)
        {
            briefcaseclicked = true;
        }
        else if (lootboxtype == 2)
        {
            goldenbriefcaseclicked = true;
        }
    }

    private void UpdateActivity()
    {
        if (!discordInitialized) return;

        var activity = new Discord.Activity();
        var party = new Discord.PartySize()
        {
            CurrentSize = 1, // Replace with actual current party size
            MaxSize = 10 // Replace with actual maximum party size
        };
        if (goldenbriefcaseclicked)
        {
            activity.State = "Opened a Golden Briefcase";

            activity.Details = "Recieved a " + mostrecentitem;
            activity.Timestamps.Start = startTime;

            activity.Assets.LargeImage = "goldenbriefcase";
            activity.Assets.LargeText = "Dollar";
            activity.Party = new Discord.ActivityParty
            {
                Id = "party_id", // Replace with actual party ID
                Size = new Discord.PartySize
                {
                    CurrentSize = 1, // Replace with actual current party size
                    MaxSize = 2 // Replace with actual maximum party size
                }
            };

            // Example of setting secrets if supported
            activity.Secrets = new Discord.ActivitySecrets
            {
                Join = "join_secret", // Replace with actual join secret
                Spectate = "spectate_secret", // Replace with actual spectate secret
                Match = "match_secret" // Replace with actual match secret
            };
        }
        else if (briefcaseclicked)
        {
            activity.State = "Opened a Briefcase";
            activity.Details = "Recieved a " + mostrecentitem;
            activity.Timestamps.Start = startTime;

            activity.Assets.LargeImage = "briefcase";
            activity.Assets.LargeText = "Dollar";
            activity.Party = new Discord.ActivityParty
            {
                Id = "party_id", // Replace with actual party ID
                Size = new Discord.PartySize
                {
                    CurrentSize = 1, // Replace with actual current party size
                    MaxSize = 2 // Replace with actual maximum party size
                }
            };

            // Example of setting secrets if supported
            activity.Secrets = new Discord.ActivitySecrets
            {
                Join = "join_secret", // Replace with actual join secret
                Spectate = "spectate_secret", // Replace with actual spectate secret
                Match = "match_secret" // Replace with actual match secret
            };
        }
        else
        {
            activity.State = goldendollarclicked ? "Clicking the Golden Dollar" : (clickCount == 0 ? "Hasn't clicked the dollar yet" : "Clicked the Dollar " + clickCount + " times");
            activity.Details = "Getting some nice drops";
            activity.Timestamps.Start = startTime;
            activity.Assets.LargeImage = goldendollarclicked ? "goldendollar" : "dollar";
            activity.Assets.LargeText = "Dollar";
            // Add buttons if supported
            // Add custom lobby data if applicable
            activity.Party = new Discord.ActivityParty
            {
                Id = "party_id", // Replace with actual party ID
                Size = new Discord.PartySize
                {
                    CurrentSize = 1, // Replace with actual current party size
                    MaxSize = 2 // Replace with actual maximum party size
                }
            };

            // Example of setting secrets if supported
            activity.Secrets = new Discord.ActivitySecrets
            {
                Join = "join_secret", // Replace with actual join secret
                Spectate = "spectate_secret", // Replace with actual spectate secret
                Match = "match_secret" // Replace with actual match secret
            };
        }



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
