using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System;



public class SteamInventoryManager : MonoBehaviour
{
    public GIFPlayer player;
    public Text timer;
    public Text cooldowntimer;
    public Text successtext;
    public Text cooldowntimer2;
    public Text successtext2;
    public GameObject cooldowntext;
    public GameObject cooldowntext2;

    public Text totaldrops;
    private int totalDrops = 0;
    private float cooldownTime = 60f;
    private bool exchanging = false;
    private bool isRunning = false;
    public ParticleSystem money2;
    public Transform StackedItemContent;
    public Transform ItemContent;
    public GameObject InventoryItem;
    public GameObject SmallerInventoryItem;
    public GameObject EvenSmallerInventoryItem;
    public GameObject VerySmallerInventoryItem;

    public GameObject InventoryScreen;
    public ItemDatabase itemDatabase;
    private Coroutine toggleCoroutine = null;
    public Image background;
    public Text clicktoequip;
    public SpriteRenderer clicker;
    public ParticleSystem leaves1;
    public ParticleSystem leaves2;
    public ParticleSystem leaves3;
    public ParticleSystem leaves4;

    public ParticleSystem moneytype;
    public ParticleSystem sparkle;
    public ParticleSystem confetti;
    public ParticleSystem silversparkle;
    public Material[] materials;
    private int currentMaterialIndex = 0;
    public List<Item> Items = new List<Item>();
    private Vector3 openPosition;
    private Vector3 closedPosition;
    private Vector3 restingPosition;
    private bool seen = false;
    public MoveScreens moveScreens;
    private bool isOpen = false;
    private bool m_bInitialized;
    private const string LastRecordedTimeKey = "LastRecordedTime";
    public void StopParticles()
    {
        sparkle.Stop();
        confetti.Stop();
    }
    private void Start()
    {
        // Call CheckForItemDrop after 5 seconds and then repeat every 3 minutes (180 seconds)
        InvokeRepeating("TriggerItemDrop", 60.2f, 180.2f);
        CheckAndStartCountdown();
        IncrementDrops();
        StartCoroutine(SaveTimeEvery10Seconds());
        openPosition = new Vector3(InventoryScreen.transform.localPosition.x, 1980, InventoryScreen.transform.localPosition.z);
        closedPosition = new Vector3(InventoryScreen.transform.localPosition.x, 1580f, InventoryScreen.transform.localPosition.z);
        restingPosition = new Vector3(InventoryScreen.transform.localPosition.x, -5000f, InventoryScreen.transform.localPosition.z);
        if (PlayerPrefs.HasKey("SteamAPIKey"))
        {
            webAPIKeyInputField.text = PlayerPrefs.GetString("SteamAPIKey");
        }
    }
    private void CheckAndStartCountdown()
    {
        // Get the current time
        DateTime currentTime = DateTime.Now;

        // Check if we have a previously saved time
        if (PlayerPrefs.HasKey(LastRecordedTimeKey))
        {
            // Retrieve the saved time
            string savedTimeStr = PlayerPrefs.GetString(LastRecordedTimeKey);
            DateTime savedTime;

            if (DateTime.TryParse(savedTimeStr, out savedTime))
            {
                // Calculate the next hour based on the saved time
                DateTime nextHour = new DateTime(savedTime.Year, savedTime.Month, savedTime.Day, savedTime.Hour, 0, 0).AddHours(1);
                double secondsUntilNextHour = (nextHour - currentTime).TotalSeconds;

                // If more than an hour has passed since the saved time, start a 60-second countdown
                if (secondsUntilNextHour <= 0)
                {
                    StartCoroutine(StartCountdown(60f));
                }
                else
                {
                    // Otherwise, start the countdown with the remaining time until the next hour
                    StartCoroutine(StartCountdown((float)secondsUntilNextHour));
                }
            }
        }
        else
        {

            StartCoroutine(StartCountdown(60f));
        }
    }

    // Coroutine that runs every 10 seconds to save the current time to PlayerPrefs
    private IEnumerator SaveTimeEvery10Seconds()
    {
        while (true)
        {
            // Save the current time to PlayerPrefs
            DateTime currentTime = DateTime.Now;
            PlayerPrefs.SetString(LastRecordedTimeKey, currentTime.ToString());
            PlayerPrefs.Save();  // Save to disk
            Debug.Log("Saved time: " + currentTime);

            // Wait for 10 seconds
            yield return new WaitForSeconds(10f);
        }
    }

