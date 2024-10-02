using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public ParticleSystem money;
    public ParticleSystem seasonal;
    public GameObject dollarsfromleftscreen1;
    public GameObject dollarsfromleftscreen2;
    public GameObject dollarsfromleftscreen3;
    public BezierMovementWithSpin bezierMovementWithSpin;

    private float cooldownTime = 4.5f; // Cooldown period in seconds
    private float nextAvailableTime = 0f; // Time when the function can be called again

    public void OnSeasonalDollarClicked()
    {
        seasonal.Play();
    }

    public void OnMenuDollarClicked()
    {
        // Check if the function is within the cooldown period
        if (Time.time < nextAvailableTime)
        {
            return; // Exit the function if it's still within cooldown
        }

        money.Play();

        // Array of dollar objects
        GameObject[] dollarObjects = { dollarsfromleftscreen1, dollarsfromleftscreen2, dollarsfromleftscreen3 };

        // Flag to determine if all X values are greater than 0
        bool allGreaterThanZero = true;

        foreach (GameObject dollar in dollarObjects)
        {
            if (dollar != null)
            {
                float xPosition = dollar.transform.position.x;

                // Check if X position is greater than 0
                if (xPosition <= 0)
                {
                    allGreaterThanZero = false;
                    break; // Exit early as soon as one fails the condition
                }
            }
        }

        if (allGreaterThanZero)
        {
            bezierMovementWithSpin.MoveNow();
        }

        // Update the next available time to be the current time plus the cooldown period
        nextAvailableTime = Time.time + cooldownTime;
    }
}
