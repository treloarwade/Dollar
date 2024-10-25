using UnityEngine;
using UnityEngine.UI; // For accessing UI components like Toggle

public class ToggleRigidbody2D : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public DraggableObject2 draggableObject; // Reference to the DraggableObject1 script
    private Vector3 startingPosition = new Vector3(0, -0.2f, 10);
    private Quaternion startingRotation = Quaternion.Euler(0, 0, 0); // No rotation
    public UpgradeMenu upgradeMenu;
    public bool fixeddollar = true;
    public Toggle controlToggle; // Reference to the UI Toggle
    private BoxCollider2D boxCollider; // Reference to BoxCollider2D

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>(); // Get the BoxCollider2D component
        // Set the initial position and rotation of the object
        transform.position = startingPosition;
        transform.rotation = startingRotation;

        // Ensure the toggle's state matches the Rigidbody's state at the start
        if (controlToggle != null)
        {
            controlToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        fixeddollar = true;
        draggableObject.enabled = false;

        rb2d.gravityScale = 0;
    }

    // Called when the Toggle is switched
    public void OnToggleValueChanged(bool isOn)
    {
        ToggleRigidbodyAndDraggable(isOn);
    }

    public void ToggleRigidbodyAndDraggable(bool isOn)
    {
        if (rb2d != null && draggableObject != null)
        {
            if (!isOn)
            {
                // Turn on Rigidbody2D and DraggableObject1
                rb2d.gravityScale = 1;
                draggableObject.defaultGravity = 1;
                draggableObject.enabled = true;
                boxCollider.isTrigger = false; // Disable trigger when Rigidbody2D is simulated
                fixeddollar = false;
            }
            else
            {
                // Turn off Rigidbody2D and DraggableObject1, and reset position/rotation
                rb2d.gravityScale = 0;
                draggableObject.defaultGravity = 0;
                draggableObject.enabled = false;
                transform.position = startingPosition;
                transform.rotation = startingRotation;
                boxCollider.isTrigger = true; // Enable trigger when Rigidbody2D is not simulated
                fixeddollar = true;
                rb2d.velocity = Vector2.zero;
            }
        }
        upgradeMenu.BordersOnOff();
    }

    void OnDestroy()
    {
        // Remove listener when the script is destroyed to avoid errors
        if (controlToggle != null)
        {
            controlToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}