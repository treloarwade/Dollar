using UnityEngine;
using System.Collections.Generic;

public class QualityToggle : MonoBehaviour
{

    // List of GameObjects to control based on quality
    public List<GameObject> gameObjects;
    public void SetVisible()
    {
        SetGameObjectsActive(true);
        Debug.Log("GameObjects set to visible");
    }

    // Function to hide all GameObjects
    public void SetHidden()
    {
        SetGameObjectsActive(false);
        Debug.Log("GameObjects set to hidden");
    }

    private void SetGameObjectsActive(bool isActive)
    {
        // Update visibility of GameObjects
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
            }
        }
    }
}
