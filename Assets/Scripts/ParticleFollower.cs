using UnityEngine;

public class ParticleFollower : MonoBehaviour
{
    public RectTransform uiObject; // The UI object to follow
    public Transform particleSystemTransform; // The particle system transform

    // UI to World conversion factors
    private const float uiWidth = 960f;
    private const float uiHeight = 580f;
    private const float worldWidth = 17.772f;
    private const float worldHeight = 10f;
    private float xFactor = worldWidth / uiWidth;
    private float yFactor = worldHeight / uiHeight;

    // Y-axis offset
    private const float yOffset = 0.3f;

    void Update()
    {
        // Get the UI object's position
        Vector2 uiPosition = uiObject.anchoredPosition;

        // Convert UI position to world position
        float worldX = uiPosition.x * xFactor;
        float worldY = uiPosition.y * yFactor + yOffset; // Apply Y offset here

        // Set the particle system's position
        particleSystemTransform.position = new Vector3(worldX, worldY, particleSystemTransform.position.z);
    }
}
