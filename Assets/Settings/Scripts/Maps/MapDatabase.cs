using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "Game/Map Database")]
public class MapDatabase : ScriptableObject
{
    public List<MapData> maps;
}