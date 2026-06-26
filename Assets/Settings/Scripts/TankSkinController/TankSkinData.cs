using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTankSkinLibrary", menuName = "Tanks/Skin Library")]
public class TankSkinLibrary : ScriptableObject
{
    public List<TankSkin> allSkins;
}

[System.Serializable]
public struct TankSkin
{
    public string skinName;
    public Sprite baseSprite;
    public Sprite turretSprite;
    public RuntimeAnimatorController baseAnimator;
}