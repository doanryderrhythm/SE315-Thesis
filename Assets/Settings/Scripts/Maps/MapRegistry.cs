using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapRegistry", menuName = "Game/Map Registry")]
public class MapRegistry : ScriptableObject
{
    public List<MapData> allMaps;

    public MapData GetByName(string name)
    {
        return allMaps.Find(m => m.mapName == name);
    }
}
