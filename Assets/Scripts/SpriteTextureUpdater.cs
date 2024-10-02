using UnityEngine;

public class SpriteTextureUpdater : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material material; // The material that uses the Shader Graph shader

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (material == null)
        {
            Debug.LogError("Material is not assigned.");
        }
    }

    void Update()
    {
        // Update the material's texture to match the SpriteRenderer's sprite texture
        if (spriteRenderer != null && material != null)
        {
            material.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
    }
}
