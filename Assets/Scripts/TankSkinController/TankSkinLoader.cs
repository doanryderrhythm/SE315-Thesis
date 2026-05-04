using UnityEngine;

public class TankSkinLoader : MonoBehaviour
{
    [Header("Tank Renderers")]
    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRenderer;

    public TankSkinLibrary skinLibrary;
    private TankColorController colorController;
    private TankSkinController skinController;

    void Awake()
    {
        skinController = GetComponent<TankSkinController>();

        int skinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);

        float r = PlayerPrefs.GetFloat("ColorR", 1f);
        float g = PlayerPrefs.GetFloat("ColorG", 1f);
        float b = PlayerPrefs.GetFloat("ColorB", 1f);
        Color savedColor = new Color(r, g, b);
        
        var skin = skinLibrary.allSkins[skinIndex];
        if (baseRenderer != null)
        {
            baseRenderer.sprite = skin.baseSprite;
            baseRenderer.color = savedColor;
        }
        if (turretRenderer != null)
        {
            turretRenderer.sprite = skin.turretSprite;
            turretRenderer.color = savedColor;
        }
    }
}