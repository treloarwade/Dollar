using System;
using UnityEngine;

public class SliceOnCollision1 : MonoBehaviour
{
    public GameObject slicedPrefab; // Assign the sliced object prefab in the Inspector
    public int ItemID;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (ItemID == 101 || ItemID == 110 || ItemID == 117 || ItemID == 118 || ItemID == 119 || ItemID == 120 || ItemID == 121 || ItemID == 122 || ItemID == 123)
        {
            return;
        }
        // Check if the collider should trigger slicing (e.g., tag the sword as "Sword")
        if (collider.gameObject.CompareTag("Sword"))
        {
            // Instantiate the sliced prefab at the current object's position and rotation
            GameObject instantiatedSlicedPrefab = Instantiate(slicedPrefab, transform.position, transform.rotation);

            // Get the SpriteSlicer component from the instantiated prefab
            SpriteSlicer1 spriteSlicer = instantiatedSlicedPrefab.GetComponent<SpriteSlicer1>();

            spriteSlicer.SliceSprite(ItemID);

            // Optionally, hide or deactivate the original dollar object
        }
    }
    public void SetItemID(int id)
    {
        ItemID = id;
    }
}
