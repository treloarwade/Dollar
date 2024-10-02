using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveScreens : MonoBehaviour
{
    public SpriteRenderer dollar;
    public Text clickcount;
    public GameObject Screen1;
    public GameObject Screen2;
    public GameObject Screen3;
    public GameObject Screen4;
    private bool isMoving = false;
    public int currentScreen = 2;
    public int resolutionsetting = 800;
    public SteamInventoryManager inventoryManager;
    public int dollartype;

    public void SetDollarType(int inputdollartype)
    {
        dollartype = inputdollartype;
    }

    public void NextScreen()
    {

        if (!isMoving && !inventoryManager.isSpinning && currentScreen < 4)
        {
            if (currentScreen == 2)
            {
                if (dollartype != 116 && dollartype != 126 && dollartype != 1003)
                {
                    StartCoroutine(MoveDollar(dollar.transform, new Vector3(0f, dollar.transform.position.y, dollar.transform.position.z), new Vector3(-30f, dollar.transform.position.y, dollar.transform.position.z), 1f));
                }

            }
            if (currentScreen == 1)
            {
                if (dollartype != 116 && dollartype != 126 && dollartype != 1003)
                {
                    StartCoroutine(MoveDollar(dollar.transform, new Vector3(30f, dollar.transform.position.y, dollar.transform.position.z), new Vector3(0f, dollar.transform.position.y, dollar.transform.position.z), 1f));
                }

            }
            else
            {
                //dollar.enabled = false;
                clickcount.text = "";
            }
            StartCoroutine(MoveToScreen(currentScreen + 1));
            //inventoryManager.StopParticles();
        }
        if (currentScreen == 3 && itemshop.value == 1)
        {
            diamondshinyObject.SetActive(true);  // Disable GameObject
            goldenshinyObject.SetActive(true);   // Disable GameObject
        }
        else
        {
            diamondshinyObject.SetActive(false);  // Disable GameObject
            goldenshinyObject.SetActive(false);   // Disable GameObject
        }

    }

    private IEnumerator MoveDollar(Transform dollarTransform, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        dollarTransform.position = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            dollarTransform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        dollarTransform.position = endPos; // Ensure the final position is set
    }
    public void PreviousScreen()
    {
        if (!isMoving && !inventoryManager.isSpinning && currentScreen > 1)
        {
            if (currentScreen == 2)
            {
                if (dollartype != 116 && dollartype != 126 && dollartype != 1003)
                {
                    StartCoroutine(MoveDollar(dollar.transform, new Vector3(0f, dollar.transform.position.y, dollar.transform.position.z), new Vector3(30f, dollar.transform.position.y, dollar.transform.position.z), 1f));
                }

            }
            else
            {
                //dollar.enabled = false;
                clickcount.text = "";
            }
            if (currentScreen == 3)
            {
                if (dollartype != 116 && dollartype != 126 && dollartype != 1003)
                {
                    StartCoroutine(MoveDollar(dollar.transform, new Vector3(-30f, dollar.transform.position.y, dollar.transform.position.z), new Vector3(0f, dollar.transform.position.y, dollar.transform.position.z), 1f));
                }

            }
            else
            {
                //dollar.enabled = false;
                clickcount.text = "";
            }
            StartCoroutine(MoveToScreen(currentScreen - 1));
            //inventoryManager.StopParticles();

        }
        if (currentScreen == 3 && itemshop.value == 1)
        {
            diamondshinyObject.SetActive(true);  // Disable GameObject
            goldenshinyObject.SetActive(true);   // Disable GameObject
        }
        else
        {
            diamondshinyObject.SetActive(false);  // Disable GameObject
            goldenshinyObject.SetActive(false);   // Disable GameObject
        }
    }
    public Dropdown itemshop;
    public GameObject diamondshinyObject;  // Changed to GameObject
    public GameObject goldenshinyObject;   // Changed to GameObject
    private void Update()
    {
        resolutionsetting = Screen.width;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextScreen();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousScreen();
        }
    }
    private IEnumerator MoveToScreen(int targetScreen)
    {
        isMoving = true;

        Vector3 screen1StartPos = Screen1.transform.position;
        Vector3 screen2StartPos = Screen2.transform.position;
        Vector3 screen3StartPos = Screen3.transform.position;
        Vector3 screen4StartPos = Screen4.transform.position;


        Vector3 screen1EndPos = screen1StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen2EndPos = screen2StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen3EndPos = screen3StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);
        Vector3 screen4EndPos = screen4StartPos + new Vector3((currentScreen - targetScreen) * resolutionsetting, 0, 0);

        currentScreen = targetScreen;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            Screen1.transform.position = Vector3.Lerp(screen1StartPos, screen1EndPos, t);
            Screen2.transform.position = Vector3.Lerp(screen2StartPos, screen2EndPos, t);
            Screen3.transform.position = Vector3.Lerp(screen3StartPos, screen3EndPos, t);
            Screen4.transform.position = Vector3.Lerp(screen4StartPos, screen4EndPos, t);

            yield return null;
        }

        Screen1.transform.position = screen1EndPos;
        Screen2.transform.position = screen2EndPos;
        Screen3.transform.position = screen3EndPos;
        Screen4.transform.position = screen4EndPos;



        isMoving = false;
    }
}
