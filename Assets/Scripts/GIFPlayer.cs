using UnityEngine;
using System.IO;
using MG.GIF; // Ensure this namespace matches the library you're using

public class GIFPlayer : MonoBehaviour
{
    public string gifFilePath = "Assets/Resources/plink.gif";
    public float frameRate = 10f; // Number of frames per second

    private Texture2D[] frames;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = false;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is not assigned.");
            return;
        }

        LoadGIF();
    }

    void Update()
    {
        if (isPlaying && frames != null && frames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = Sprite.Create(frames[currentFrame], new Rect(0, 0, frames[currentFrame].width, frames[currentFrame].height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    void LoadGIF()
    {
        if (!File.Exists(gifFilePath))
        {
            Debug.LogError($"GIF file not found at {gifFilePath}");
            return;
        }

        byte[] data = File.ReadAllBytes(gifFilePath);

        using (var decoder = new Decoder(data))
        {
            var img = decoder.NextImage();
            var textureList = new System.Collections.Generic.List<Texture2D>();

            while (img != null)
            {
                Texture2D tex = img.CreateTexture();
                textureList.Add(tex);
                img = decoder.NextImage();
            }

            if (textureList.Count == 0)
            {
                Debug.LogError("No frames found in GIF.");
                return;
            }

            frames = textureList.ToArray();
        }
    }

    public void PlayGIF()
    {
        if (frames != null && frames.Length > 0)
        {
            isPlaying = true;
        }
        else
        {
            Debug.LogWarning("Cannot play GIF. No frames loaded.");
        }
    }

    public void StopGIF()
    {
        isPlaying = false;
        if (frames != null && frames.Length > 0)
        {
            currentFrame = 0;
            spriteRenderer.sprite = Sprite.Create(frames[currentFrame], new Rect(0, 0, frames[currentFrame].width, frames[currentFrame].height), new Vector2(0.5f, 0.5f));
        }
    }
}
