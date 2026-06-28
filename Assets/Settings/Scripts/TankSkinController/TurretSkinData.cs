using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTankTurretLibrary", menuName = "Tanks/Turret Library")]
public class TurretSkinLibrary : ScriptableObject
{
    public List<TurretSkin> allTurrets;
}

[System.Serializable]
public struct TurretSkin
{
    public string turretName;
    public Sprite turretSprite;
}