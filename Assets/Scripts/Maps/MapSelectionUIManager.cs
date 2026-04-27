using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class MapSelectionUIManager : MonoBehaviour
{
    public TMP_Dropdown mapTypeDropdown;
    public TMP_InputField mapCountInput;
    public MapDatabase database;

    public void OnStartClicked()
    {
        int selectedIndex = mapTypeDropdown.value;

        bool isRandom = selectedIndex == mapTypeDropdown.options.Count - 1;

        if (isRandom)
        {
            int count;
            if (!int.TryParse(mapCountInput.text, out count))
                count = 1;

            SelectRandom(count);
        }
        else
        {
            SelectByType((MapType)selectedIndex);
        }

        Debug.Log("Maps selected: " + GameSettings.selectedMaps.Count);

        SceneManager.LoadScene("GameScene");
    }
    void SelectByType(MapType type)
    {
        var maps = database.maps
            .Where(m => m.mapType == type)
            .ToList();

        Shuffle(maps);

        GameSettings.selectedMaps = maps;
    }

    void SelectRandom(int count)
    {
        var maps = new List<MapData>(database.maps);

        Shuffle(maps);

        GameSettings.selectedMaps = maps.Take(count).ToList();
    }

    void Shuffle(List<MapData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}