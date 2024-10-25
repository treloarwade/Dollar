using UnityEngine;

public class DollarCollisionParticles : MonoBehaviour
{
    public ParticleSystem metalDollar1;

    // Assuming the object has a PolygonCollider2D and is marked as a trigger
    private void OnCollisionEnter2D()
    {
        metalDollar1.Play();
    }
}
