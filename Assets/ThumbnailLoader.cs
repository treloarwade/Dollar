using System.IO;
using UnityEngine;
using UnityEngine.UI;  // Required for UI.Image

public class ThumbnailLoader : MonoBehaviour
{
    public Image imageComponent;  // Reference to the Image component where you want to display the thumbnail

    void Start()
    {
        // Define the thumbnail file path (same as before)
        string thumbnailFolderPath = Path.Combine(Application.persistentDataPath, "WorkshopThumbnailFolder").Replace("\\", "/");
        string thumbnailFilePath = null;

        // Check if the directory exists and find the thumbnail
        if (Directory.Exists(thumbnailFolderPath))
        {
            string[] thumbnailExtensions = { ".jpg", ".png", ".gif" };
            foreach (string ext in thumbnailExtensions)
            {
                string potentialPath = Path.Combine(thumbnailFolderPath, "thumbnail" + ext).Replace("\\", "/");
                if (File.Exists(potentialPath))
                {
                    thumbnailFilePath = potentialPath;
                    break;
                }
            }
        }

        // If a valid thumbnail is found, try to load and display it
        if (!string.IsNullOrEmpty(thumbnailFilePath))
        {
            // Load the image file into a Texture2D
            byte[] fileData = File.ReadAllBytes(thumbnailFilePath);
            Texture2D texture = new Texture2D(2, 2);  // Create a new texture to load the image into
            texture.LoadImage(fileData);  // Load the image data into the texture

            // Create a Sprite from the Texture2D
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Assign the Sprite to the Image component
            imageComponent.sprite = sprite;
            Debug.Log("Thumbnail loaded successfully and displayed!");
        }
        else
        {
            Debug.LogWarning("No valid thumbnail found at: " + thumbnailFolderPath);
        }
    }
}
