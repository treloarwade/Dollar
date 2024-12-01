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
    public bool isAutumn = false;  // Flag to indicate if it's autumn or not
    public GameObject moneyTree1;   // Reference to the Money Tree GameObject
    public GameObject moneyTree2;   // Reference to the Money Tree GameObject
    public GameObject moneyTree3;   // Reference to the Money Tree GameObject

    public GameObject moneyTree4;   // Reference to the Money Tree GameObject

    private Sprite fallTreeSprite; // Sprite for autumn (fall)
    private Sprite treeSprite;     // Sprite for non-autumn (regular tree)
    public ParticleSystem leaves1;
    public ParticleSystem leaves2;
    public ParticleSystem leaves3;
    public ParticleSystem money2;
    public ParticleSystem autumnleaves1;
    public ParticleSystem autumnleaves2;
    public ParticleSystem autumnleaves3;
    public ParticleSystem autumnleaves4;
    public ParticleSystem ice1;
    public ParticleSystem ice2;



    void Start()
    {
        LoadClickCount();
        int LoadLevelWithMinimum(string key, int minValue = 1)
        {
            int value = LoadLevel(key);
            return Mathf.Max(value, minValue);
        }

        numberofATMs = LoadLevelWithMinimum("numberofATMs");
        numberofCows = LoadLevelWithMinimum("numberofCows");
        numberofCarts = LoadLevelWithMinimum("numberofCarts");
        numberofTrees = LoadLevelWithMinimum("numberofTrees");
        numberofPots = LoadLevelWithMinimum("numberofPots");


        atmLevel = LoadLevel("ATM");
        walletLevel = LoadLevel("Wallet");
        cowLevel = LoadLevel("Cow");
        treeLevel = LoadLevel("Tree");
        mineLevel = LoadLevel("Mine");
        potofgoldLevel = LoadLevel("Potofgold");
        UpdateLevelCostText();
        UpdateMoneyToAdd();
        CheckAndRemoveLockedIcons();
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
        // Load the tree and fall tree sprites from the Resources folder
        fallTreeSprite = Resources.Load<Sprite>("falltree");
        treeSprite = Resources.Load<Sprite>("tree");

        // Set the initial sprite based on the autumn flag
        UpdateMoneyTreeSprite();
    }
    public void SetAutumn(bool autumn)
    {
        isAutumn = autumn;
        UpdateMoneyTreeSprite();  // Update the sprite whenever autumn state changes
        if (leaves1.isPlaying)
        {
            if (autumn)
            {
                leaves1.Stop();
                leaves2.Stop();
                leaves3.Stop();
            }
        }
        if (autumnleaves1.isPlaying)
        {
            if(!autumn)
            {
                leaves1.Play();
                leaves2.Play();
                leaves3.Play();
            }
        }
    }
    public void ToggleTree()
    {
        if (treeLevel > 0)
        {
            moneyTree1.SetActive(!moneyTree1.activeSelf);
            if (moneyTree1.activeSelf)
            {
                if (isAutumn)
                {
                    leaves1.Stop();
                    leaves2.Stop();
                    leaves3.Stop();
                    money2.Play();
                }
                else
                {
                    leaves1.Play();
                    leaves2.Play();
                    leaves3.Play();
                    money2.Play();
                }
            }
        }
    }
    private int currentATMCount = 0;       // Tracks how many ATMs are currently enabled
    private int currentCowCount = 0;       // Tracks how many ATMs are currently enabled
    private int currentCartCount = 0;       // Tracks how many ATMs are currently enabled
    private int currentTreeCount = 0;       // Tracks how many ATMs are currently enabled
    private int currentPotCount = 0;       // Tracks how many ATMs are currently enabled


    public void IncreaseATMCount()
    {
        if (atmLevel > 0 && currentATMCount < upgradeMenu.atmPhysicsItems.Length && currentATMCount < numberofATMs)
        {
            currentATMCount++;
            upgradeMenu.ToggleATMs(currentATMCount);
            Debug.Log($"Increased ATM count to {currentATMCount}");
        }
    }

    // Method to decrease the number of enabled ATMs
    public void DecreaseATMCount()
    {
        if (atmLevel > 0 && currentATMCount > 0)
        {
            currentATMCount--;
            upgradeMenu.ToggleATMs(currentATMCount);
            Debug.Log($"Decreased ATM count to {currentATMCount}");
        }
    }
    public void IncreaseCowCount()
    {
        if (cowLevel > 0 && currentCowCount < upgradeMenu.cowPhysicsItems.Length && currentCowCount < numberofCows)
        {
            currentCowCount++;
            upgradeMenu.ToggleCows(currentCowCount);
            Debug.Log($"Increased ATM count to {currentCowCount}");
        }
    }

    // Method to decrease the number of enabled ATMs
    public void DecreaseCowCount()
    {
        if (cowLevel > 0 && currentCowCount > 0)
        {
            currentCowCount--;
            upgradeMenu.ToggleCows(currentCowCount);
            Debug.Log($"Decreased ATM count to {currentCowCount}");
        }
    }


    // Method to decrease the number of enabled ATMs
    public void DecreaseTreeCount()
    {
        if (treeLevel > 0 && currentTreeCount > 0)
        {
            currentTreeCount--;
            upgradeMenu.ToggleTrees(currentTreeCount);
            Debug.Log($"Decreased ATM count to {currentTreeCount}");
        }
    }
    public void IncreaseTreeCount()
    {
        if (treeLevel > 0 && currentTreeCount < upgradeMenu.treePhysicsItems.Length && currentTreeCount < numberofTrees)
        {
            currentTreeCount++;
            upgradeMenu.ToggleTrees(currentTreeCount);
            Debug.Log($"Increased ATM count to {currentTreeCount}");
        }
    }
    public void IncreaseCartCount()
    {
        if (mineLevel > 0 && currentCartCount < upgradeMenu.cartPhysicsItems.Length && currentCartCount < numberofCarts)
        {
            currentCartCount++;
            upgradeMenu.ToggleCarts(currentCartCount);
            Debug.Log($"Increased ATM count to {currentCartCount}");
        }
    }
    // Method to decrease the number of enabled ATMs
    public void DecreaseCartCount()
    {
        if (mineLevel > 0 && currentCartCount > 0)
        {
            currentCartCount--;
            upgradeMenu.ToggleCarts(currentCartCount);
            Debug.Log($"Decreased ATM count to {currentCartCount}");
        }
    }
    public void IncreaseGoldCount()
    {
        if (potofgoldLevel > 0 && currentPotCount < upgradeMenu.goldPhysicsItems.Length && currentPotCount < numberofPots)
        {
            currentPotCount++;
            upgradeMenu.TogglePots(currentPotCount);
            Debug.Log($"Increased ATM count to {currentPotCount}");
        }
    }
    // Method to decrease the number of enabled ATMs
    public void DecreaseGoldCount()
    {
        if (potofgoldLevel > 0 && currentPotCount > 0)
        {
            currentPotCount--;
            upgradeMenu.TogglePots(currentPotCount);
            Debug.Log($"Decreased ATM count to {currentPotCount}");
        }
    }
    public void ToggleATM()
    {
        if (atmLevel > 0)
        {
            IncreaseATMCount();
        }
    }
    public void ToggleCow()
    {
        if (cowLevel > 0)
        {
            upgradeMenu.ToggleCowPhysicsItem();
        }
    }
    public void TogglePotOfGold()
    {
        if (potofgoldLevel > 0)
        {
            upgradeMenu.TogglePotOfGoldPhysicsItem();
        }
    }
    public void ToggleMinecart()
    {
        if (mineLevel > 0)
        {
            upgradeMenu.ToggleMinecartPhysicsItem();
        }
    }
    // Function to update the Money Tree sprite based on the autumn flag
    private void UpdateMoneyTreeSprite()
    {
        if (moneyTree1 != null)
        {
            SpriteRenderer spriteRenderer = moneyTree1.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // If it's autumn, change to falltree sprite, otherwise use the regular tree sprite
                spriteRenderer.sprite = isAutumn ? fallTreeSprite : treeSprite;
            }
            else
            {
                Debug.LogError("No SpriteRenderer found on the moneyTree GameObject.");
            }
        }
        else
        {
            Debug.LogError("MoneyTree GameObject is not assigned.");
        }
        if (moneyTree2 != null)
        {
            Image spriteRenderer = moneyTree2.GetComponent<Image>();
            if (spriteRenderer != null)
            {
                // If it's autumn, change to falltree sprite, otherwise use the regular tree sprite
                spriteRenderer.sprite = isAutumn ? fallTreeSprite : treeSprite;
            }
            else
            {
                Debug.LogError("No SpriteRenderer found on the moneyTree GameObject.");
            }
        }
        else
        {
            Debug.LogError("MoneyTree GameObject is not assigned.");
        }
        if (moneyTree3 != null)
        {
            SpriteRenderer spriteRenderer = moneyTree3.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // If it's autumn, change to falltree sprite, otherwise use the regular tree sprite
                spriteRenderer.sprite = isAutumn ? fallTreeSprite : treeSprite;
            }
            else
            {
                Debug.LogError("No SpriteRenderer found on the moneyTree GameObject.");
            }
        }
        else
        {
            Debug.LogError("MoneyTree GameObject is not assigned.");
        }
        if (moneyTree4 != null)
        {
            SpriteRenderer spriteRenderer = moneyTree4.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // If it's autumn, change to falltree sprite, otherwise use the regular tree sprite
                spriteRenderer.sprite = isAutumn ? fallTreeSprite : treeSprite;
            }
            else
            {
                Debug.LogError("No SpriteRenderer found on the moneyTree GameObject.");
            }
        }
        else
        {
            Debug.LogError("MoneyTree GameObject is not assigned.");
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
                OnDollarClicked(1); // Increment by 1
                break;
            case 102:
                OnDollarClicked(100); // Increment by 100
                break;
            case 103:
                OnDollarClicked(1000); // Increment by 1000
                break;
            case 104:
                OnDollarClicked(10000); // Increment by 10000
                break;
            case 105:
                OnDollarClicked(100000); // Increment by 100000
                break;
            case 106:
                OnDollarClicked(1000000); // Increment by 1000000
                break;
            case 108:
                OnFrutigerClicked(1);
                break;
            case 111:
                OnNotADollarClicked();
                break;
            case 122:
                OnDiamondDollarClicked(1);
                break;
            case 123:
                OnDiamondDollarClicked(1);
                break;
            case 141:
                OnIceDollarClicked(1);
                break;
            default:
                OnDollarClicked(1);
                break;
        }
    }
    public void ClickWithNoIncome()
    {
        switch (dollartype)
        {
            case 100:
                OnDollarClicked(0); // Increment by 1
                break;
            case 102:
                OnDollarClicked(0); // Increment by 100
                break;
            case 103:
                OnDollarClicked(0); // Increment by 1000
                break;
            case 104:
                OnDollarClicked(0); // Increment by 10000
                break;
            case 105:
                OnDollarClicked(0); // Increment by 100000
                break;
            case 106:
                OnDollarClicked(0); // Increment by 1000000
                break;
            case 108:
                OnFrutigerClicked(0);
                break;
            case 122:
                OnDiamondDollarClicked(0);
                break;
            case 123:
                OnDiamondDollarClicked(0);
                break;
            case 141:
                OnIceDollarClicked(0);
                break;
            default:
                OnDollarClicked(0);
                break;
        }
    }
    public void OnDollarClicked(int incrementAmount)
    {
        // Increment click count by the provided amount
        clickCount += incrementAmount;

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
        if (particletoggleon && clicker.enabled)
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
    public float moveDuration = 2f; // Initial move duration
    public int atmLevel = 0; // Starting level
    public int walletLevel = 0; // Starting level
    public int cowLevel = 0; // Starting level
    public int treeLevel = 0; // Starting level
    public int mineLevel = 0; // Starting level
    public int potofgoldLevel = 0; // Starting level

    public Text atmnumberLevelText;
    public Text cownumberLevelText;
    public Text cartnumberLevelText;
    public Text treenumberLevelText;
    public Text goldnumberLevelText;

    public Text atmLevelText;
    public Text walletLevelText;
    public Text cowLevelText;
    public Text treeLevelText;
    public Text mineLevelText;
    public Text potofgoldLevelText;

    public Text atmnumberCostText;
    public Text cownumberCostText;
    public Text cartnumberCostText;
    public Text treenumberCostText;
    public Text goldnumberCostText;

    public Text atmCostText;
    public Text walletCostText;
    public Text cowCostText;
    public Text treeCostText;
    public Text mineCostText;
    public Text potofgoldCostText;


    public BigDouble moneyToAdd = 0; // Adjust this value as needed
    public BigDouble moneyFromATM = 0;
    public BigDouble moneyFromWallet = 0;
    public BigDouble moneyFromCow = 0;
    public BigDouble moneyFromTree = 0;
    public BigDouble moneyFromMine = 0;
    public BigDouble moneyFromPotofgold = 0;


    private IEnumerator AddMoneyOverTime()
    {
        while (true)
        {
            // Add the calculated amount to clickCount
            clickCount += moneyToAdd;

            // Update the counter display with appropriate formatting
            if (clickCount >= 1_000_000_000_000_000) // Greater than or equal to a quadrillion
            {
                counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
            }
            else
            {
                // Use ToString("N0") for thousands separator
                counter.text = clickCount.ToString();
            }

            ClickWithNoIncome(); // Call your existing function to process the click
            yield return new WaitForSeconds(0.5f); // Wait based on the calculated time delay
        }
    }
    public GameObject atmLockedIcon;   // Assign the ATM locked icon in the Inspector
    public GameObject walletLockedIcon; // Assign the Wallet locked icon in the Inspector
    public GameObject cowLockedIcon;    // Assign the Cow locked icon in the Inspector
    public GameObject treeLockedIcon;    // Assign the Cow locked icon in the Inspector
    public GameObject mineLockedIcon;    // Assign the Cow locked icon in the Inspector
    public GameObject potofgoldLockedIcon;    // Assign the Cow locked icon in the Inspector



    // Call this function to check and remove locked icons based on level
    public void CheckAndRemoveLockedIcons()
    {
        if (atmLevel > 0 && atmLockedIcon != null)
        {
            atmLockedIcon.SetActive(false);  // Disable ATM locked icon if level is greater than 0
        }

        if (walletLevel > 0 && walletLockedIcon != null)
        {
            walletLockedIcon.SetActive(false);  // Disable Wallet locked icon if level is greater than 0
        }

        if (cowLevel > 0 && cowLockedIcon != null)
        {
            cowLockedIcon.SetActive(false);  // Disable Cow locked icon if level is greater than 0
        }

        if (treeLevel > 0 && treeLockedIcon != null)
        {
            treeLockedIcon.SetActive(false);  // Disable Cow locked icon if level is greater than 0
        }
        if (mineLevel > 0 && mineLockedIcon != null)
        {
            mineLockedIcon.SetActive(false);  // Disable Cow locked icon if level is greater than 0
        }
        if (potofgoldLevel > 0 && potofgoldLockedIcon != null)
        {
            potofgoldLockedIcon.SetActive(false);  // Disable Cow locked icon if level is greater than 0
        }
    }
    public int numberofATMs;
    public int numberofCows;
    public int numberofCarts;
    public int numberofTrees;
    public int numberofPots;


    private void UpdateMoneyToAdd()
    {
        moneyFromATM = CalculateMoney(atmLevel);
        moneyFromWallet = 3 * CalculateMoney(walletLevel);
        moneyFromCow = 8 * CalculateMoney(cowLevel);
        moneyFromTree = 20 * CalculateMoney(treeLevel);
        moneyFromMine = 38 * CalculateMoney(mineLevel);
        moneyFromPotofgold = 59 * CalculateMoney(potofgoldLevel);
        moneyToAdd = (numberofATMs * moneyFromATM) + (moneyFromWallet * numberofATMs) + (numberofCows * moneyFromCow) + (numberofTrees * moneyFromTree) + (numberofCarts * moneyFromMine) + (numberofPots * moneyFromPotofgold);
    }
    public void UpdateLevelCostText()
    {
        BigDouble cost11 = 1000 * GetLevelUpCost(numberofATMs + 1);
        atmnumberCostText.text = FormatCostText(cost11);
        BigDouble cost12 = 300000 * GetLevelUpCost(numberofCows + 1);
        cownumberCostText.text = FormatCostText(cost12);
        BigDouble cost13 = 50000000 * GetLevelUpCost(numberofCarts + 1);
        cartnumberCostText.text = FormatCostText(cost13);
        BigDouble cost14 = 4000000 * GetLevelUpCost(numberofTrees + 1);
        treenumberCostText.text = FormatCostText(cost14);
        BigDouble cost15 = 600000000 * GetLevelUpCost(numberofPots + 1);
        goldnumberCostText.text = FormatCostText(cost15);
        BigDouble cost1 = 10 * GetLevelUpCost(atmLevel + 1);
        atmCostText.text = FormatCostText(cost1);
        BigDouble cost2 = 3000 * GetLevelUpCost(cowLevel + 1);
        cowCostText.text = FormatCostText(cost2);
        BigDouble cost3 = 200 * GetLevelUpCost(walletLevel + 1);
        walletCostText.text = FormatCostText(cost3);
        BigDouble cost4 = 40000 * GetLevelUpCost(treeLevel + 1);
        treeCostText.text = FormatCostText(cost4);
        BigDouble cost5 = 500000 * GetLevelUpCost(mineLevel + 1);
        mineCostText.text = FormatCostText(cost5);
        BigDouble cost6 = 6000000 * GetLevelUpCost(potofgoldLevel + 1);
        potofgoldCostText.text = FormatCostText(cost6);
        // For ATM Level text
        atmnumberLevelText.text = FormatNumberOrUnlockText(atmLevel, numberofATMs, "ATMs");
        cownumberLevelText.text = FormatNumberOrUnlockText(cowLevel, numberofCows, "Cows");
        cartnumberLevelText.text = FormatNumberOrUnlockText(mineLevel, numberofCarts, "Carts");
        treenumberLevelText.text = FormatNumberOrUnlockText(treeLevel, numberofTrees, "Trees");
        goldnumberLevelText.text = FormatNumberOrUnlockText(potofgoldLevel, numberofPots, "Pots");

        // Update level texts
        atmLevelText.text = FormatLevelText(atmLevel);
        walletLevelText.text = FormatLevelText(walletLevel);
        cowLevelText.text = FormatLevelText(cowLevel);
        treeLevelText.text = FormatLevelText(treeLevel);
        mineLevelText.text = FormatLevelText(mineLevel);
        potofgoldLevelText.text = FormatLevelText(potofgoldLevel);
    }
    string FormatNumberOrUnlockText(int level, int count, string itemName)
    {
        return level == 0 ? "Unlock" : $"{itemName}: {count}";
    }

    string FormatLevelText(int level)
    {
        return level == 0 ? "Unlock" : $"Level: {level}";
    }
    private string FormatCostText(BigDouble cost)
    {
        if (cost >= 10_000_000_000_000)
        {
            return (cost / 1_000_000_000f).ToString("F0") + " billion";
        }
        else
        {
            return cost.ToString();
        }
    }
    public void ResetLevels()
    {
        atmLevel = 0;
        walletLevel = 0;
        cowLevel = 0;
        treeLevel = 0;
        mineLevel = 0;
        potofgoldLevel = 0;
        numberofATMs = 1;
        numberofCows = 1;
        numberofCarts = 1;
        numberofTrees = 1;
        numberofPots = 1;
        atmLockedIcon.SetActive(true);  // Disable ATM locked icon if level is greater than 0
        walletLockedIcon.SetActive(true);  // Disable Wallet locked icon if level is greater than 0
        cowLockedIcon.SetActive(true);  // Disable Cow locked icon if level is greater than 0
        treeLockedIcon.SetActive(true);
        mineLockedIcon.SetActive(true);
        potofgoldLockedIcon.SetActive(true);
        UpdateMoneyToAdd();
        SaveLevel("numberofATMs", numberofATMs);
        SaveLevel("ATM", atmLevel);
        SaveLevel("Wallet", walletLevel);
        SaveLevel("numberofCows", numberofCows);
        SaveLevel("Cow", cowLevel);
        SaveLevel("Tree", treeLevel);
        SaveLevel("numberofCarts", numberofCarts);
        SaveLevel("Mine", mineLevel);
        SaveLevel("Potofgold", potofgoldLevel);
        SaveLevel("numberofTrees", numberofTrees);
        SaveLevel("numberofPots", numberofPots);

        UpdateLevelCostText();
    }
    private BigDouble CalculateMoney(int level)
    {
        // Ensure level is at least 1
        if (level == 0)
        {
            return 0;
        }

        // Use controlled exponential growth (e.g., base 1.5)
        // Round up the result to the next whole number using Math.Ceiling
        return (BigDouble)Math.Ceiling(Math.Pow(1.4, level - 1));  // Change the base (e.g., 1.5) to your preference
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
        return PlayerPrefs.GetInt(itemName + "_Level", 0);  // Default to level 1 if no data exists.
    }

    public void LevelUp(ref int level, string levelName, BigDouble costMultiplier, Text levelText, Text costText)
    {
        if (level < 100)
        {
            BigDouble cost = costMultiplier * GetLevelUpCost(level + 1); // Get the cost for the next level

            if (clickCount >= cost)
            {
                clickCount -= cost;
                UpdateCounter();

                level++;
                SaveLevel(levelName, level);
                Debug.Log($"Leveled up to {level}. Clicks remaining: {clickCount}");
                levelText.text = "Level: " + level;
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

        UpdateLevelCostText();
        CheckAndRemoveLockedIcons();
    }
    // Function to update the counter display
    private void UpdateCounter()
    {
        if (clickCount >= 1_000_000_000_000_000)
        {
            counter.text = (clickCount / 1_000_000f).ToString("F0") + " million";
        }
        else
        {
            counter.text = clickCount.ToString();
        }
    }

    // Example usage for specific levels
    public void LevelUpNumberOfATM()
    {
        if (atmLevel == 0)
        {
            LevelUpATM();
            return;
        }
        LevelUp(ref numberofATMs, "numberofATMs", 1000, atmnumberLevelText, atmnumberCostText);
    }
    public void LevelUpNumberOfCow()
    {
        if (cowLevel == 0)
        {
            LevelUpCow();
            return;
        }
        LevelUp(ref numberofCows, "numberofCows", 300000, cownumberLevelText, cownumberCostText);
    }
    public void LevelUpNumberOfCart()
    {
        if (mineLevel == 0)
        {
            LevelUpMine();
            return;
        }
        LevelUp(ref numberofCarts, "numberofCarts", 50000000, cartnumberLevelText, cartnumberCostText);
    }
    public void LevelUpNumberOfTree()
    {
        if (treeLevel == 0)
        {
            LevelUpTree();
            return;
        }
        LevelUp(ref numberofTrees, "numberofTrees", 4000000, treenumberLevelText, treenumberCostText);

    }
    public void LevelUpNumberOfPots()
    {
        if (potofgoldLevel == 0)
        {
            LevelUpPotOfGold();
            return;
        }
        LevelUp(ref numberofPots, "numberofPots", 600000000, goldnumberLevelText, goldnumberCostText);

    }

    public void LevelUpATM()
    {
        LevelUp(ref atmLevel, "ATM", 10, atmLevelText, atmCostText);
    }

    public void LevelUpWallet()
    {
        LevelUp(ref walletLevel, "Wallet", 200, walletLevelText, walletCostText);
    }

    public void LevelUpCow()
    {
        LevelUp(ref cowLevel, "Cow", 3000, cowLevelText, cowCostText);
    }
    public void LevelUpTree()
    {
        LevelUp(ref treeLevel, "Tree", 40000, treeLevelText, treeCostText);
    }
    public void LevelUpMine()
    {
        LevelUp(ref mineLevel, "Mine", 500000, mineLevelText, mineCostText);
    }
    public void LevelUpPotOfGold()
    {
        LevelUp(ref potofgoldLevel, "Potofgold", 6000000, potofgoldLevelText, potofgoldCostText);
    }

    private BigDouble GetLevelUpCost(BigDouble targetLevel)
    {
        // Define initial base cost and growth factor using BigDouble
        BigDouble baseCost = 10;    // Initial cost for level 2
        BigDouble growthFactor = 1.5;  // Increase cost by 50% per level

        // Dynamic cost calculation: baseCost * (growthFactor ^ (targetLevel - 1))
        return BigDouble.Floor(baseCost * BigDouble.Pow(growthFactor, targetLevel - 1));
    }
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
    public void OnTwoDollarClicked(int incrementAmount)
    {
        // Increment the click count
        clickCount += incrementAmount;
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
    public void OnDiamondDollarClicked(int incrementAmount)
    {
        // Increment the click count
        clickCount += incrementAmount;
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
        if (particletoggleon && clicker.enabled)
        {
            diamond1.Play();
            diamond2.Play();
        }


    }
    public void OnFrutigerClicked(int incrementAmount)
    {
        clickCount += incrementAmount;

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
        if (particletoggleon && clicker.enabled)
        {
            PlayParticleWithChance(fish1, 0.5f);
            PlayParticleWithChance(fish2, 0.5f);
            PlayParticleWithChance(fish3, 0.5f);
            PlayParticleWithChance(frutigerphone, 0.8f);
        }

    }
    public void OnIceDollarClicked(int incrementAmount)
    {
        clickCount += incrementAmount;

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
        if (particletoggleon && clicker.enabled)
        {
            PlayParticleWithChance(ice1, 0.2f);
            PlayParticleWithChance(ice2, 0.2f);
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
    private bool diamondbriefcaseclicked = false;
    private bool pumpkinbriefcaseclicked = false;

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
        else if (lootboxtype == 3)
        {
            diamondbriefcaseclicked = true;
        }
        else if (lootboxtype == 4)
        {
            pumpkinbriefcaseclicked = true;
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
        if (diamondbriefcaseclicked)
        {
            activity.State = "Opened a Diamond Briefcase";
            activity.Details = "Recieved a " + mostrecentitem;
            activity.Timestamps.Start = startTime;

            activity.Assets.LargeImage = "diamondbriefcase";
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
        else if (goldenbriefcaseclicked)
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
        else if (pumpkinbriefcaseclicked)
        {
            activity.State = "Opened a Golden Briefcase";

            activity.Details = "Recieved a " + mostrecentitem;
            activity.Timestamps.Start = startTime;

            activity.Assets.LargeImage = "pumpkinbriefcase";
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
