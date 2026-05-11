using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TankColorController : MonoBehaviour
{
    [Header("Tank Renderers")]
    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRenderer;

    [Header("HSV Configuration")]
    public float currentHue;
    public float currentSat;
    public float currentVal;

    [SerializeField] private RawImage hueImage, satValImage, outputImage;
    [SerializeField] private Slider hueSlider;
    [SerializeField] private TMP_InputField hexInputField;
    [SerializeField] private SVImageControl svControl;

    private Texture2D hueTexture, svTexture, outputTexture;

    private void Start()
    {
        CreateHueImage();

        CreateSVImage();

        CreateOutputImage();

        UpdateOutputImage();
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i <hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));
        }

        hueTexture.Apply();
        currentHue = 1f;
        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y,Color.HSVToRGB(
                    currentHue,
                    (float)x / svTexture.width,
                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0f;
        currentVal = 1f;

        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i <outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    private void UpdateOutputImage()
    {
        float preferredVal = Mathf.Max(currentVal, 0.25f);
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, preferredVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();
        string hexString = ColorUtility.ToHtmlStringRGB(currentColor);
        hexInputField.text = "#" + hexString;

        PlayerPrefs.SetFloat("ColorR", currentColor.r);
        PlayerPrefs.SetFloat("ColorG", currentColor.g);
        PlayerPrefs.SetFloat("ColorB", currentColor.b);

        if (baseRenderer != null)
        {
            baseRenderer.color = currentColor;
        }

        if (turretRenderer != null)
        {
            turretRenderer.color = currentColor;
        }
    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        float hue = currentHue;
        
        for(int y = 0; y <svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                    hue,
                    (float)x / svTexture.width,
                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        if(svControl != null)
        {
            svControl.UpdatePickerColor();
        }

        UpdateOutputImage();
    }
    public void OnHueChanged(float hue)
    {
        currentHue = hue;
        UpdateSVImage();
    }

    public void OnTextInput()
    {
        string hex = hexInputField.text;
        if (hex.Length < 6) return;

        if (!hex.StartsWith("#"))
        {
            hex = "#" + hex;
        }

        Color color;

        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);

            hueSlider.SetValueWithoutNotify(currentHue);

            if (svControl != null)
                svControl.SetPickerFromHex();

            UpdateSVImage();
        }
    }
}