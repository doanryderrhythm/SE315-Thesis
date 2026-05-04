using UnityEngine;
using System.Collections.Generic;

public class TankSkinController : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer baseRenderer;
    public SpriteRenderer turretRenderer;

    public TankSkinLibrary skinLibrary;
    private int currentSkinIndex = 0;

    void Awake() 
    {
        currentSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
        ApplySkin(currentSkinIndex);
    }

    public void NextSkin()
    {
        currentSkinIndex = (currentSkinIndex + 1) % skinLibrary.allSkins.Count;
        ApplySkin(currentSkinIndex);
    }

    public void PreviousSkin()
    {
        currentSkinIndex--;
        if (currentSkinIndex < 0) currentSkinIndex = skinLibrary.allSkins.Count - 1;
        ApplySkin(currentSkinIndex);
    }

    public void ApplySkin(int index)
    {
        if (skinLibrary.allSkins.Count == 0) return;

        var skin = skinLibrary.allSkins[index];

        baseRenderer.sprite = skin.baseSprite;
        turretRenderer.sprite = skin.turretSprite;
        
        PlayerPrefs.SetInt("SelectedSkinIndex", index);
    }
}