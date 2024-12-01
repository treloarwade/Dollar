using UnityEngine;
using System.Collections;


public class UpgradeMenu : MonoBehaviour
{
    public GameObject Menu;
    private Vector3 openPosition = new Vector3(0, -100, 0);
    private Vector3 closedPosition = new Vector3(0, -450, 0);
    private Vector3 restingPosition = new Vector3(0, -10000, 0);
    private Vector3 itemposition;

    private Coroutine toggleCoroutine;
    private bool isOpen = false; // Track if the menu is currently open
    public GameObject atmphysicsitem;
    public GameObject cowphysicsitem;
    public GameObject duckphysicsitem;
    public GameObject diamondduckphysicsitem;
    public GameObject minecartphysicsitem;
    public GameObject pumpkinphysicsitem;
    public GameObject swordphysicsitem;
    public GameObject blueswordphysicsitem;
    public GameObject greenswordphysicsitem;
    public GameObject onyxswordphysicsitem;
    public GameObject purpleswordphysicsitem;
    public GameObject nightswordphysicsitem;
    public GameObject potofgoldphysicsitem;
    public bool forceborderson = false;

    public GameObject moneytree;
    public GameObject borders;
    public long moneyToAdd; // Adjust this value as needed
    public ToggleRigidbody2D toggleRigidbody2D;
    public void BordersOnOff()
    {
        if (!forceborderson && toggleRigidbody2D.fixeddollar && !cowphysicsitem.activeSelf && !atmphysicsitem.activeSelf && !duckphysicsitem.activeSelf && !diamondduckphysicsitem.activeSelf && !pumpkinphysicsitem.activeSelf && !minecartphysicsitem.activeSelf && !swordphysicsitem.activeSelf && !nightswordphysicsitem.activeSelf && !onyxswordphysicsitem.activeSelf && !purpleswordphysicsitem.activeSelf && !greenswordphysicsitem.activeSelf && !blueswordphysicsitem.activeSelf && !potofgoldphysicsitem.activeSelf)
        {
            borders.SetActive(false);
        }
        else
        {
            borders.SetActive(true);
        }
    }
    public void BordersOn()
    {
        forceborderson = true;
    }
    public void BordersOff()
    {
        forceborderson = true;
    }
    public GameObject[] atmPhysicsItems;  // Array of ATM GameObjects
    public GameObject[] cowPhysicsItems;  // Array of ATM GameObjects
    public GameObject[] cartPhysicsItems;  // Array of ATM GameObjects

    public GameObject[] treePhysicsItems;  // Array of ATM GameObjects
    public GameObject[] goldPhysicsItems;  // Array of ATM GameObjects




    public void ToggleATMs(int count)
    {
        // Make sure the count doesn't exceed the total number of ATMs in the array
        count = Mathf.Clamp(count, 0, atmPhysicsItems.Length);

        Debug.Log($"Toggling {count} ATMs.");

        for (int i = 0; i < atmPhysicsItems.Length; i++)
        {
            if (i < count)
            {
                EnableATM(atmPhysicsItems[i]);  // Enable if within count
            }
            else
            {
                DisableATM(atmPhysicsItems[i]);  // Disable if outside count
            }
        }
    }
    public void ToggleCows(int count)
    {
        // Make sure the count doesn't exceed the total number of ATMs in the array
        count = Mathf.Clamp(count, 0, cowPhysicsItems.Length);

        Debug.Log($"Toggling {count} ATMs.");

        for (int i = 0; i < cowPhysicsItems.Length; i++)
        {
            if (i < count)
            {
                EnableATM(cowPhysicsItems[i]);  // Enable if within count
            }
            else
            {
                DisableATM(cowPhysicsItems[i]);  // Disable if outside count
            }
        }
    }
    public void ToggleCarts(int count)
    {
        // Make sure the count doesn't exceed the total number of ATMs in the array
        count = Mathf.Clamp(count, 0, cartPhysicsItems.Length);

        Debug.Log($"Toggling {count} ATMs.");

        for (int i = 0; i < cartPhysicsItems.Length; i++)
        {
            if (i < count)
            {
                EnableATM(cartPhysicsItems[i]);  // Enable if within count
            }
            else
            {
                DisableATM(cartPhysicsItems[i]);  // Disable if outside count
            }
        }
    }
    public void TogglePots(int count)
    {
        // Make sure the count doesn't exceed the total number of ATMs in the array
        count = Mathf.Clamp(count, 0, goldPhysicsItems.Length);

        Debug.Log($"Toggling {count} ATMs.");

        for (int i = 0; i < goldPhysicsItems.Length; i++)
        {
            if (i < count)
            {
                EnableATM(goldPhysicsItems[i]);  // Enable if within count
            }
            else
            {
                DisableATM(goldPhysicsItems[i]);  // Disable if outside count
            }
        }
    }
    public void ToggleTrees(int count)
    {
        // Make sure the count doesn't exceed the total number of ATMs in the array
        count = Mathf.Clamp(count, 0, treePhysicsItems.Length);

        Debug.Log($"Toggling {count} ATMs.");

        for (int i = 0; i < treePhysicsItems.Length; i++)
        {
            if (i < count)
            {
                EnableTree(treePhysicsItems[i]);  // Enable if within count
            }
            else
            {
                DisableATM(treePhysicsItems[i]);  // Disable if outside count
            }
        }
    }
    // Helper method to enable an ATM GameObject and set its position
    private void EnableATM(GameObject atm)
    {
        if (!atm.activeSelf)
        {
            atm.SetActive(true);
            atm.transform.localPosition = new Vector3(0, 4, 10);
        }
        BordersOnOff();  // Adjust borders if needed (if applicable for each ATM)
    }
    private void EnableTree(GameObject atm)
    {
        atm.SetActive(true);
        BordersOnOff();  // Adjust borders if needed (if applicable for each ATM)
    }

