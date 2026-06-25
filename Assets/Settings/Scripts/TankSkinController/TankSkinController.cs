using UnityEngine;
using System.Collections.Generic;

public class TankSkinController : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer turretRenderer;

    [SerializeField] private TankSkinLibrary skinLibrary;

    private int currentSkinIndex = 0;

    void Awake() 
    {
        currentSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
        ApplySkin(currentSkinIndex);
    }

    public void NextSkin()
    {
        if (skinLibrary.allSkins.Count == 0) return;

        currentSkinIndex++;
        if (currentSkinIndex >= skinLibrary.allSkins.Count) currentSkinIndex = 0;
        ApplySkin(currentSkinIndex);
    }

    public void PreviousSkin()
    {
        if (skinLibrary.allSkins.Count == 0) return;

        currentSkinIndex--;
        if (currentSkinIndex < 0) currentSkinIndex = skinLibrary.allSkins.Count - 1;
        ApplySkin(currentSkinIndex);
    }

    private void ApplySkin(int index)
    {
        var skin = skinLibrary.allSkins[index];

        if (baseRenderer) baseRenderer.sprite = skin.baseSprite;
        if (turretRenderer) turretRenderer.sprite = skin.turretSprite;

        PlayerPrefs.SetInt("SelectedSkinIndex", index);
    }
}