using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;
    [SerializeField] private Slider textSlider;
    [SerializeField] private Image colorPreview;
    [SerializeField] private Image topBar;
    [SerializeField] private Button defaultButton;
    [SerializeField] private Text[] texts;  // Array to hold multiple Text components
    public bool colorchanged = false;

    private void Start()
    {
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
        textSlider.onValueChanged.AddListener(UpdateTextColor);
        defaultButton.onClick.AddListener(SetDefaultColor);
    }

    private void UpdateColor(float _)
    {
        float r = redSlider.value;
        float g = greenSlider.value;
        float b = blueSlider.value;
        colorPreview.color = new Color(r, g, b, 183 / 255f);
        topBar.color = new Color(r, g, b, 183 / 255f);
        colorchanged = true;
    }
    private void UpdateTextColor(float _)
    {
        float w = textSlider.value;
        Color textColor = new Color(w, w, w, 1f);

        foreach (Text text in texts)
        {
            text.color = textColor;
        }
    }

    private void SetDefaultColor()
    {
        redSlider.value = 113 / 255f;
        greenSlider.value = 113 / 255f;
        blueSlider.value = 255 / 255f;
        textSlider.value = 50 / 255f;
        colorchanged = false;
    }

    private void OnDestroy()
    {
        redSlider.onValueChanged.RemoveListener(UpdateColor);
        greenSlider.onValueChanged.RemoveListener(UpdateColor);
        blueSlider.onValueChanged.RemoveListener(UpdateColor);
        defaultButton.onClick.RemoveListener(SetDefaultColor);
    }
}