    private void ChangeSprite(int index)
    {
        // Load the new sprite from the Resources folder
        Sprite newSprite = Resources.Load<Sprite>($"sprite_{index}");

        // Check if the sprite was successfully loaded
        if (newSprite != null)
        {
            // Assign the new sprite to the target Image component
            clicker.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"The sprite 'sprite_{index}' could not be found in the Resources folder.");
        }
    }
    public void ChangeMaterial(int index)
    {
        if (index >= 0 && index < materials.Length)
        {
            var renderer = moneytype.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = materials[index];
                currentMaterialIndex = index;
            }
            else
            {
                Debug.LogWarning("No ParticleSystemRenderer found on the particle system.");
            }
        }
        else
        {
            Debug.LogWarning("Material index out of range.");
        }
    }

    IEnumerator StartCountdown(float timeRemaining)
    {
        isRunning = true;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }

        // Ensure timer shows 00:00 at the end
        timer.text = "Dropping";
        isRunning = false;
        TriggerItemDrop();

    }
    public void ToggleInventory()
    {
        if (toggleCoroutine != null)
        {
            return;
        }

        Vector3 targetPosition;
        Vector3 startingPosition;

        if (!isOpen) // If currently closed, open it
        {
            if (clickcounter.onfire)
            {
                return;
            }
            IncrementDrops();
            targetPosition = openPosition; // Open position first
            startingPosition = closedPosition;
            clickcounter.clickenabled = false;
        }
        else // If currently open, close it
        {
            targetPosition = closedPosition; // Close position next
            startingPosition = openPosition;
            if (previousitem != 116 && previousitem != 126 && previousitem != 136 && previousitem != 1003)
            {
                clickcounter.clickenabled = true;
            }

        }

        toggleCoroutine = StartCoroutine(MoveInventoryScreen(targetPosition, startingPosition));
    }

    private IEnumerator MoveInventoryScreen(Vector3 targetPosition, Vector3 startingPosition)
    {
        float duration = 1.0f;
        float elapsedTime = 0f;
        float startAlpha = background.color.a;
        float targetAlpha = targetPosition == openPosition ? 1f : 0f; // Determine target alpha based on target position

        // Enable the background image if we are opening the inventory
        if (targetPosition == openPosition) // Opening the inventory
        {
            background.enabled = true;
            if (!seen)
            {
                clicktoequip.text = "Click to Equip";
                StartCoroutine(CloseText());
                seen = true;
            }
        }

        // Animate the movement and color change
        while (elapsedTime < duration)
        {
            InventoryScreen.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);

            // Lerp alpha value
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            // Set the background color using the currentbackgroundcolor RGB and lerped alpha
            Color backgroundColor = currentbackgroundcolor; // Assuming currentbackgroundcolor is a Color variable
            backgroundColor.a = newAlpha;
            background.color = backgroundColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finalize the position and alpha
        InventoryScreen.transform.localPosition = targetPosition;
        Color finalColor = currentbackgroundcolor;
        finalColor.a = targetAlpha;
        background.color = finalColor;

        // If closing, move to resting position
        if (targetPosition == closedPosition) // Closing the inventory
        {
            background.enabled = false;

            // Move to resting position after closing
            yield return new WaitForSeconds(0.1f); // Optional delay
            InventoryScreen.transform.localPosition = restingPosition;
        }

        // Update the isOpen flag
        isOpen = targetPosition == openPosition; // Update the state based on the target position

        toggleCoroutine = null;
    }



    IEnumerator CloseText()
    {
        yield return new WaitForSeconds(3f);
        clicktoequip.text = "";

        yield return null;
    }
    private bool successfulexchange = false;
    public void SuccessText()
    {
        successtext.text = "Success";
        successtext2.text = "Success";
        money2.Play();
        successfulexchange = true;
        StartCoroutine(ClearSuccess());
    }
    IEnumerator ClearSuccess()
    {
        yield return new WaitForSeconds(1f);
        successfulexchange = false;
        yield return null;
    }
    public void NotEnoughText()
    {
        if(successfulexchange)
        {
            return;
        }
        successtext.text = "Failed, not enough items";
        successtext2.text = "Failed, not enough items";

    }
    public void IncrementDrops()
    {
        StartCoroutine(LogItems());
    }

    private void UpdateTotalDropsText()
    {
        if (totalDrops == 1)
        {
            totaldrops.text = totalDrops.ToString() + " Item";
        }
        else
        {
            totaldrops.text = totalDrops.ToString() + " Items";
        }
    }

    public void GetAchievement()
    {
        Steamworks.SteamUserStats.GetAchievement("CLICK_THE_DOLLAR", out bool achievementCompleted);
        if (!achievementCompleted)
        {
            SteamUserStats.SetAchievement("CLICK_THE_DOLLAR");
            SteamUserStats.StoreStats();
        }
        normalbutton.SetActive(false);
    }
    public void GetAchievement2()
    {
        Steamworks.SteamUserStats.GetAchievement("CLICK_GOLDEN_DOLLAR", out bool achievementCompleted);
        if (!achievementCompleted)
        {
            SteamUserStats.SetAchievement("CLICK_GOLDEN_DOLLAR");
            SteamUserStats.StoreStats();
        }
    }
    public void ClearInventory()
    {
        SteamInventoryResult_t resultHandle;
        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            uint arraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize))
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    // Loop through each item and consume it
                    foreach (var item in itemDetails)
                    {
                        SteamInventoryResult_t consumeResultHandle;
                        bool success = SteamInventory.ConsumeItem(out consumeResultHandle, item.m_itemId, item.m_unQuantity);

                        if (success)
                        {
                            Debug.Log($"Item {item.m_itemId} consumed successfully.");
                        }
                        else
                        {
                            Debug.LogError($"Failed to consume item {item.m_itemId}.");
                        }

                        SteamInventory.DestroyResult(consumeResultHandle);
                    }
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array. Result code: " + SteamInventory.GetResultStatus(resultHandle));
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items. Result code: " + SteamInventory.GetResultStatus(resultHandle));
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory. Result code: " + SteamInventory.GetResultStatus(resultHandle));
        }
    }
    public int ID;
    public void GenerateItem()
    {
        SteamInventoryResult_t inventoryResult;
        SteamItemDef_t[] itemDefs = new SteamItemDef_t[ID];
        for (int i = 0; i < itemDefs.Length; i++)
        {
            // Initialize each element as needed
            itemDefs[i] = new SteamItemDef_t(ID);
        }
        //bool success = SteamInventory.GenerateItems(out inventoryResult, itemDefs, null, 1);
        SteamInventory.GenerateItems(out inventoryResult, itemDefs, null, 1);
        SteamInventory.DestroyResult(inventoryResult);
    }
    public void GrantPromoItem()
    {
        // Define the item ID for the promo item
        SteamItemDef_t itemDef = (SteamItemDef_t)115; // Cast the int to SteamItemDef_t
        SteamInventoryResult_t resultHandle;

        bool success = SteamInventory.AddPromoItem(out resultHandle, itemDef);
        SteamInventory.DestroyResult(resultHandle);

    }
    private float lastCheckTime = -300f; // Initialize with a time 5 minutes ago
    public void TriggerItemDrop()
    {
        Debug.Log("Item Dropping" + System.DateTime.Now);
        System.DateTime currentTime = System.DateTime.Now;

        SteamInventoryResult_t inventoryResult;
        if (currentTime.Hour >= 18 || currentTime.Hour < 6)
        {
            // After 6 PM and before 6 AM
            Debug.Log("Past 6");
            SteamInventory.TriggerItemDrop(out inventoryResult, new SteamItemDef_t(11));
        }
        else
        {
            SteamInventory.TriggerItemDrop(out inventoryResult, new SteamItemDef_t(10));
            Debug.Log("Before 6");

        }

        if (!isRunning)
        {
            StartCoroutine(StartCountdown(3600f));
        }

        SteamInventory.DestroyResult(inventoryResult);

        // Check if 5 minutes have passed since the last check
        if (Time.time - lastCheckTime >= 300f)
        {
            StartCoroutine(WaitToCheckItems());
        }
        else
        {
            Debug.Log("Less than 5 minutes since last check, skipping WaitToCheckItems.");
        }
    }

    private IEnumerator WaitToCheckItems()
    {
        yield return new WaitForSeconds(20f);
        IncrementDrops();
        lastCheckTime = Time.time; // Update the timestamp
        yield return null;
    }
    IEnumerator LogItems()
    {
        LogAllItemsInInventory();
        yield return new WaitForSeconds(0.6f);
        LogAllItemsInInventory();
        UpdateTotalDropsText();
        yield return null;
    }
    public Dictionary<int, int> storeditems;
    public Text briefcasecounttext;
    public Text goldenbriefcasecounttext;
    public GameObject StackWarning;
    public GameObject StackWarning2;
    public void LogAllItemsInInventory()
    {
        itemsToSplit.Clear();
        itemsToSplitIDs.Clear();
        quantitiesToSplit.Clear();
        SteamInventoryResult_t resultHandle;
        uint arraySize = 0;
        int totalItems = 0;

        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize) && arraySize > 0)
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    Dictionary<int, int> itemCounts = new Dictionary<int, int>(); // Dictionary to count items
                    foreach (var item in itemDetails)
                    {
                        try
                        {
                            int itemId = (int)item.m_iDefinition;

                            if (itemCounts.ContainsKey(itemId))
                            {
                                itemCounts[itemId] += (int)item.m_unQuantity;
                            }
                            else
                            {
                                itemCounts[itemId] = (int)item.m_unQuantity;
                            }

                            totalItems += (int)item.m_unQuantity;

                            if (item.m_unQuantity > 1)
                            {
                                StackWarning.SetActive(true);
                                itemsToSplitIDs.Add(itemId);
                                itemsToSplit.Add(item.m_itemId.m_SteamItemInstanceID);
                                quantitiesToSplit.Add(item.m_unQuantity);
                                Debug.Log($"Stored item with ID: {item.m_itemId.m_SteamItemInstanceID} for splitting (Quantity: {item.m_unQuantity})");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing item: {ex.Message}");
                        }
                    }

                    Items.Clear(); // Clear the current list of items

                    foreach (var kvp in itemCounts)
                    {
                        int itemId = kvp.Key;
                        Item newItem = itemDatabase.GetItemByID(itemId);
                        if (newItem != null)
                        {
                            Items.Add(newItem);
                        }
                        else
                        {
                            Debug.LogWarning($"Item with definition {itemId} not found in database.");
                        }
                    }

                    totalDrops = totalItems; // Set totalDrops to the total number of items
                    storeditems = itemCounts;
                    Debug.Log($"Total items: {totalItems}");
                    UpdateInventoryUI(itemCounts);
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array. Result code: " + SteamInventory.GetResultStatus(resultHandle));
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items or array size is zero. Result code: " + SteamInventory.GetResultStatus(resultHandle));
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory. Result code: " + SteamInventory.GetResultStatus(resultHandle));
        }
    }

    public InputField webAPIKeyInputField; // Input field for the Steam Web API key
    public Text logText; // UI Text to display logs
    public int sleepDuration = 1000; // Time to sleep between API calls (in milliseconds)
    private List<ulong> itemsToSplit = new List<ulong>(); // List of item IDs to split
    private List<int> itemsToSplitIDs = new List<int>(); // List of item IDs to split
    private List<int> quantitiesToSplit = new List<int>(); // List of item IDs to split
    private string webAPIKey;
    private bool unstacking = false;
    // Called after the API key is input by the player
    private Coroutine splitCoroutine; // Variable to hold the reference to the coroutine
    private Coroutine unstackCoroutine; // Variable to hold the reference to the coroutine


    public void StartSplittingItems(int itemIndex) // Accept an index as a parameter
    {
        StopSplittingItems();
        webAPIKey = webAPIKeyInputField.text;

        if (string.IsNullOrEmpty(webAPIKey))
        {
            Debug.LogError("API key is required to proceed.");
            return;
        }

        // Optionally save the API key
        PlayerPrefs.SetString("SteamAPIKey", webAPIKey);
        PlayerPrefs.Save();
        if (unstackCoroutine != null)
        {
            StopCoroutine(unstackCoroutine); // Stop the running coroutine
            unstackCoroutine = null; // Clear the reference
        }
        // Start the coroutine for the specific item index
        splitCoroutine = StartCoroutine(SplitItems(itemIndex));
    }
    public void StopSplittingItems()
    {
        if (splitCoroutine != null)
        {
            StopCoroutine(splitCoroutine); // Stop the running coroutine
            splitCoroutine = null; // Clear the reference
            Debug.Log("Item splitting stopped.");
            logText.text = "";
            unstackCoroutine = StartCoroutine(UnstackingBool());
        }
    }
    public IEnumerator SplitItems(int itemIndex)
    {
        // Check if the index is valid
        if (itemIndex < 0 || itemIndex >= itemsToSplit.Count)
        {
            Debug.LogError("Invalid item index." + itemIndex);
            yield break; // Exit if the index is out of bounds
        }

        unstacking = true;

        // Get the item ID and quantity from the specified index
        ulong itemId = itemsToSplit[itemIndex];
        int quantity = quantitiesToSplit[itemIndex];

        Debug.Log($"Starting to split Item ID: {itemId}, Quantity: {quantity}");

        // Only split if quantity is greater than 1
        if (quantity > 1)
        {
            for (int i = 1; i < quantity; i++) // Split quantity - 1 times
            {
                string url = "https://api.steampowered.com/IInventoryService/SplitItemStack/v1/";

                // Prepare the form data for the POST request
                WWWForm form = new WWWForm();
                form.AddField("key", webAPIKey);
                form.AddField("appid", "3069470");
                form.AddField("itemid", itemId.ToString());
                form.AddField("quantity", "1");

                using (UnityWebRequest request = UnityWebRequest.Post(url, form))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError($"Error splitting item {itemId}: {request.error}");
                        break; // Exit the loop if an error occurs for this item
                    }
                    else
                    {
                        Debug.Log($"Successfully split item {itemId}, iteration {i}/{quantity}");
                        logText.text = $"{i}/{quantity}"; // Update log text to reflect the current split
                        unstacking = true;

                    }
                }

                // Sleep for the specified duration
                yield return new WaitForSeconds(sleepDuration / 1000f);
            }
        }
        else
        {
            Debug.Log($"Item ID: {itemId} has no need to be split (Quantity: {quantity})");
        }

        unstackCoroutine = StartCoroutine(UnstackingBool());
        logText.text = $"Item splitting completed.";
        Debug.Log("Item splitting completed.");
    }
    private IEnumerator UnstackingBool()
    {
        yield return new WaitForSeconds(2f);
        unstacking = false;
        yield return null;
    }


    private void UpdateInventoryUI(Dictionary<int, int> itemCounts)
    {
        // Determine which item prefab to use based on the number of items
        GameObject itemPrefab;
        if (Items.Count > 18)
        {
            itemPrefab = VerySmallerInventoryItem;
        }
        else if (Items.Count > 15)
        {
            itemPrefab = EvenSmallerInventoryItem;
        }
        else if (Items.Count > 8)
        {
            itemPrefab = SmallerInventoryItem;
        }
        else
        {
            itemPrefab = InventoryItem;
        }

        // Set the cell size based on the item type
        GridLayoutGroup gridLayoutGroup = ItemContent.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            if (Items.Count > 18)
            {
                gridLayoutGroup.cellSize = new Vector2(99, 99);
                gridLayoutGroup.padding.left = 2;
                gridLayoutGroup.padding.top = 0;
            }
            else if (Items.Count > 15)
            {
                gridLayoutGroup.cellSize = new Vector2(132, 132);
                gridLayoutGroup.padding.left = 2;
                gridLayoutGroup.padding.top = 0;
            }
            else if (Items.Count > 8)
            {
                gridLayoutGroup.cellSize = new Vector2(160, 160);
                gridLayoutGroup.padding.left = 0;
                gridLayoutGroup.padding.top = -15;
            }
            else
            {
                gridLayoutGroup.cellSize = new Vector2(200, 200);
                gridLayoutGroup.padding.left = 0;
                gridLayoutGroup.padding.top = 0;
            }
        }

        // Clear existing items
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in StackedItemContent)
        {
            Destroy(item.gameObject);
        }

        // Define color values
        Color highlightColor = Color.white;
        Color defaultColor = new Color32(50, 50, 50, 255); // #323232

        // Populate the inventory UI with the current items
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(itemPrefab, ItemContent);
            Button button = obj.GetComponent<Button>();
            int itemId = item.ID;
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            button.onClick.AddListener(() => ToggleInventory());
            button.onClick.AddListener(() => OnInventoryItemClick(itemId));

            // Check if the item ID is over 999
            string displayName = item.Name;
            if (itemId > 499 || itemId == 123)
            {
                itemName.text = displayName; // Only the name, no quantity
            }
            else
            {
                int quantity = itemCounts[itemId];

                // List of item IDs to ignore for the pluralization rule
                List<int> ignorePluralization = new List<int> { 102, 103, 104, 105, 106 };

                // Check if the item ID should be ignored for the pluralization rule
                if (!ignorePluralization.Contains(itemId) && quantity > 1)
                {
                    displayName += "s"; // Append 's' for plural
                }

                itemName.text = quantity.ToString() + " " + displayName; // Include quantity
            }

            itemIcon.sprite = item.Icon;

            // Set text color based on previously equipped item ID
            if (previousitem == 109 || previousitem == 114 || previousitem == 124)
            {
                itemName.color = highlightColor;
            }
            else
            {
                itemName.color = defaultColor;
            }
        }
        if (itemsToSplitIDs.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            emptyinventory.SetActive(true); // Enable the emptyinventory GameObject
            return; // Exit the function early
        }
        if (itemsToSplitIDs.Count != quantitiesToSplit.Count)
        {
            Debug.LogError("Mismatch between itemsToSplitIDs and quantitiesToSplit.");
            return; // Prevent further execution
        }


        // If the list is not empty, ensure the emptyinventory is disabled
        emptyinventory.SetActive(false);
        for (int i = 0; i < itemsToSplitIDs.Count; i++)
        {
            int itemIdInt = itemsToSplitIDs[i];
            GameObject obj = Instantiate(itemPrefab, StackedItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            Button button = obj.GetComponent<Button>();

            // Capture the current index value
            int index = i; // Create a new variable to hold the current index
            button.onClick.AddListener(() => StartSplittingItems(index)); // Use the captured index

            Item item = itemDatabase.GetItemByID(itemIdInt);
            if (item != null)
            {
                int quantity = quantitiesToSplit[i];
                itemName.text = $"{quantity} {item.Name}";
                itemIcon.sprite = item.Icon;
            }
            else
            {
                Debug.LogWarning($"Item with ID {itemIdInt} not found in database.");
            }
        }


    }
    public UpgradeMenu upgradeMenu;
    public GameObject emptyinventory;

    void Update()
    {
        if (isMoving)
        {
            // Update angle for circular motion
            angle += speed * Time.deltaTime;

            // Calculate new position
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            trailObject.transform.position = new Vector3(x, y, 0);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            upgradeMenu.ToggleUpgradeMenu();
        }
    }
    private Color currentbackgroundcolor = new Color(0.6078f, 0.8667f, 1.0f, 0.0f);
    public Image topbar;
    IEnumerator TopBarColorChange(Color color)
    {
        float elapsedTime = 0f;
        Color initialColor = topbar.color; // Store the initial color

        while (elapsedTime < duration)
        {
            // Interpolate between the initial color and the target color
            topbar.color = Color.Lerp(initialColor, color, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        yield return null;
    }
    IEnumerator ColorChange (Color color)
    {
        float elapsedTime = 0f;
        Color initialColor = maincamera.backgroundColor; // Store the initial color

        while (elapsedTime < duration)
        {
            // Interpolate between the initial color and the target color
            maincamera.backgroundColor = Color.Lerp(initialColor, color, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        currentbackgroundcolor = color;
        yield return null;
    }
    public Text deletetext;
    IEnumerator ColorChangeText(Color color)
    {
        if (counter == null) yield break; // Exit if counter is not assigned

        Color initialColor = counter.color; // Store the initial color
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate between the initial color and the target color
            counter.color = Color.Lerp(initialColor, color, elapsedTime / duration);
            deletetext.color = Color.Lerp(initialColor, color, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final color is set
        counter.color = color;
        deletetext.color = color;
    }
    public SpriteRenderer windowswallpaper;
    private IEnumerator Fade(SpriteRenderer spriteRenderer, float targetAlpha)
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            color.a = alpha;
            spriteRenderer.color = color;
            yield return null;
        }

        // Ensure the final alpha value is set
        color.a = targetAlpha;
        spriteRenderer.color = color;
    }
    public void WindowsVistaStartup()
    {
        StartCoroutine(ColorChange(new Color32(26, 132, 155, 255)));
        windowswallpaper.enabled = true;
        StartCoroutine(Fade(windowswallpaper, 1f));
    }
    public void WindowsVistaShutdown()
    {
        StartCoroutine(Fade(windowswallpaper, 0f));

    }
    public void StartMovement()
    {
        StartCoroutine(ColorChange(Color.black));
        StartCoroutine(ColorChangeText(Color.white));

        isMoving = true;
        trailRenderer.enabled = true; // Enable the trail when starting movement
    }

    public void StopMovement()
    {
        StartCoroutine(ColorChange(normalColor));
        StartCoroutine(ColorChangeText(normalTextColor));
        isMoving = false;
        trailRenderer.enabled = false; // Disable the trail when stopping movement
    }
    public float duration = 2.0f;
    public Color normalTextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f);
    public Color normalColor = new Color(155f / 255f, 221f / 255f, 255f / 255f); // Background color
    public SpriteRenderer dollar;
    public GameObject normalbutton;
    public GameObject briefcase;
    public GameObject goldenbriefcase;
    public GameObject diamondbriefcase;

    public GameObject thevoicebriefcase;
    public GameObject twodollar;
    public GameObject notadollar;
    public Text counter;
    public GameObject trailObject;
    public TrailRenderer trailRenderer;
    public float radius = 3.0f; // Radius of the circular motion
    public float speed = 5.0f; // Speed of the circular motion
    public Camera maincamera;
    public GameObject lighter;
    public GameObject goldenlighter;

    private bool isMoving = false; // Flag to control movement
    private float angle = 0f; // Current angle in radians
    private Color defaultbackgroundcolor = new Color(0.6078f, 0.8667f, 1.0f, 0.0f);
    public ClickCounter clickcounter;
    public void ConsumeItem()
    {
        SteamInventoryResult_t resultHandle;
        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            uint arraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize))
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    // Find the item with definition 100
                    SteamItemDetails_t itemToConsume = default;
                    bool itemFound = false;
                    for (int i = 0; i < itemDetails.Length; i++)
                    {
                        if ((int)itemDetails[i].m_iDefinition == previousitem) // Compare item definition
                        {
                            itemToConsume = itemDetails[i];
                            itemFound = true;
                            break; // Exit loop once the item is found
                        }
                    }

                    if (itemFound)
                    {
                        // Consume only one item from the stack
                        SteamInventoryResult_t consumeResultHandle;
                        bool success = SteamInventory.ConsumeItem(out consumeResultHandle, itemToConsume.m_itemId, 1);

                        if (success)
                        {
                            Debug.Log($"Item {itemToConsume.m_itemId} consumed successfully.");
                        }
                        else
                        {
                            Debug.LogError($"Failed to consume item {itemToConsume.m_itemId}.");
                        }

                        SteamInventory.DestroyResult(consumeResultHandle);
                    }
                    else
                    {
                        Debug.LogWarning("Item with definition 100 not found in inventory.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array. Result code: " + SteamInventory.GetResultStatus(resultHandle));
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items. Result code: " + SteamInventory.GetResultStatus(resultHandle));
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory. Result code: " + SteamInventory.GetResultStatus(resultHandle));
        }
    }


    public GameObject deletetoggle;
    private Coroutine toggleCoroutine2;
    public AudioManager audioManager;
    public BoxCollider2D dollarhitbox;
    public AutoclickerManager autoclickerManager;
    private void OnInventoryItemClick(int itemId)
    {
        if (itemId < 500 || itemId > 600)
        {
            // Perform the actions if itemId is below 500 or above 600
            clickcounter.dollarburned = false;

            dollar.enabled = false;
            dollarhitbox.enabled = false;
            briefcase.SetActive(false);
            goldenbriefcase.SetActive(false);
            diamondbriefcase.SetActive(false);

            thevoicebriefcase.SetActive(false);
            twodollar.SetActive(false);
            notadollar.SetActive(false);
            StopMovement();
            sparkle.Stop();
            confetti.Stop();
            silversparkle.Stop();
            StopMovingClouds();
            moveScreens.SetDollarType(itemId);
            discordController.SetDollarType(itemId);
            player.StopGIF();
            WindowsVistaShutdown();
            neonobjects.SetActive(false);
            neonobjects2.SetActive(false);
            StopFading();
            leaves1.Stop();
            leaves2.Stop();
            leaves3.Stop();
            leaves4.Stop();
            StartCoroutine(TopBarColorChange(new Color(113f / 255f, 113f / 255f, 255f / 255f, 183f / 255f)));



        }
        clickcounter.SetDollarType(itemId);

        switch (itemId)
        {
            case 100:
                // Handle item ID 100 logic
                dollar.enabled = true;
                dollarhitbox.enabled = true;
                ChangeMaterial(0);
                ChangeSprite(100);
                notadollar.SetActive(true);

                StartCoroutine(ColorChange(defaultbackgroundcolor));

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 101:
                // Handle item ID 101 logic
                dollarhitbox.enabled = true;

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                dollar.enabled = true;
                ChangeMaterial(1);
                ChangeSprite(101);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 102:
                // Handle item ID 102 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(5);
                ChangeSprite(102);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 103:
                // Handle item ID 103 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(6);
                ChangeSprite(103);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 104:
                // Handle item ID 104 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(6);
                ChangeSprite(104);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 105:
                // Handle item ID 105 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(7);
                ChangeSprite(105);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 106:
                // Handle item ID 106 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(8);
                ChangeSprite(106);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 107:
                // Handle item ID 107 logic
                dollarhitbox.enabled = true;

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                    confetti.Play();
                }
                dollar.enabled = true;
                ChangeMaterial(4);
                ChangeSprite(100);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 108:
                // Handle item ID 108 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeSprite(108);
                notadollar.SetActive(true);

                WindowsVistaStartup();
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 109:
                // Handle item ID 109 logic
                dollarhitbox.enabled = true;

                dollar.enabled = true;
                ChangeMaterial(3);
                ChangeSprite(109);
                notadollar.SetActive(true);

                StartMovement();
                StartCoroutine(ChangeAllImagesColors(Color.grey, 1f));
                break;
            case 110:
                // Handle item ID 110 logic
                dollarhitbox.enabled = true;

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                dollar.enabled = true;
                ChangeMaterial(2);
                ChangeSprite(110);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 111:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(0);
                ChangeSprite(111);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 112:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(0);
                ChangeSprite(112);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 113:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(9);
                ChangeSprite(113);
                notadollar.SetActive(true);

                StartMovingClouds();

                StartCoroutine(ColorChange(new Color(24f / 255f, 128f / 255f, 222f / 255f)));
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 114:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(10);
                ChangeSprite(114);
                notadollar.SetActive(true);

                StartMovingClouds2();
                PlaceMoonAndStars();
                StartCoroutine(ColorChange(new Color(12f / 255f, 23f / 255f, 33f / 255f)));
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 115:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(18);
                ChangeSprite(115);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 116:
                // Handle item ID 116 logic
                dollarhitbox.enabled = false;
                briefcase.SetActive(true);
                counter.text = "";
                clicktoequip.text = "Click multiple times to open";
                StartCoroutine(CloseText());
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 117:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(11);
                ChangeSprite(117);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 118:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(12);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(118);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 119:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(13);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(119);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 120:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(14);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(120);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 121:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(15);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(121);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 122:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(25);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(122);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 123:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(26);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(123);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 124:
                dollar.enabled = true;
                dollarhitbox.enabled = true;
                StartFading();
                StartCoroutine(ColorChangeText(Color.white));
                neonobjects.SetActive(true);
                neonobjects2.SetActive(true);
                notadollar.SetActive(true);

                ChangeSprite(124);
                ChangeMaterial(27);

                StartCoroutine(ChangeAllImagesColors(Color.grey, 1f));
                break;
            case 125:
                dollar.enabled = true;
                dollarhitbox.enabled = true;
                ChangeMaterial(28);
                ChangeSprite(125);
                notadollar.SetActive(true);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 126:
                // Handle item ID 116 logic
                dollarhitbox.enabled = false;


                goldenbriefcase.SetActive(true);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                counter.text = "";
                clicktoequip.text = "Click multiple times to open";
                StartCoroutine(CloseText());
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 127:
                dollar.enabled = true;
                dollarhitbox.enabled = true;
                ChangeMaterial(28);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(127);
                notadollar.SetActive(true);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 128:
                dollar.enabled = true;
                dollarhitbox.enabled = true;
                ChangeMaterial(30);
                ChangeSprite(128);
                notadollar.SetActive(true);
                StartCoroutine(ColorChange(new Color(121f / 255f, 24f / 255f, 1f / 255f)));
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                StartCoroutine(TopBarColorChange(new Color(166f / 255f, 76f / 255f, 1f / 255f, 183f / 255f)));
                leaves1.Play();
                leaves2.Play();
                leaves3.Play();
                leaves4.Play();
                break;
            case 136:
                // Handle item ID 116 logic
                dollarhitbox.enabled = false;


                diamondbriefcase.SetActive(true);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                counter.text = "";
                clicktoequip.text = "Click multiple times to open";
                StartCoroutine(CloseText());
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 500:
                musicicon.SetActive(true);
                audioManager.ChangeSong(0);
                counter.text = "";
                break;
            case 501:
                lighter.SetActive(!lighter.activeSelf);
                lighter.transform.position = new Vector2(6.42f, -1.73f);
                deletetoggle.SetActive(!deletetoggle.activeSelf);
                counter.text = "";
                break;
            case 503:
                break;
            case 504:
                goldenlighter.SetActive(!goldenlighter.activeSelf);
                goldenlighter.transform.position = new Vector2(6.42f, -1.73f);
                deletetoggle.SetActive(!deletetoggle.activeSelf);
                counter.text = "";
                break;
            case 507:
                musicicon.SetActive(true);
                audioManager.ChangeSong(1);
                counter.text = "";
                break;
            case 508:
                autoclickerobject.SetActive(!autoclickerobject.activeSelf);
                autoclickerbutton.SetActive(!autoclickerbutton.activeSelf);
                autoclickerManager.StopAutoClicker();

                // Find the child object named "autoclicker"
                Transform autoclickerTransform = autoclickerobject.transform.Find("autoclicker");

                if (autoclickerTransform != null)
                {
                    // Get the Image component attached to the child object
                    Image autoclickerImage = autoclickerTransform.GetComponent<Image>();
                    Image autoclickerButton = autoclickerbutton.GetComponent<Image>();

                    if (autoclickerImage != null)
                    {
                        // Load the sprite from the Resources folder
                        Sprite newSprite = Resources.Load<Sprite>("click3");
                        autoclickerButton.sprite = Resources.Load<Sprite>("goldcrop");

                        if (newSprite != null)
                        {
                            // Change the image to the new sprite
                            autoclickerImage.sprite = newSprite;
                        }
                        else
                        {
                            Debug.LogError("Sprite 'click2' not found in Resources folder.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Image component not found on 'autoclicker' child object.");
                    }
                }
                else
                {
                    Debug.LogError("'autoclicker' child object not found.");
                }
                break;
            case 509:
                autoclickerobject.SetActive(!autoclickerobject.activeSelf);
                autoclickerbutton.SetActive(!autoclickerbutton.activeSelf);
                autoclickerManager.StopAutoClicker();
                // Find the child object named "autoclicker"
                Transform autoclickerTransform2 = autoclickerobject.transform.Find("autoclicker");
                if (autoclickerTransform2 != null)
                {
                    // Get the Image component attached to the child object
                    Image autoclickerImage = autoclickerTransform2.GetComponent<Image>();
                    Image autoclickerButton = autoclickerbutton.GetComponent<Image>();

                    if (autoclickerImage != null)
                    {
                        // Load the sprite from the Resources folder
                        Sprite newSprite = Resources.Load<Sprite>("click2");
                        autoclickerButton.sprite = Resources.Load<Sprite>("silvercrop");
                        if (newSprite != null)
                        {
                            // Change the image to the new sprite
                            autoclickerImage.sprite = newSprite;
                        }
                        else
                        {
                            Debug.LogError("Sprite 'click2' not found in Resources folder.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Image component not found on 'autoclicker' child object.");
                    }
                }
                else
                {
                    Debug.LogError("'autoclicker' child object not found.");
                }
                break;
            case 510:
                upgradeMenu.ToggleDuckPhysicsItem();
                break;
            case 511:
                upgradeMenu.ToggleDiamondDuckPhysicsItem();
                break;
            case 512:
                upgradeMenu.TogglePumpkinPhysicsItem();
                break;
            case 1000:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(16);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1000);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1001:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(17);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1001);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1002:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(19);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1002);
                notadollar.SetActive(true);

                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1003:
                thevoicebriefcase.SetActive(true);
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                counter.text = "";
                clicktoequip.text = "Click multiple times to open";
                StartCoroutine(CloseText());
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1004:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                player.PlayGIF();
                ChangeMaterial(21);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1005:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(21);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1005);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1006:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(22);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1006);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1007:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(24);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1007);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1008:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(29);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1008);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            case 1010:
                dollar.enabled = true;
                dollarhitbox.enabled = true;

                ChangeMaterial(23);
                notadollar.SetActive(true);

                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                ChangeSprite(1010);
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
            default:
                // Handle general item click
                Debug.Log($"Item with ID {itemId} clicked: Default action.");
                StartCoroutine(ChangeAllImagesColors(Color.white, 1f));
                break;
        }

        if (itemId < 499 || itemId > 600)
        {
            previousitem = itemId;
        }

    }
    public GameObject autoclickerobject;
    public GameObject autoclickerbutton;
    public GameObject neonobjects;
    public GameObject neonobjects2;
    private int previousitem;
    public GameObject musicicon;
    public List<GameObject> dayClouds; // Assign your GameObjects in the Inspector
    public List<GameObject> nightClouds;
    public List<GameObject> moonandstars;
    public List<GameObject> neon;

    public float resetXPosition = -300f; // X position where clouds reset
    public float minTravelTime = 15f; // Minimum time to cross the screen
    public float maxTravelTime = 20f; // Maximum time to cross the screen
    public float offsetX = 300f; // Offset to start clouds off-screen

    private List<Coroutine> cloudMovementCoroutines = new List<Coroutine>();
    private void PlaceMoonAndStars()
    {
        foreach (GameObject celestialObject in moonandstars)
        {
            Vector2 randomPosition = GetRandomPosition();
            celestialObject.transform.position = new Vector3(randomPosition.x, randomPosition.y, celestialObject.transform.position.z);
        }
    }
    private bool isRunning2;
    private HashSet<GameObject> currentlyFading = new HashSet<GameObject>();

    public void StartFading()
    {
        if (!isRunning2)
        {
            isRunning2 = true;
            StartCoroutine(ColorChange(Color.black));
            StartCoroutine(PlaceNeon());
        }
    }

    public void StopFading()
    {
        isRunning2 = false;
    }

    private IEnumerator PlaceNeon()
    {
        float elapsedTime = 0f;
        float interval = 2f; // Start with a 2-second interval between color changes

        while (isRunning2)
        {
            // Randomly select 1-3 objects from the array to reduce frequency
            GameObject[] selectedObjects = neon.OrderBy(x => UnityEngine.Random.value).Take(UnityEngine.Random.Range(1, 4)).ToArray();

            foreach (GameObject celestialObject in selectedObjects)
            {
                // Ensure the object is active before starting the color change
                celestialObject.SetActive(true);

                // Check if the object is already undergoing the color change process
                if (!currentlyFading.Contains(celestialObject))
                {
                    currentlyFading.Add(celestialObject);

                    // Determine if the object has an Image or SpriteRenderer component
                    Image image = celestialObject.GetComponent<Image>();
                    SpriteRenderer spriteRenderer = celestialObject.GetComponent<SpriteRenderer>();

                    if (image != null || spriteRenderer != null)
                    {
                        // Start the color switch process
                        StartCoroutine(ColorSwitch(image, spriteRenderer, celestialObject));
                    }
                }
            }

            // Wait for the current interval before the next round
            yield return new WaitForSeconds(interval);

            // Increase the frequency of color changes as time progresses
            elapsedTime += interval;
            if (elapsedTime < 10f)
            {
                interval = Mathf.Max(0.5f, interval - 0.2f); // Decrease interval to a minimum of 0.5 seconds
            }
            else if (elapsedTime >= 10f && elapsedTime < 15f)
            {
                interval = Mathf.Max(0.1f, interval - 0.1f); // Further decrease interval to a minimum of 0.1 seconds
            }
            else if (elapsedTime >= 15f)
            {
                // After 15 seconds, trigger the final flash and reset the pattern
                yield return StartCoroutine(FinalFlash());
                elapsedTime = 0f; // Reset the elapsed time
                interval = 2f; // Reset the interval
            }
        }
    }

    private IEnumerator ColorSwitch(Image image, SpriteRenderer spriteRenderer, GameObject celestialObject)
    {
        // Immediately change color to grey
        if (image != null)
        {
            image.color = Color.grey;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.grey;
        }

        // Wait for 0.5 seconds while the color is grey
        yield return new WaitForSeconds(0.5f);

        // Change color back to white
        if (image != null)
        {
            image.color = Color.white;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // Remove the object from the currently fading set
        currentlyFading.Remove(celestialObject);
    }

    private IEnumerator FinalFlash()
    {
        for (int i = 0; i < 2; i++)
        {
            // Change all objects to grey simultaneously
            foreach (GameObject celestialObject in neon)
            {
                celestialObject.SetActive(true);

                Image image = celestialObject.GetComponent<Image>();
                SpriteRenderer spriteRenderer = celestialObject.GetComponent<SpriteRenderer>();

                if (image != null)
                {
                    image.color = Color.grey;
                }
                else if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.grey;
                }
            }

            // Wait for 0.5 seconds while all objects are grey
            yield return new WaitForSeconds(0.5f);

            // Change all objects back to white simultaneously
            foreach (GameObject celestialObject in neon)
            {
                Image image = celestialObject.GetComponent<Image>();
                SpriteRenderer spriteRenderer = celestialObject.GetComponent<SpriteRenderer>();

                if (image != null)
                {
                    image.color = Color.white;
                }
                else if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
            }

            // Wait for 0.5 seconds before the second flash (for i=0) or the final wait (for i=1)
            yield return new WaitForSeconds(0.5f);
        }

        // Wait for 2 seconds before resuming the normal coroutine
        yield return new WaitForSeconds(2f);

        // Clear the currently fading list to allow normal operation to resume
        currentlyFading.Clear();
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, Image image, float fadeInDuration, float fadeOutDuration, GameObject celestialObject)
    {
        float elapsedTime = 0f;

        // Fade in to grey
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            image.color = Color.grey; // Remain grey while fading in
            yield return null;
        }

        canvasGroup.alpha = 1f;
        image.color = Color.grey;
        yield return new WaitForSeconds(1f);

        // Instant color change to white
        image.color = Color.white;

        // Wait for a random amount of time before fading out
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 1.5f));
        // Reset time and fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            image.color = Color.white; // Remain white while fading out
            yield return null;
        }

        canvasGroup.alpha = 0f;
        image.color = Color.white;

        // Optionally deactivate the object after fading out
        canvasGroup.gameObject.SetActive(false);

        // Remove the object from the currently fading set
        currentlyFading.Remove(celestialObject);
    }

    private Vector2 GetRandomPosition()
    {
        // Get the screen bounds in world units
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;


        // Generate random positions within the screen bounds
        float randomX = UnityEngine.Random.Range(0, screenWidth);
        float randomY = UnityEngine.Random.Range(0, Screen.height);


        return new Vector2(randomX, randomY);
    }
    public void StartMovingClouds()
    {
        StartCoroutine(StartEachCloud());
    }
    public void StartMovingClouds2()
    {
        StartCoroutine(StartEachCloud2());
    }
    
    private IEnumerator StartEachCloud()
    {
        cloudsenabled = true;
        foreach (GameObject cloud in dayClouds)
        {
            if(cloudsenabled)
            {
                Coroutine cloudMovementCoroutine = StartCoroutine(MoveCloud(cloud));
                cloudMovementCoroutines.Add(cloudMovementCoroutine);
                yield return new WaitForSeconds(0.5f);
            }

        }
        yield return null;
    }
    private IEnumerator StartEachCloud2()
    {
        cloudsenabled2 = true;
        foreach (GameObject cloud in nightClouds)
        {
            if(cloudsenabled2)
            {
                Coroutine cloudMovementCoroutine = StartCoroutine(MoveCloud(cloud));
                cloudMovementCoroutines.Add(cloudMovementCoroutine);
                yield return new WaitForSeconds(1f);
            }

        }
        yield return null;
    }
    private bool cloudsenabled;
    private bool cloudsenabled2;
    public void StopMovingClouds()
    {
        cloudsenabled = false;
        cloudsenabled2 = false;
        foreach (Coroutine coroutine in cloudMovementCoroutines)
        {
            StopCoroutine(coroutine);

        }
        cloudMovementCoroutines.Clear();
        foreach (GameObject cloud in dayClouds)
        {
            cloud.transform.position = new Vector3(0f, -10000f, cloud.transform.position.z);
        }
        foreach (GameObject cloud in nightClouds)
        {
            cloud.transform.position = new Vector3(0f, -10000f, cloud.transform.position.z);
        }
        foreach (GameObject cloud in moonandstars)
        {
            cloud.transform.position = new Vector3(0f, -10000f, cloud.transform.position.z);
        }
    }

    private IEnumerator MoveCloud(GameObject cloud)
    {
        while (true)
        {
            // Calculate the starting X position based on the screen width and offset
            float startXPosition = Screen.width + offsetX;

            // Random Y position between 0 and 1080
            float randomY = UnityEngine.Random.Range(0f, Screen.height);
            cloud.transform.position = new Vector3(startXPosition, randomY, cloud.transform.position.z);

            // Calculate the distance the cloud needs to travel (positive difference)
            float distance = startXPosition - resetXPosition;

            // Random time to travel the distance
            float travelTime = UnityEngine.Random.Range(minTravelTime, maxTravelTime);

            // Calculate the speed needed to cover the distance in the given time
            float speed = distance / travelTime;

            while (cloud.transform.position.x > resetXPosition)
            {
                // Move the cloud to the left at the calculated speed
                cloud.transform.position += Vector3.left * speed * Time.deltaTime;
                yield return null; // Continue the loop on the next frame
            }
        }
    }





    public void LogAllMoneyInInventory()
    {
        SteamInventoryResult_t resultHandle;
        uint arraySize = 0;
        int totalItems = 0;

        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize))
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    foreach (var item in itemDetails)
                    {
                        int itemDefinition = (int)item.m_iDefinition;

                        switch (itemDefinition)
                        {
                            case 100:
                                totalItems += 1;
                                break;
                            case 102:
                                totalItems += 100;
                                break;
                            case 103:
                                totalItems += 1000;
                                break;
                            case 104:
                                totalItems += 10000;
                                break;
                            case 105:
                                totalItems += 100000;
                                break;
                            case 106:
                                totalItems += 1000000;
                                break;
                        }
                        //Debug.Log($"Item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                    totalDrops = totalItems;
                    UpdateTotalDropsText();
                    Debug.Log($"Total drops count: {totalDrops}");
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array. Result code: " + SteamInventory.GetResultStatus(resultHandle));
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items. Result code: " + SteamInventory.GetResultStatus(resultHandle));
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory. Result code: " + SteamInventory.GetResultStatus(resultHandle));
        }
    }
    private List<SteamItemInstanceID_t> GetItemIdsFromInventory(SteamItemDef_t itemDefId, int maxItems)
    {
        List<SteamItemInstanceID_t> itemIds = new List<SteamItemInstanceID_t>();
        SteamInventoryResult_t resultHandle;
        uint arraySize = 0;

        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize))
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    foreach (var item in itemDetails)
                    {
                        if (itemIds.Count >= maxItems)
                            break;

                        Debug.Log($"Item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");

                        if (item.m_iDefinition == itemDefId)
                        {
                            Debug.Log($"Found matching item ID: {item.m_itemId}");
                            itemIds.Add(item.m_itemId);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory.");
        }

        if (itemIds.Count == 0)
        {
            Debug.LogWarning("No matching items found in inventory");
        }

        return itemIds;
    }


    private Callback<SteamInventoryResultReady_t> inventoryResultReadyCallback;

    private void OnEnable()
    {
        inventoryResultReadyCallback = Callback<SteamInventoryResultReady_t>.Create(OnInventoryResultReady);
    }

    private void OnDisable()
    {
        inventoryResultReadyCallback.Dispose();
    }
    private bool briefcasecooldown = false;
    IEnumerator BriefcaseCooldown()
    {
        briefcasecooldown = true;
        yield return new WaitForSeconds(0.5f);
        briefcasecooldown = false;
        yield return null;
    }

    public void SimpleExchangeTest()
    {
        if (unstacking)
        {
            return;
        }
        if (briefcasecooldown)
        {
            return;
        }
        else
        {
            StartCoroutine(BriefcaseCooldown());
        }
        Debug.Log("Attempting simple exchange");
        if (exchanging)
        {
            return;
        }

        // Use minimal setup for testing
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(116), 1);
        lootboxType = 1;
        if (inputItems.Count < 1)
        {
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }
        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1;
        }

        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(12);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);
        StartCoroutine(ExchangingCooldown());
        if (success)
        {

            if (resultHandle != SteamInventoryResult_t.Invalid)
            {
                Debug.Log("Exchange request successful, waiting for callback...");
                Debug.Log($"Result handle: {resultHandle}");
            }
            else
            {
                Debug.LogError("Invalid result handle received from exchange request.");
            }
        }
        else
        {
            Debug.LogError("Exchange request failed");
        }
    }
    public int lootboxType;
    public void SimpleExchangeTest2()
    {
        if (unstacking)
        {
            return;
        }
        if (briefcasecooldown)
        {
            return;
        }
        else
        {
            StartCoroutine(BriefcaseCooldown());
        }
        if (exchanging)
        {
            return;
        }
        Debug.Log("Attempting simple exchange");

        // Use minimal setup for testing
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(126), 1);
        lootboxType = 2;
        if (inputItems.Count < 1)
        {
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1;
        }

        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(13);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);
        StartCoroutine(ExchangingCooldown());
        if (success)
        {
            if (resultHandle != SteamInventoryResult_t.Invalid)
            {

                Debug.Log("Exchange request successful, waiting for callback...");
                Debug.Log($"Result handle: {resultHandle}");
            }
            else
            {
                Debug.LogError("Invalid result handle received from exchange request.");
            }
        }
        else
        {
            Debug.LogError("Exchange request failed");
        }
    }
    public void SimpleExchangeTest3()
    {
        if (unstacking)
        {
            return;
        }
        if (briefcasecooldown)
        {
            return;
        }
        else
        {
            StartCoroutine(BriefcaseCooldown());
        }
        Debug.Log("Attempting simple exchange");
        if (exchanging)
        {
            return;
        }
        // Use minimal setup for testing
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(1003), 1);
        lootboxType = 2;
        if (inputItems.Count < 1)
        {
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }
        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1;
        }

        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(14);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);
        StartCoroutine(ExchangingCooldown());
        if (success)
        {

            if (resultHandle != SteamInventoryResult_t.Invalid)
            {
                Debug.Log("Exchange request successful, waiting for callback...");
                Debug.Log($"Result handle: {resultHandle}");
            }
            else
            {
                Debug.LogError("Invalid result handle received from exchange request.");
            }
        }
        else
        {
            Debug.LogError("Exchange request failed");
        }
    }
    public void SimpleExchangeTest4()
    {
        if (unstacking)
        {
            return;
        }
        if (briefcasecooldown)
        {
            return;
        }
        else
        {
            StartCoroutine(BriefcaseCooldown());
        }
        Debug.Log("Attempting simple exchange");
        if (exchanging)
        {
            return;
        }

        // Use minimal setup for testing
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(136), 1);
        lootboxType = 3;
        if (inputItems.Count < 1)
        {
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }
        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1;
        }

        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(15);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);
        StartCoroutine(ExchangingCooldown());
        if (success)
        {

            if (resultHandle != SteamInventoryResult_t.Invalid)
            {
                Debug.Log("Exchange request successful, waiting for callback...");
                Debug.Log($"Result handle: {resultHandle}");
            }
            else
            {
                Debug.LogError("Invalid result handle received from exchange request.");
            }
        }
        else
        {
            Debug.LogError("Exchange request failed");
        }
    }
    IEnumerator HideText()
    {
        yield return new WaitForSeconds(5f);
        inventoryText.text = "";
        yield return null;
    }

    private void OnInventoryResultReady(SteamInventoryResultReady_t callback)
    {
        Debug.Log($"Inventory result callback received with result: {callback.m_result}, handle: {callback.m_handle}");

        // Early exit if the result is not OK or handle is invalid
        if (callback.m_result != EResult.k_EResultOK || callback.m_handle == SteamInventoryResult_t.Invalid)
        {
            Debug.LogError($"Error or invalid handle: {callback.m_result}");
            return;
        }

        // Wait a short period to ensure inventory processing is complete
        StartCoroutine(WaitAndGetDropDetails(callback.m_handle));
    }

    private IEnumerator WaitAndGetDropDetails(SteamInventoryResult_t resultHandle)
    {
        yield return new WaitForSeconds(1.0f); // Waiting to ensure inventory processing

        GetDropDetails(resultHandle); // Attempt to get the drop details
    }

    private void GetDropDetails(SteamInventoryResult_t resultHandle)
    {
        if (resultHandle == SteamInventoryResult_t.Invalid)
        {
            Debug.LogError("Invalid result handle, cannot get drop details.");
            return;
        }

        // Attempt to get the number of items in the result
        uint itemCount = 0;
        if (SteamInventory.GetResultItems(resultHandle, null, ref itemCount))
        {
            Debug.Log($"Number of items to retrieve: {itemCount}");

            if (itemCount > 0)
            {
                SteamItemDetails_t[] items = new SteamItemDetails_t[itemCount];
                if (SteamInventory.GetResultItems(resultHandle, items, ref itemCount))
                {
                    Debug.Log($"Number of new items: {itemCount}");
                    foreach (var item in items)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                        DisplayItemDetails(item);
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("No items found, but call succeeded.");
            }
        }
        else
        {
            Debug.LogError("Failed to get the number of new result items.");
        }

        // Destroy the result handle to free up resources
        SteamInventory.DestroyResult(resultHandle);
    }

    private bool isFirstCallback = true; // Flag to track if it's the first callback

    public void DisplayItemDetails(SteamItemDetails_t item)
    {
        Debug.Log($"Displaying item: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");

        if (isFirstCallback)
        {
            // Ignore the first callback
            isFirstCallback = false;
            return;
        }

        selectedItem = item;
        Item newItem = itemDatabase.GetItemByID((int)item.m_iDefinition);
        //inventoryText.text = $"Received a {newItem.name}";
        if (unstacking)
        {
            return;
        }
        // Start spinning animation with the selected item
        StartCoroutine(SpinItems(item));
    }

    public Text inventoryText;
    public bool isSpinning = false;
    private SteamItemDetails_t selectedItem;
    public HorizontalLayoutGroup itemGrid;
    public Item[] items;
    public GameObject itemPrefab;
    private List<int> additionalItemIDs = new List<int> { 101, 110, 118, 119, 120, 121, 136 };
    private List<int> itemIDs1 = new List<int> { 100, 102, 103, 104, 105, 106, 110, 112, 116, 117, 125, 126, 128, 510 };
    private List<int> itemIDs2 = new List<int> { 101, 104, 105, 106, 110, 117, 118, 119, 120, 121, 126 };
    private List<int> itemIDs3 = new List<int> { 101, 105, 106, 110, 118, 119, 120, 121, 122, 123, 511 };
    public GameObject spinningBar;
    public ParticleSystem golddollars;
    public ParticleSystem silverdollars;
    public ParticleSystem diamond1;
    public ParticleSystem diamond2;

    public Image[] allimages;


    public string mostrecentitemname;

    private IEnumerator ChangeAllImagesColors(Color targetColor, float duration)
    {
        float time = 0;
        Color[] initialColors = new Color[allimages.Length];

        // Store initial colors of all images
        for (int i = 0; i < allimages.Length; i++)
        {
            if (allimages[i] != null)
            {
                initialColors[i] = allimages[i].color;
            }
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            for (int i = 0; i < allimages.Length; i++)
            {
                if (allimages[i] != null)
                {
                    allimages[i].color = Color.Lerp(initialColors[i], targetColor, t);
                }
            }

            yield return null;
        }

        // Ensure all images have the final target color
        for (int i = 0; i < allimages.Length; i++)
        {
            if (allimages[i] != null)
            {
                allimages[i].color = targetColor;
            }
        }
    }
    private int GetRandomItemFromList(List<int> itemList)
    {
        if (itemList == null || itemList.Count == 0)
        {
            throw new System.ArgumentException("Item list cannot be null or empty.");
        }

        int randomIndex = UnityEngine.Random.Range(0, itemList.Count);
        return itemList[randomIndex];
    }
    public DiscordController discordController;
    public AudioSource openingsound;
    private IEnumerator SpinItems(SteamItemDetails_t item)
    {

        isSpinning = true;
        // Populate the grid with items
        int selectedIndex = UnityEngine.Random.Range(15, 26); // Ensure the selected item lands between positions 15 and 25
        if (lootboxType == 1)
        {
            int randomAdditionalItem = GetRandomItemFromList(additionalItemIDs);
            itemIDs1.Add(randomAdditionalItem);
            List<int> randomItems = GenerateRandomItemList((int)item.m_iDefinition, 30, selectedIndex, itemIDs1);

            PopulateGridWithItems(randomItems);
            itemIDs1.Remove(randomAdditionalItem);
        }
        else if (lootboxType == 3)
        {
            int randomAdditionalItem = GetRandomItemFromList(additionalItemIDs);
            itemIDs3.Add(randomAdditionalItem);
            List<int> randomItems = GenerateRandomItemList((int)item.m_iDefinition, 30, selectedIndex, itemIDs3);

            PopulateGridWithItems(randomItems);
            itemIDs3.Remove(randomAdditionalItem);
        }
        else
        {
            int randomAdditionalItem = GetRandomItemFromList(additionalItemIDs);
            itemIDs2.Add(randomAdditionalItem);
            List<int> randomItems = GenerateRandomItemList((int)item.m_iDefinition, 30, selectedIndex, itemIDs2);

            PopulateGridWithItems(randomItems);
            itemIDs2.Remove(randomAdditionalItem);
        }


        float elapsedTime = 0f;
        Vector3 restingPosition = new Vector3(0, -10000, 0);
        Vector3 startPosition = new Vector3 (0, -300, 0);
        Vector3 targetPosition = new Vector3(0, -172.4f, 0);
        while (elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.25f;

            // Using a smooth step function for deceleration
            float smoothStep = t * t * (3f - 2f * t);
            spinningBar.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothStep);
            yield return null;
        }

        Debug.Log($"Selected Index: {selectedIndex}");

        // Hardcoded target positions based on index
        float[] targetPositions = {
        10f,     // 15
        -100f,   // 16
        -210f,   // 17
        -320f,   // 18
        -430f,   // 19
        -540f,   // 20
        -650f,   // 21
        -760f,   // 22
        -870f,   // 23
        -980f,   // 24
        -1090f,   // 25
        -1200f
        };

        float targetX = targetPositions[selectedIndex - 14];



        // Wait a short period to ensure items are initialized
        yield return new WaitForSeconds(0.1f);

        // Move from the start position to the target position with deceleration
        yield return StartCoroutine(MoveToPositionWithDeceleration(1300f, targetX, 3.0f));
        Item newItem = itemDatabase.GetItemByID((int)item.m_iDefinition);

        
        mostrecentitemname = newItem.Name;
        discordController.BriefcaseActivity(newItem.name, lootboxType);
        // Start the cooldown countdown
        inventoryText.text = "Received a " + newItem.Name;
        if (lootboxType == 2)
        {
            golddollars.Play();
            silverdollars.Play();
        }
        else if (lootboxType == 3)
        {
            diamond1.Play();
            diamond2.Play();

        }
        else
        {
            money2.Play();

        }
        // Reset the spinner position for the next use
        yield return new WaitForSeconds(2.5f);
        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;

            // Using a smooth step function for deceleration
            float smoothStep = t * t * (3f - 2f * t);
            spinningBar.transform.localPosition = Vector3.Lerp(targetPosition, startPosition, smoothStep);
            yield return null;
        }
        isSpinning = false;
        spinningBar.transform.localPosition = restingPosition;
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(OpenCaseCooldown());



        itemGrid.transform.localPosition = new Vector3(1300, 0, 0);

    }

    private IEnumerator MoveToPositionWithDeceleration(float startX, float targetX, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = new Vector3(startX, itemGrid.transform.localPosition.y, itemGrid.transform.localPosition.z);
        Vector3 targetPosition = new Vector3(targetX, itemGrid.transform.localPosition.y, itemGrid.transform.localPosition.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Using a smooth step function for deceleration
            float smoothStep = t * t * (3f - 2f * t);
            itemGrid.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothStep);
            yield return null;
        }

        itemGrid.transform.localPosition = targetPosition;
        yield return null;
    }

    private IEnumerator OpenCaseCooldown()
    {
        int countdownTime = 4; // Total countdown time in seconds
        while (countdownTime > 0)
        {
            inventoryText.text = $"{countdownTime} seconds"; // Use string interpolation
            yield return new WaitForSeconds(1f); // Wait for 1 second
            countdownTime--;
        }
        inventoryText.text = ""; // Clear the text after the countdown is complete
    }





    private void PopulateGridWithItems(List<int> itemIDs)
    {
        foreach (Transform item in itemGrid.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (var itemID in itemIDs)
        {
            GameObject newItem = Instantiate(itemPrefab, itemGrid.transform);
            var itemName = newItem.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = newItem.transform.Find("ItemIcon").GetComponent<Image>();
            Item item = itemDatabase.GetItemByID(itemID);
            itemName.text = item.Name;
            itemIcon.sprite = item.Icon;
        }
    }



    private List<int> GenerateRandomItemList(int targetItemID, int totalItems, int targetIndex, List<int> possibleIDs)
    {
        List<int> itemList = new List<int>();
        System.Random rand = new System.Random();

        for (int i = 0; i < totalItems; i++)
        {
            if (i == targetIndex)
            {
                itemList.Add(targetItemID);
            }
            else
            {
                itemList.Add(possibleIDs[rand.Next(0, possibleIDs.Count)]);
            }
        }

        return itemList;
    }
    IEnumerator WaitForText()
    {

        yield return new WaitForSeconds(5f);
        inventoryText.text = "";
        yield return null;
    }
    public void ExchangeForDiamondBriefcaseFunction()
    {
        Debug.Log("Attempting exchange");

        // Define the required item definitions
        SteamItemDef_t[] requiredItems = new SteamItemDef_t[] {
        new SteamItemDef_t(118),
        new SteamItemDef_t(119),
        new SteamItemDef_t(120),
        new SteamItemDef_t(121)
    };

        // Collect the input items
        List<SteamItemInstanceID_t> inputItems = new List<SteamItemInstanceID_t>();
        foreach (var itemDef in requiredItems)
        {
            var items = GetItemIdsFromInventory(itemDef, 1);
            if (items.Count < 1)
            {
                NotEnoughText();
                Debug.LogError($"Not enough items found in inventory for item definition {itemDef.m_SteamItemDef}.");
                return;
            }
            inputItems.Add(items[0]); // Add the first item found for each definition
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item to 1
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(136); // Assume 126 is the Diamond Briefcase
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            successtext.text = "Success";
            successtext2.text = "Success";

            successfulexchange = true;
            StartCoroutine(ClearSuccess());
            diamond1.Play();
            diamond2.Play();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeForDiamondBriefcase()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForDiamondBriefcaseFunction();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForDiamondBriefcaseFunction();
        yield return null;
    }
    public void ButtonForDiamondBriefcase()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForDiamondBriefcase());
    }
    public void ExchangeForGoldenBriefcaseFunction()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(117), 10);
        if (inputItems.Count < 10)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(126);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            successtext.text = "Success";
            successtext2.text = "Success";

            successfulexchange = true;
            StartCoroutine(ClearSuccess());
            golddollars.Play();
            silverdollars.Play();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }
    IEnumerator ExchangeForGoldenBriefcase()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForGoldenBriefcaseFunction();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForGoldenBriefcaseFunction();
        yield return null;
    }
    public void ButtonForGoldenBriefcase()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForGoldenBriefcase());
    }
    public void ExchangeForLighterFunction()
    {
        Debug.Log("Attempting exchange for Lighter");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> topHalfItems = GetItemIdsFromInventory(new SteamItemDef_t(502), 1);
        List<SteamItemInstanceID_t> bottomHalfItems = GetItemIdsFromInventory(new SteamItemDef_t(503), 1);

        if (topHalfItems.Count < 1 || bottomHalfItems.Count < 1)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        SteamItemInstanceID_t[] inputItems = { topHalfItems[0], bottomHalfItems[0] };
        uint[] inputQuantities = { 1, 1 };

        Debug.Log($"Input items and quantities:");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output item and quantity
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(501);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems, inputQuantities, (uint)inputItems.Length);

        if (success)
        {
            Debug.Log("Exchange successful");
            successtext.text = "Success";
            successtext2.text = "Success";

            successfulexchange = true;
            StartCoroutine(ClearSuccess());
            // Play success animations or sounds
            golddollars.Play();
            silverdollars.Play();

            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeForLighter()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForLighterFunction();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForLighterFunction();
        yield return null;
    }

    public void ButtonForLighter()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForLighter());
    }
    public void ExchangeForGoldenLighterFunction()
    {
        Debug.Log("Attempting exchange for Golden Lighter");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> topHalfItems = GetItemIdsFromInventory(new SteamItemDef_t(505), 1);
        List<SteamItemInstanceID_t> bottomHalfItems = GetItemIdsFromInventory(new SteamItemDef_t(506), 1);

        if (topHalfItems.Count < 1 || bottomHalfItems.Count < 1)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        SteamItemInstanceID_t[] inputItems = { topHalfItems[0], bottomHalfItems[0] };
        uint[] inputQuantities = { 1, 1 };

        Debug.Log($"Input items and quantities:");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output item and quantity
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(504);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems, inputQuantities, (uint)inputItems.Length);

        if (success)
        {
            Debug.Log("Exchange successful");
            successtext.text = "Success";
            successtext2.text = "Success";

            successfulexchange = true;
            StartCoroutine(ClearSuccess());
            // Play success animations or sounds
            golddollars.Play();
            silverdollars.Play();

            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeForGoldenLighter()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForGoldenLighterFunction();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForGoldenLighterFunction();
        yield return null;
    }

    public void ButtonForGoldenLighter()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForGoldenLighter());
    }

    public void ExchangeForBriefcaseFunction()
    {
        Debug.Log("Attempting exchange");

        // Setup input items and quantities
        List<SteamItemDetails_t> itemDetailsList = GetItemDetailsFromInventory(new SteamItemDef_t(100), 100);
        if (itemDetailsList.Count < 100)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        // Prepare input items and quantities
        List<SteamItemInstanceID_t> inputItems = new List<SteamItemInstanceID_t>();
        List<uint> inputQuantities = new List<uint>();

        foreach (var itemDetail in itemDetailsList)
        {
            // Ignore items with a quantity greater than 1
            if (itemDetail.m_unQuantity > 1) continue;
            inputItems.Add(itemDetail.m_itemId);
            inputQuantities.Add(itemDetail.m_unQuantity);
            if (inputItems.Count >= 100) break; // Stop if we have enough items
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        for (int i = 0; i < inputItems.Count; i++)
        {
            Debug.Log($"Input item ID: {inputItems[i]}, Quantity: {inputQuantities[i]}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(116);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities.ToArray(), (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();

            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            successtext.text = "Steam says try again";
            successtext2.text = "Steam says try again";

            Debug.LogError("Exchange failed");
        }
    }
    public void ExchangeForBriefcaseFunction2()
    {
        Debug.Log("Attempting exchange");

        // Setup input items and quantities
        List<SteamItemDetails_t> itemDetailsList = GetItemDetailsFromInventory(new SteamItemDef_t(102), 1);
        if (itemDetailsList.Count < 1)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        // Prepare input items and quantities
        List<SteamItemInstanceID_t> inputItems = new List<SteamItemInstanceID_t>();
        List<uint> inputQuantities = new List<uint>();

        foreach (var itemDetail in itemDetailsList)
        {
            inputItems.Add(itemDetail.m_itemId);
            inputQuantities.Add(itemDetail.m_unQuantity);
            if (inputItems.Count >= 1) break; // Stop if we have enough items
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        for (int i = 0; i < inputItems.Count; i++)
        {
            Debug.Log($"Input item ID: {inputItems[i]}, Quantity: {inputQuantities[i]}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(116);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities.ToArray(), (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();

            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            successtext.text = "Steam says try again";
            successtext2.text = "Steam says try again";

            Debug.LogError("Exchange failed");
        }
    }


    // Helper method to get detailed item information from inventory
    private List<SteamItemDetails_t> GetItemDetailsFromInventory(SteamItemDef_t itemDefId, int maxItems)
    {
        List<SteamItemDetails_t> itemDetailsList = new List<SteamItemDetails_t>();
        SteamInventoryResult_t resultHandle;
        uint arraySize = 0;

        if (SteamInventory.GetAllItems(out resultHandle))
        {
            Debug.Log("Fetched all items from inventory");

            if (SteamInventory.GetResultItems(resultHandle, null, ref arraySize))
            {
                SteamItemDetails_t[] itemDetails = new SteamItemDetails_t[arraySize];
                if (SteamInventory.GetResultItems(resultHandle, itemDetails, ref arraySize))
                {
                    foreach (var item in itemDetails)
                    {
                        if (itemDetailsList.Count >= maxItems)
                            break;

                        if (item.m_iDefinition == itemDefId)
                        {
                            itemDetailsList.Add(item);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to get result item details after allocating array.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory.");
        }

        if (itemDetailsList.Count == 0)
        {
            Debug.LogWarning("No matching items found in inventory");
        }

        return itemDetailsList;
    }

    IEnumerator ExchangeForBriefcase()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForBriefcaseFunction();
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForBriefcaseFunction();
        yield return null;
    }
    public void ButtonForBriefcase()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForBriefcase());
    }
    IEnumerator ExchangeForBriefcase2()
    {
        StartCoroutine(ExchangingCooldown());
        ExchangeForBriefcaseFunction2();
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        ExchangeForBriefcaseFunction2();
        yield return null;
    }
    public void ButtonForBriefcase2()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeForBriefcase2());
    }
    public void Exchange100Function()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(100), 100);
        if (inputItems.Count < 100)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(102);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }
    IEnumerator ExchangeFor100()
    {
        StartCoroutine(ExchangingCooldown());
        Exchange100Function();
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange100Function();
        yield return null;
    }
    public void ButtonFor100()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeFor100());
    }
    public void Exchange1000Function()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(102), 10);
        if (inputItems.Count < 10)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(103);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeFor1000()
    {
        StartCoroutine(ExchangingCooldown());
        Exchange1000Function();
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange1000Function();
        yield return null;
    }

    public void ButtonFor1000()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeFor1000());
    }
    public void Exchange10000Function()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(103), 10);
        if (inputItems.Count < 10)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(104);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeFor10000()
    {
        StartCoroutine(ExchangingCooldown());
        Exchange10000Function();
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange10000Function();
        yield return null;
    }
    IEnumerator ExchangingCooldown()
    {
        
        exchanging = true;
        cooldownTime = 15f;
        cooldowntext.SetActive(true);
        cooldowntext2.SetActive(true);

        while (cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(cooldownTime % 60);
            cooldowntimer.text = string.Format("{00}", seconds);
            cooldowntimer2.text = string.Format("{00}", seconds);

            yield return null;
        }
        cooldowntext.SetActive(false);
        cooldowntext2.SetActive(false);
        successtext.text = "";
        successtext2.text = "";
        cooldowntimer.text = "";
        cooldowntimer2.text = "";

        exchanging = false;
        yield return null;
    }

    public void ButtonFor10000()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeFor10000());
    }
    public void Exchange100000Function()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(104), 10);
        if (inputItems.Count < 10)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(105);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            Debug.Log("Exchange successful");
            SuccessText();
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeFor100000()
    {
        StartCoroutine(ExchangingCooldown());
        Exchange100000Function();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange100000Function();
        yield return null;
    }

    public void ButtonFor100000()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeFor100000());
    }
    public void Exchange1000000Function()
    {
        Debug.Log("Attempting exchange");
        // Setup input items and quantities
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(105), 10);
        if (inputItems.Count < 10)
        {
            NotEnoughText();
            Debug.LogError("Not enough items found in inventory for exchange.");
            return;
        }

        uint[] inputQuantities = new uint[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            inputQuantities[i] = 1; // Set quantity for each input item (adjust as needed)
        }

        Debug.Log($"Input items and quantities (Total: {inputItems.Count}):");
        foreach (var inputItem in inputItems)
        {
            Debug.Log($"Input item ID: {inputItem.m_SteamItemInstanceID}");
        }

        // Setup output items and quantities
        SteamItemDef_t[] outputItems = new SteamItemDef_t[1];
        outputItems[0] = new SteamItemDef_t(106);
        uint[] outputQuantity = new uint[1];
        outputQuantity[0] = 1;

        Debug.Log($"Output item definition ID: {outputItems[0].m_SteamItemDef}, Quantity: {outputQuantity[0]}");

        // Perform the exchange
        SteamInventoryResult_t resultHandle;
        bool success = SteamInventory.ExchangeItems(out resultHandle, outputItems, outputQuantity, (uint)outputItems.Length, inputItems.ToArray(), inputQuantities, (uint)inputItems.Count);

        if (success)
        {
            SuccessText();
            Debug.Log("Exchange successful");
            uint newArraySize = 0;
            if (SteamInventory.GetResultItems(resultHandle, null, ref newArraySize))
            {
                SteamItemDetails_t[] newItems = new SteamItemDetails_t[newArraySize];
                if (SteamInventory.GetResultItems(resultHandle, newItems, ref newArraySize))
                {
                    foreach (var item in newItems)
                    {
                        Debug.Log($"New item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to get new result items details.");
                }
            }
            else
            {
                Debug.LogError("Failed to get the number of new result items.");
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Exchange failed");
        }
    }

    IEnumerator ExchangeFor1000000()
    {
        StartCoroutine(ExchangingCooldown());
        Exchange1000000Function();
        successtext.text = "";
        successtext2.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange1000000Function();
        yield return null;
    }

    public void ButtonFor1000000()
    {
        if (exchanging)
        {
            return;
        }
        StartCoroutine(ExchangeFor1000000());
    }


}
