using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ClickCounter : MonoBehaviour
{
    public float clickCount = 0; // Variable to keep track of the number of clicks
    public DiscordController controller;
    public GameObject fire;
    public GameObject goldenfire;
    public int dollartype = 100;
    public bool clickenabled = true;
    public bool onfire = false;
    public bool dollarburned = false;


    public SteamInventoryManager inventoryManager;
    public void OnMouseDown()
    {
        Dollarclick();
    }
    private void Dollarclick()
    {
        if (!clickenabled)
        {
            return;
        }
        if (dollarburned)
        {
            return;
        }
        switch (dollartype)
        {
            case 100:
                controller.OnDollarClicked(1);
                break;
            case 102:
                controller.OnDollarClicked(100);
                break;
            case 103:
                controller.OnDollarClicked(1000);
                break;
            case 104:
                controller.OnDollarClicked(10000);
                break;
            case 105:
                controller.OnDollarClicked(100000);
                break;
            case 106:
                controller.OnDollarClicked(1000000);
                break;
            case 108:
                controller.OnFrutigerClicked(1);
                break;
            case 111:
                controller.OnNotADollarClicked();
                break;
            case 122:
                controller.OnDiamondDollarClicked(1);
                break;
            case 123:
                controller.OnDiamondDollarClicked(1);
                break;
            case 141:
                controller.OnIceDollarClicked(1);
                break;
            default:
                controller.OnDollarClicked(1);
                break;
        }
    }
    public void SetDollarType(int inputdollartype)
    {
        dollartype = inputdollartype;
        Debug.Log(dollartype + " dollartype " + clickenabled + " clickenabled");
        if (dollartype >= 500 && dollartype < 600)
        {
            return;
        }
        else if (dollartype != 116 && dollartype != 126 && dollartype != 136 && dollartype != 1003)
        {
            clickenabled = true;
        }
        else
        {
            clickenabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Fire"))
        {


            BurstIntoFlames();
        }
        else if (collider.CompareTag("GoldenFire"))
        {
            BurstIntoFlames2();
        }
    }
    public ParticleSystem fire1;
    public ParticleSystem fire2;
    public ParticleSystem fire3;
    public ParticleSystem fire4;
    public Toggle deletetoggle;
    public bool deletetoggleon;
    void Start()
    {
        // Initialize the boolean value based on the initial state of the toggle
        if (deletetoggle != null)
        {
            deletetoggleon = deletetoggle.isOn; // Set toggleValue to true if toggle is checked
            deletetoggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void OnDestroy()
    {
        // Remove listener when the object is destroyed
        if (deletetoggle != null)
        {
            deletetoggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    // Method called when the toggle state changes
    private void OnToggleChanged(bool isOn)
    {
        deletetoggleon = isOn; // Update toggleValue based on the toggle's state
        Debug.Log($"Toggle value changed to: {deletetoggleon}");
    }

    // Method to get the current value of the boolean
    public bool GetToggleValue()
    {
        return deletetoggleon;
    }

    private void BurstIntoFlames()
    {
        if (!clickenabled)
        {
            return;
        }
        if (dollartype == 101 || dollartype == 110 || dollartype == 117 || dollartype == 118 || dollartype == 119 || dollartype == 120 || dollartype == 121 || dollartype == 122 || dollartype == 123)
        {
            return;
        }
        if (!spriteRenderer.enabled)
        {
            return;
        }
        if (dollartype == 103)
        {
            StartCoroutine(CatchFire2(fire));
        }
        else if (dollartype == 104 || dollartype == 106)
        {
            StartCoroutine(CatchFire3(fire));
        }
        else
        {
            StartCoroutine(CatchFire(fire));
        }
        dollarburned = true;
    }
    private void BurstIntoFlames2()
    {
        if (!clickenabled)
        {
            return;
        }
        if (dollartype == 101 || dollartype == 110 || dollartype == 117 || dollartype == 118 || dollartype == 119 || dollartype == 120 || dollartype == 121 || dollartype == 122 || dollartype == 123)
        {
            return;
        }
        if (!spriteRenderer.enabled)
        {
            return;
        }
        if (dollartype == 103)
        {
            StartCoroutine(CatchFire2(goldenfire));
        }
        else if (dollartype == 104 || dollartype == 106)
        {
            StartCoroutine(CatchFire3(goldenfire));
        }
        else
        {
            StartCoroutine(CatchFire(goldenfire));
        }
        dollarburned = true;
    }
    private IEnumerator CatchFire3(GameObject firecollision)
    {
        onfire = true;

        if (firecollision.transform.position.x > 0)
        {
            fire2.Play();
            fire3.Play();

            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(2.3f, -1.5f, fire2.transform.position.z), new Vector3(1.15f, -1.5f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(2.3f, -1.5f, fire3.transform.position.z), new Vector3(-1.15f, -1.5f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 3f));

        }
        else
        {
            fire3.Play();
            fire2.Play();
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(-2.3f, -1.5f, fire3.transform.position.z), new Vector3(-1.15f, -1.5f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(-2.3f, -1.5f, fire2.transform.position.z), new Vector3(1.15f, -1.5f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 3f));
        }


        yield return new WaitForSeconds(1f);
        StartCoroutine(TransitionColor());
        fire4.Play();
        fire1.Play();
        StartCoroutine(MoveAndScaleFire(fire4, new Vector3(fire3.transform.position.x, fire3.transform.position.y, fire3.transform.position.z), new Vector3(-1.15f, -0.10f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 2f));
        StartCoroutine(MoveAndScaleFire(fire1, new Vector3(fire2.transform.position.x, fire2.transform.position.y, fire2.transform.position.z), new Vector3(1.15f, -0.10f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(3.5f, 1.1f, 0f), 2f));
        if (deletetoggleon)
        {
            inventoryManager.ConsumeItem();
        }
        yield return new WaitForSeconds(2f);
        Smoke2.Play();

        yield return new WaitForSeconds(3f);
        Smoke3.Play();
        fire3.Stop();
        fire2.Stop();
        yield return new WaitForSeconds(1.5f);
        fire1.Stop();
        fire4.Stop();
        Smoke2.Stop();
        yield return new WaitForSeconds(7f);
        Smoke3.Stop();
        spriteRenderer.enabled = false;
        spriteRenderer.color = Color.white;
        inventoryManager.StopParticles();
        onfire = false;

        yield return null;
    }
    private IEnumerator CatchFire2(GameObject firecollision)
    {
        onfire = true;

        if (firecollision.transform.position.x > 0)
        {
            fire2.Play();
            fire3.Play();

            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(2.1f, -1.5f, fire2.transform.position.z), new Vector3(1.15f, -1.5f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(2.1f, -1.5f, fire3.transform.position.z), new Vector3(-1.15f, -1.5f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 3f));

        }
        else
        {
            fire3.Play();
            fire2.Play();
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(-2.1f, -1.5f, fire3.transform.position.z), new Vector3(-1.15f, -1.5f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(-2.1f, -1.5f, fire2.transform.position.z), new Vector3(1.15f, -1.5f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 3f));
        }


        yield return new WaitForSeconds(1f);
        StartCoroutine(TransitionColor());
        fire4.Play();
        fire1.Play();
        StartCoroutine(MoveAndScaleFire(fire4, new Vector3(fire3.transform.position.x, fire3.transform.position.y, fire3.transform.position.z), new Vector3(-1.15f, -0.10f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 2f));
        StartCoroutine(MoveAndScaleFire(fire1, new Vector3(fire2.transform.position.x, fire2.transform.position.y, fire2.transform.position.z), new Vector3(1.15f, -0.10f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.7f, 1.1f, 0f), 2f));
        if (deletetoggleon)
        {
            inventoryManager.ConsumeItem();
        }
        yield return new WaitForSeconds(2f);
        Smoke2.Play();

        yield return new WaitForSeconds(3f);
        Smoke3.Play();
        fire3.Stop();
        fire2.Stop();
        yield return new WaitForSeconds(1.5f);
        fire1.Stop();
        fire4.Stop();
        Smoke2.Stop();
        yield return new WaitForSeconds(7f);
        Smoke3.Stop();
        spriteRenderer.enabled = false;
        spriteRenderer.color = Color.white;
        inventoryManager.StopParticles();
        onfire = false;

        yield return null;
    }
    private IEnumerator CatchFire(GameObject firecollision)
    {
        onfire = true;

        if (firecollision.transform.position.x > 0)
        {
            fire2.Play();
            fire3.Play();

            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(2.1f, -0.86f, fire2.transform.position.z), new Vector3(1.15f, -0.86f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(2.1f, -0.86f, fire3.transform.position.z), new Vector3(-1.15f, -0.86f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 3f));

        }
        else
        {
            fire3.Play();
            fire2.Play();
            StartCoroutine(MoveAndScaleFire(fire3, new Vector3(-2.1f, -0.86f, fire3.transform.position.z), new Vector3(-1.15f, -0.86f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 3f));
            StartCoroutine(MoveAndScaleFire(fire2, new Vector3(-2.1f, -0.86f, fire2.transform.position.z), new Vector3(1.15f, -0.86f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 3f));
        }


        yield return new WaitForSeconds(1f);
        StartCoroutine(TransitionColor());
        fire4.Play();
        fire1.Play();
        StartCoroutine(MoveAndScaleFire(fire4, new Vector3(fire3.transform.position.x, fire3.transform.position.y, fire3.transform.position.z), new Vector3(-1.15f, -0.10f, fire3.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 2f));
        StartCoroutine(MoveAndScaleFire(fire1, new Vector3(fire2.transform.position.x, fire2.transform.position.y, fire2.transform.position.z), new Vector3(1.15f, -0.10f, fire2.transform.position.z), new Vector3(0.5f, 0f, 0f), new Vector3(2.5f, 0f, 0f), 2f));
        if (deletetoggleon)
        {
            inventoryManager.ConsumeItem();
        }
        yield return new WaitForSeconds(2f);
        Smoke2.Play();

        yield return new WaitForSeconds(3f);
        Smoke1.Play();
        fire3.Stop();
        fire2.Stop();
        yield return new WaitForSeconds(1.5f);
        fire1.Stop();
        fire4.Stop();
        Smoke2.Stop();
        yield return new WaitForSeconds(5f);
        Smoke1.Stop();
        spriteRenderer.enabled = false;
        spriteRenderer.color = Color.white;
        inventoryManager.StopParticles();
        onfire = false;

        yield return null;
    }
    public float transitionDuration = 5.0f; // Duration of the transition in seconds
    public SpriteRenderer spriteRenderer;
    public ParticleSystem Smoke1;
    public ParticleSystem Smoke2;
    public ParticleSystem Smoke3;

    public ParticleSystem sparkle;
    public ParticleSystem confetti;
    IEnumerator TransitionColor()
    {
        Color startColor = Color.white;
        Color endColor = Color.black;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = endColor; // Ensure the final color is set
    }
    private IEnumerator MoveAndScaleFire(ParticleSystem ps, Vector3 startPosition, Vector3 endPosition, Vector3 startScale, Vector3 endScale, float duration)
    {
        Vector3 initialPos = startPosition;
        Vector3 finalPos = endPosition;
        Vector3 initialScale = startScale;
        Vector3 finalScale = endScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Update elapsed time
            elapsed += Time.deltaTime;

            // Calculate the fraction of time
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp position
            ps.transform.position = Vector3.Lerp(initialPos, finalPos, t);

            // Lerp scale
            var shape = ps.shape;
            shape.scale = Vector3.Lerp(initialScale, finalScale, t);

            yield return null;
        }

        // Ensure final position and scale are set
        ps.transform.position = finalPos;
        var finalShape = ps.shape;
        finalShape.scale = finalScale;
    }
}
