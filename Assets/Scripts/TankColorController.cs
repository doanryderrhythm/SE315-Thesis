using UnityEngine;
using UnityEngine.UI;

public class TankColorController : MonoBehaviour
{
    [Header("Tank Renderers")]
    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRenderer;

    [Header("UI Sliders")]
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;

    [Header("UI Preview (Optional)")]
    public Image colorPreviewBox;

    void Start()
    {
        // Set sliders to default (1 is white/full color)
        rSlider.value = 1f;
        gSlider.value = 1f;
        bSlider.value = 1f;

        // Add "Listeners" so the color updates the moment you slide
        rSlider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        gSlider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        bSlider.onValueChanged.AddListener(delegate { OnSliderChanged(); });

        // Initial color call
        OnSliderChanged();
    }

    public void OnSliderChanged()
    {
        // Create the new color from slider values (R, G, B)
        Color pickedColor = new Color(rSlider.value, gSlider.value, bSlider.value);

        // Apply it to the Tank Base
        if (baseRenderer != null)
            baseRenderer.color = pickedColor;

        // Apply it to the Turret
        if (turretRenderer != null)
            turretRenderer.color = pickedColor;

        // Update the UI preview box if you have one
        if (colorPreviewBox != null)
            colorPreviewBox.color = pickedColor;
    }
}