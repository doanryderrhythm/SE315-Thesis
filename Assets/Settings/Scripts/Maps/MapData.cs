using UnityEngine;

[CreateAssetMenu(menuName = "Game/Map")]
public class MapData : ScriptableObject
{
    public string mapName;
    public Sprite previewImage;

    public GameObject mapPrefab;

    public MapType type; // Fire, Ice...
}