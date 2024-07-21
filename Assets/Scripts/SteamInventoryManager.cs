using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class SteamInventoryManager : MonoBehaviour
{
    public Text timer;
    public Text cooldowntimer;
    public Text successtext;
    public GameObject cooldowntext;
    public Text totaldrops;
    private int totalDrops = 0;
    private int previousTotalDrops = 0;
    private float cooldownTime = 60f;
    private bool exchanging = false;
    private bool isRunning = false;
    public ParticleSystem money2;
    public Transform ItemContent;
    public GameObject InventoryItem;
    public GameObject SmallerInventoryItem;
    public GameObject InventoryScreen;
    public ItemDatabase itemDatabase;
    private Coroutine toggleCoroutine = null;
    public Image background;
    public Text clicktoequip;
    public Image clicker;
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
    private void Start()
    {
        // Call CheckForItemDrop after 5 seconds and then repeat every 3 minutes (180 seconds)
        InvokeRepeating("TriggerItemDrop", 2f, 180.2f);
        StartCoroutine(StartCountdown(1800f));
        IncrementDrops();

        openPosition = new Vector3(InventoryScreen.transform.localPosition.x, 1980, InventoryScreen.transform.localPosition.z);
        closedPosition = new Vector3(InventoryScreen.transform.localPosition.x, 1580f, InventoryScreen.transform.localPosition.z);
        restingPosition = new Vector3(InventoryScreen.transform.localPosition.x, -5000f, InventoryScreen.transform.localPosition.z);
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
            targetPosition = openPosition; // Open position first
            startingPosition = closedPosition;
        }
        else // If currently open, close it
        {
            targetPosition = closedPosition; // Close position next
            startingPosition = openPosition;
        }

        toggleCoroutine = StartCoroutine(MoveInventoryScreen(targetPosition, startingPosition));
    }

    private IEnumerator MoveInventoryScreen(Vector3 targetPosition, Vector3 startingPosition)
    {
        Vector3 startPosition = InventoryScreen.transform.localPosition;
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

        // Animate the movement
        while (elapsedTime < duration)
        {
            InventoryScreen.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            Color backgroundColor = background.color;
            backgroundColor.a = newAlpha;
            background.color = backgroundColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finalize the position and alpha
        InventoryScreen.transform.localPosition = targetPosition;
        Color finalColor = background.color;
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
    private void CheckTotalDropsChange()
    {
        Debug.Log("previousTotalDrops" + previousTotalDrops);
        Debug.Log("totalDrops" + totalDrops);
        if (totalDrops - previousTotalDrops == 1)
        {

                StartCoroutine(StartCountdown(1780f));

        }
        previousTotalDrops = totalDrops; // Update previousTotalDrops to the current totalDrops
    }
    public void SuccessText()
    {
        successtext.text = "Successfully stacked";
        money2.Play();
    }
    public void NotEnoughText()
    {
        successtext.text = "Failed, not enough items";
    }   
    public void IncrementDrops()
    {
        previousTotalDrops = totalDrops;
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
    public void GenerateItem()
    {
        SteamInventoryResult_t inventoryResult;
        SteamItemDef_t[] itemDefs = new SteamItemDef_t[116];
        for (int i = 0; i < itemDefs.Length; i++)
        {
            // Initialize each element as needed
            itemDefs[i] = new SteamItemDef_t(116);
        }
        //bool success = SteamInventory.GenerateItems(out inventoryResult, itemDefs, null, 1);
        SteamInventory.GenerateItems(out inventoryResult, itemDefs, null, 1);
        SteamInventory.DestroyResult(inventoryResult);
    }
    private float lastCheckTime = -300f; // Initialize with a time 5 minutes ago

    public void TriggerItemDrop()
    {
        Debug.Log("Item Dropping");

        SteamInventoryResult_t inventoryResult;
        bool triggerSuccess = SteamInventory.TriggerItemDrop(out inventoryResult, new SteamItemDef_t(10));

        if (!isRunning)
        {
            // StartCoroutine(StartCountdown());
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
        CheckTotalDropsChange();
        UpdateTotalDropsText();
        yield return null;
    }
    public void LogAllItemsInInventory()
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
                    Dictionary<int, int> itemCounts = new Dictionary<int, int>(); // Dictionary to count items

                    foreach (var item in itemDetails)
                    {
                        //Debug.Log($"Item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");

                        // Adjust the definition ID to match your database indexing
                        int itemId = (int)item.m_iDefinition;
                        if (itemCounts.ContainsKey(itemId))
                        {
                            itemCounts[itemId] += (int)item.m_unQuantity;
                        }
                        else
                        {
                            itemCounts[itemId] = (int)item.m_unQuantity;
                        }

                        // Add item quantity to totalItems
                        totalItems += (int)item.m_unQuantity;
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
                Debug.LogError("Failed to get the number of result items. Result code: " + SteamInventory.GetResultStatus(resultHandle));
            }

            SteamInventory.DestroyResult(resultHandle);
        }
        else
        {
            Debug.LogError("Failed to fetch all items from inventory. Result code: " + SteamInventory.GetResultStatus(resultHandle));
        }
    }


    private void UpdateInventoryUI(Dictionary<int, int> itemCounts)
    {
        // Determine whether to use the smaller item prefab
        bool useSmallerItems = Items.Count > 8;
        GameObject itemPrefab = useSmallerItems ? SmallerInventoryItem : InventoryItem;

        // Set the cell size based on the item type
        GridLayoutGroup gridLayoutGroup = ItemContent.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.cellSize = useSmallerItems ? new Vector2(150, 150) : new Vector2(200, 200);
        }

        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(itemPrefab, ItemContent);
            Button button = obj.GetComponent<Button>();
            int itemId = item.ID;
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            button.onClick.AddListener(() => ToggleInventory());
            button.onClick.AddListener(() => OnInventoryItemClick(itemId));
            int quantity = itemCounts[item.ID];
            string displayName = item.Name;

            // List of item IDs to ignore for the pluralization rule
            List<int> ignorePluralization = new List<int> { 102, 103, 104, 105, 106 };

            // Check if the item ID should be ignored for the pluralization rule
            if (!ignorePluralization.Contains(item.ID) && quantity > 1)
            {
                displayName += "s"; // Append 's' for plural
            }

            itemName.text = quantity.ToString() + " " + displayName;
            itemIcon.sprite = item.Icon;
        }
    }
    public GameObject normalbutton;
    public GameObject briefcase;
    public GameObject twodollar;
    public GameObject notadollar;
    public Text counter;
    private void OnInventoryItemClick(int itemId)
    {
        // Initially hide both buttons
        normalbutton.SetActive(false);
        briefcase.SetActive(false);
        twodollar.SetActive(false);
        notadollar.SetActive(false);

        sparkle.Stop();
        confetti.Stop();
        silversparkle.Stop();

        switch (itemId)
        {
            case 100:
                // Handle item ID 100 logic
                normalbutton.SetActive(true);
                ChangeMaterial(0);
                ChangeSprite(100);
                break;
            case 101:
                // Handle item ID 101 logic
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                normalbutton.SetActive(true);
                ChangeMaterial(1);
                ChangeSprite(101);
                break;
            case 102:
                // Handle item ID 102 logic
                normalbutton.SetActive(true);
                ChangeMaterial(5);
                ChangeSprite(102);
                break;
            case 103:
                // Handle item ID 103 logic
                normalbutton.SetActive(true);
                ChangeMaterial(6);
                ChangeSprite(103);
                break;
            case 104:
                // Handle item ID 104 logic
                normalbutton.SetActive(true);
                ChangeMaterial(6);
                ChangeSprite(104);
                break;
            case 105:
                // Handle item ID 105 logic
                normalbutton.SetActive(true);
                ChangeMaterial(7);
                ChangeSprite(105);
                break;
            case 106:
                // Handle item ID 106 logic
                normalbutton.SetActive(true);
                ChangeMaterial(8);
                ChangeSprite(106);
                break;
            case 107:
                // Handle item ID 107 logic
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                    confetti.Play();
                }
                normalbutton.SetActive(true);
                ChangeMaterial(4);
                ChangeSprite(100);
                break;
            case 108:
                // Handle item ID 108 logic
                normalbutton.SetActive(true);
                Debug.Log("Item 108 clicked: Special action here.");
                ChangeSprite(109);
                break;
            case 109:
                // Handle item ID 109 logic
                normalbutton.SetActive(true);
                ChangeMaterial(3);
                ChangeSprite(109);
                break;
            case 110:
                // Handle item ID 110 logic
                if (moveScreens.currentScreen == 2)
                {
                    sparkle.Play();
                }
                normalbutton.SetActive(true);
                ChangeMaterial(2);
                ChangeSprite(110);
                break;
            case 111:
                notadollar.SetActive(true);
                ChangeMaterial(0);
                ChangeSprite(111);
                break;
            case 112:
                twodollar.SetActive(true);
                ChangeMaterial(0);
                ChangeSprite(112);
                break;
            case 113:
                normalbutton.SetActive(true);
                ChangeMaterial(9);
                ChangeSprite(113);
                break;
            case 114:
                normalbutton.SetActive(true);
                ChangeMaterial(10);
                ChangeSprite(114);
                break;
            case 115:
                normalbutton.SetActive(true);
                ChangeMaterial(10);
                ChangeSprite(114);
                break;
            case 116:
                // Handle item ID 116 logic
                briefcase.SetActive(true);
                counter.text = "";
                break;
            default:
                // Handle general item click
                normalbutton.SetActive(true);
                Debug.Log($"Item with ID {itemId} clicked: Default action.");
                break;
        }
    }
    public void StopParticles()
    {
        sparkle.Stop();
        confetti.Stop();
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
                    int count = 0;
                    foreach (var item in itemDetails)
                    {
                        if (count >= maxItems)
                            break;

                        Debug.Log($"Item ID: {item.m_itemId}, Definition: {item.m_iDefinition}, Quantity: {item.m_unQuantity}");

                        if (item.m_iDefinition == itemDefId)
                        {
                            Debug.Log($"Found matching item ID: {item.m_itemId}");
                            itemIds.Add(item.m_itemId);
                            count++;
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


    public void SimpleExchangeTest()
    {
        Debug.Log("Attempting simple exchange");
        StartCoroutine(ExchangingCooldown());
        // Use minimal setup for testing
        List<SteamItemInstanceID_t> inputItems = GetItemIdsFromInventory(new SteamItemDef_t(116), 1);
        if (inputItems.Count < 1)
        {
            Debug.LogError("Not enough items found in inventory for exchange.");
            inventoryText.text = "Out of briefcases";
            StartCoroutine(HideText());
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

        // Start spinning animation with the selected item
        StartCoroutine(SpinItems(item));
    }

    public Text inventoryText;
    public bool isSpinning = false;
    private SteamItemDetails_t selectedItem;
    public HorizontalLayoutGroup itemGrid;
    public Item[] items;
    public GameObject itemPrefab;
    private List<int> itemIDs = new List<int> { 100, 101, 102, 103, 104, 105, 106, 109, 110, 111, 112, 116 };
    public GameObject spinningBar;


    public string mostrecentitemname;

    private IEnumerator SpinItems(SteamItemDetails_t item)
    {
        isSpinning = true;
        // Populate the grid with items
        int selectedIndex = Random.Range(15, 26); // Ensure the selected item lands between positions 15 and 25

        List<int> randomItems = GenerateRandomItemList((int)item.m_iDefinition, 30, selectedIndex);
        PopulateGridWithItems(randomItems);
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


        Debug.Log($"Selected Item: {newItem.Name}");

        // Start the cooldown countdown
        inventoryText.text = "Received a " + newItem.Name;
        money2.Play();
        mostrecentitemname = newItem.Name;
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
        int countdownTime = 6; // Total countdown time in seconds
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



    private List<int> GenerateRandomItemList(int targetItemID, int totalItems, int targetIndex)
    {
        List<int> itemList = new List<int>();
        int[] possibleIDs = { 100, 101, 102, 103, 104, 105, 106, 109, 110, 111, 112, 116 };
        System.Random rand = new System.Random();

        for (int i = 0; i < totalItems; i++)
        {
            if (i == targetIndex)
            {
                itemList.Add(targetItemID);
            }
            else
            {
                itemList.Add(possibleIDs[rand.Next(0, possibleIDs.Length)]);
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
    public void ButtonForBriefcase()
    {
        if (exchanging)
        {
            return;
        }
        SimpleExchangeTest();
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
        successtext.text = "";
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
        successtext.text = "";
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
        successtext.text = "";
        yield return new WaitForSeconds(0.6f);
        Exchange10000Function();
        yield return null;
    }
    IEnumerator ExchangingCooldown()
    {
        exchanging = true;
        cooldownTime = 15f;
        cooldowntext.SetActive(true);
        while (cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(cooldownTime % 60);
            cooldowntimer.text = string.Format("{00}", seconds);
            yield return null;
        }
        cooldowntext.SetActive(false);
        // Ensure timer shows 00:00 at the end
        successtext.text = "";
        cooldowntimer.text = "";
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
