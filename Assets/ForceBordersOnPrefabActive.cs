using UnityEngine;

public class ForceBordersOnPrefabActive : MonoBehaviour
{
    private UpgradeMenu upgradeMenu;

    void Awake()
    {
        // Find the UpgradeMenu component automatically in the scene
        upgradeMenu = FindObjectOfType<UpgradeMenu>();

        // Check if it was found successfully
        if (upgradeMenu == null)
        {
            Debug.LogError("UpgradeMenu not found in the scene!");
            return;
        }
    }

    void OnEnable()
    {
        // Ensure UpgradeMenu is available before accessing it
        if (upgradeMenu != null)
        {
            upgradeMenu.forceborderson = true;
        }
    }

    void OnDisable()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.forceborderson = false;
            upgradeMenu.BordersOnOff();
        }
    }
}
