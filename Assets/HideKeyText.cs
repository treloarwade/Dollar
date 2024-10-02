using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideKeyText : MonoBehaviour
{
    public GameObject blocker;
    public void HideFunction()
    {
        blocker.SetActive(true);
    }
    private IEnumerator HideAPIKey()
    {
        blocker.SetActive(true);
        yield return null;
    }
}