    // Helper method to disable an ATM GameObject
    private void DisableATM(GameObject atm)
    {
        atm.SetActive(false);
        BordersOnOff();  // Adjust borders if needed (if applicable for each ATM)
    }

    public void ToggleAtmPhysicsItem()
    {
        atmphysicsitem.SetActive(!atmphysicsitem.activeSelf);
        atmphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void TogglePumpkinPhysicsItem()
    {
        pumpkinphysicsitem.SetActive(!pumpkinphysicsitem.activeSelf);
        pumpkinphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleCowPhysicsItem()
    {
        cowphysicsitem.SetActive(!cowphysicsitem.activeSelf);
        cowphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleDuckPhysicsItem()
    {
        duckphysicsitem.SetActive(!duckphysicsitem.activeSelf);
        duckphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleDiamondDuckPhysicsItem()
    {
        diamondduckphysicsitem.SetActive(!diamondduckphysicsitem.activeSelf);
        diamondduckphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleMinecartPhysicsItem()
    {
        minecartphysicsitem.SetActive(!minecartphysicsitem.activeSelf);
        minecartphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleSwordPhysicsItem()
    {
        swordphysicsitem.SetActive(!swordphysicsitem.activeSelf);
        swordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleBlueSwordPhysicsItem()
    {
        blueswordphysicsitem.SetActive(!blueswordphysicsitem.activeSelf);
        blueswordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleGreenSwordPhysicsItem()
    {
        greenswordphysicsitem.SetActive(!greenswordphysicsitem.activeSelf);
        greenswordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void TogglePurpleSwordPhysicsItem()
    {
        purpleswordphysicsitem.SetActive(!purpleswordphysicsitem.activeSelf);
        purpleswordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleOnyxSwordPhysicsItem()
    {
        onyxswordphysicsitem.SetActive(!onyxswordphysicsitem.activeSelf);
        onyxswordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleNightSwordPhysicsItem()
    {
        nightswordphysicsitem.SetActive(!nightswordphysicsitem.activeSelf);
        nightswordphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void TogglePotOfGoldPhysicsItem()
    {
        potofgoldphysicsitem.SetActive(!potofgoldphysicsitem.activeSelf);
        potofgoldphysicsitem.transform.localPosition = new Vector3(0, 4, 10);
        BordersOnOff();
    }
    public void ToggleTree()
    {
        moneytree.SetActive(!moneytree.activeSelf);
    }
    public void AddMoney()
    {
        moneyToAdd =+ 1;
    }
    public void ToggleUpgradeMenu()
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

        toggleCoroutine = StartCoroutine(MoveMenuScreen(targetPosition, startingPosition));
    }

    private IEnumerator MoveMenuScreen(Vector3 targetPosition, Vector3 startingPosition)
    {
        float duration = 1.0f;
        float elapsedTime = 0f;

        // Animate the movement
        while (elapsedTime < duration)
        {
            Menu.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finalize the position
        Menu.transform.localPosition = targetPosition;

        // If closing, move to resting position
        if (targetPosition == closedPosition)
        {
            yield return new WaitForSeconds(0.1f); // Optional delay
            Menu.transform.localPosition = restingPosition;
        }

        // Update the isOpen flag
        isOpen = targetPosition == openPosition;

        toggleCoroutine = null;
    }
}
