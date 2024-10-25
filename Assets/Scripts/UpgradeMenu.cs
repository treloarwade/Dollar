using UnityEngine;
using System.Collections;


public class UpgradeMenu : MonoBehaviour
{
    public GameObject Menu;
    private Vector3 openPosition = new Vector3(0, -200, 0);
    private Vector3 closedPosition = new Vector3(0, -400, 0);
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
    public GameObject moneytree;
    public GameObject borders;
    public long moneyToAdd; // Adjust this value as needed
    public ToggleRigidbody2D toggleRigidbody2D;
    public void BordersOnOff()
    {
        if (toggleRigidbody2D.fixeddollar && !cowphysicsitem.activeSelf && !atmphysicsitem.activeSelf && !duckphysicsitem.activeSelf && !diamondduckphysicsitem.activeSelf && !pumpkinphysicsitem.activeSelf && !minecartphysicsitem.activeSelf)
        {
            borders.SetActive(false);
        }
        else
        {
            borders.SetActive(true);
        }
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
