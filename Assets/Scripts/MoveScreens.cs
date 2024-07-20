using System.Collections;
using UnityEngine;

public class MoveScreens : MonoBehaviour
{
    public GameObject Screen1;
    public GameObject Screen2;
    public GameObject Screen3;
    public GameObject Screen4;
    public GameObject Screen5;
    public GameObject Screen6;
    public GameObject Screen7;
    private bool isMoving = false;
    public int currentScreen = 2;
    public int resolutionsetting = 800;
    public SteamInventoryManager inventoryManager;

    public void NextScreen()
    {

        if (!isMoving && currentScreen < 4)
        {
            StartCoroutine(MoveToScreen(currentScreen + 1));
            inventoryManager.StopParticles();
        }
    }

    public void PreviousScreen()
    {
        if (!isMoving && currentScreen > 1)
        {
            StartCoroutine(MoveToScreen(currentScreen - 1));
            inventoryManager.StopParticles();
        }
    }
    private void Update()
    {
        resolutionsetting = Screen.width;
    }
    private IEnumerator MoveToScreen(int targetScreen)
    {
        isMoving = true;

        Vector3 screen1StartPos = Screen1.transform.position;
        Vector3 screen2StartPos = Screen2.transform.position;
        Vector3 screen3StartPos = Screen3.transform.position;
        Vector3 screen4StartPos = Screen4.transform.position;
        Vector3 screen5StartPos = Screen5.transform.position;
        Vector3 screen6StartPos = Screen6.transform.position;
        Vector3 screen7StartPos = Screen7.transform.position;

        Vector3 screen1EndPos = screen1StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen2EndPos = screen2StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen3EndPos = screen3StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen4EndPos = screen4StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen5EndPos = screen5StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen6EndPos = screen6StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen7EndPos = screen7StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        currentScreen = targetScreen;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            Screen1.transform.position = Vector3.Lerp(screen1StartPos, screen1EndPos, t);
            Screen2.transform.position = Vector3.Lerp(screen2StartPos, screen2EndPos, t);
            Screen3.transform.position = Vector3.Lerp(screen3StartPos, screen3EndPos, t);
            Screen4.transform.position = Vector3.Lerp(screen4StartPos, screen4EndPos, t);
            Screen5.transform.position = Vector3.Lerp(screen5StartPos, screen5EndPos, t);
            Screen6.transform.position = Vector3.Lerp(screen6StartPos, screen6EndPos, t);
            Screen7.transform.position = Vector3.Lerp(screen7StartPos, screen7EndPos, t);
            yield return null;
        }

        Screen1.transform.position = screen1EndPos;
        Screen2.transform.position = screen2EndPos;
        Screen3.transform.position = screen3EndPos;
        Screen4.transform.position = screen4EndPos;
        Screen5.transform.position = screen5EndPos;
        Screen6.transform.position = screen6EndPos;
        Screen7.transform.position = screen7EndPos;


        isMoving = false;
    }
}
