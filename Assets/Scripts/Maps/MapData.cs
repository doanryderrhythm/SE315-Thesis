using UnityEngine;

[CreateAssetMenu(fileName = "NewMap", menuName = "Game/Map")]
public class MapData : ScriptableObject
{
    public string mapName;
    public MapType mapType;
    public GameObject mapPrefab;
}