using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Map Group")]
public class MapGroup : ScriptableObject
{
    public string groupName;          // Jungle, Desert...
    public MapType type;

    public Sprite previewImage;       // ảnh đại diện (không phải từng map)

    public List<MapData> maps;        // DANH SÁCH map nhỏ

    public int MaxRounds => maps.Count;
}