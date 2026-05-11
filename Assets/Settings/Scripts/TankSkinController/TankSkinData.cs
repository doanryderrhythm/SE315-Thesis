using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTankSkinLibrary", menuName = "Tanks/Skin Library")]
public class TankSkinLibrary : ScriptableObject
{
    public List<TankSkin> allSkins;
}

[System.Serializable]
public class TankSkin
{
    public string skinName;
    public Sprite baseSprite;
    public Sprite turretSprite;
}