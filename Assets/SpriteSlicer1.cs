using UnityEngine;

public class SpriteSlicer1 : MonoBehaviour
{
    public Sprite originalSprite;  // The original full sprite to crop
    private GameObject leftHalfObject;
    private GameObject rightHalfObject;


    private Sprite CropSprite(Sprite original, Rect cropRect)
    {
        // Calculate the pixels per unit based on the original sprite
        float pixelsPerUnit = original.pixelsPerUnit;

        // Create a new sprite with the specified crop rectangle
        Sprite croppedSprite = Sprite.Create(original.texture, cropRect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        return croppedSprite;
    }
    public void SliceSprite(int ID)
    {
        // Automatically find the child objects by name
        leftHalfObject = transform.Find("leftHalfPrefab")?.gameObject;
        rightHalfObject = transform.Find("rightHalfPrefab")?.gameObject;

        // Check if both objects were found
        if (leftHalfObject == null || rightHalfObject == null)
        {
            Debug.LogError("Both 'leftHalfPrefab' and 'rightHalfPrefab' must be children of this GameObject.");
        }
        Sprite spriteToSlice = Resources.Load<Sprite>($"sprite_{ID}");
        // Ensure the GameObjects have SpriteRenderers
        if (leftHalfObject.GetComponent<SpriteRenderer>() == null || rightHalfObject.GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError("Both GameObjects must have a SpriteRenderer component attached.");
            return;
        }

        // Crop the sprite into left and right halves
        Sprite leftHalf = CropSprite(spriteToSlice, new Rect(spriteToSlice.rect.width / 2, 0, spriteToSlice.rect.width / 4, spriteToSlice.rect.height));
        Sprite rightHalf = CropSprite(spriteToSlice, new Rect(spriteToSlice.rect.width / 4, 0, spriteToSlice.rect.width / 4, spriteToSlice.rect.height));

        // Assign cropped sprites to the GameObjects
        leftHalfObject.GetComponent<SpriteRenderer>().sprite = leftHalf;
        rightHalfObject.GetComponent<SpriteRenderer>().sprite = rightHalf;
    }

}
