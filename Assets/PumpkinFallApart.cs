using UnityEngine;

public class PumpkinFallApart2D : MonoBehaviour
{
    public float explosionVelocityThreshold = 5f; // Minimum relative velocity to trigger explosion
    public GameObject explosionPrefab; // The prefab for explosion effect (particles or fragments)
    public float explosionForce = 10f; // Maximum velocity applied to the pumpkin pieces

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check the magnitude of the relative velocity at the moment of collision
        if (collision.relativeVelocity.magnitude >= explosionVelocityThreshold)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Instantiate the explosion effect at the pumpkin's position (optional)
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Notify all child objects that they should "explode"
        foreach (Transform child in transform)
        {
            PumpkinPiece piece = child.GetComponent<PumpkinPiece>();
            if (piece != null)
            {
                piece.ExplodePiece(explosionForce); // Trigger each piece's individual explosion behavior
            }
        }

        // Destroy the parent object (pumpkin)
        gameObject.SetActive(false);
    }
}
