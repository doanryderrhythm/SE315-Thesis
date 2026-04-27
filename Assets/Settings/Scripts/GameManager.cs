using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentMapIndex = 0;
    private GameObject currentMap;

    void Start()
    {
        // Check if maps were selected
        if (GameSettings.selectedMaps == null || GameSettings.selectedMaps.Count == 0)
        {
            Debug.LogError("No maps selected!");
            return;
        }

        LoadNextMap();
    }

    void Update()
    {
        // Restart game (your existing feature)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // TEMP: press N to go to next map (for testing)
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextMap();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextMap()
    {
        // Destroy current map
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        // Check if finished all maps
        if (currentMapIndex >= GameSettings.selectedMaps.Count)
        {
            Debug.Log("All maps finished!");
            return;
        }

        // Get next map
        var mapData = GameSettings.selectedMaps[currentMapIndex];

        // Spawn it
        currentMap = Instantiate(mapData.mapPrefab);

        Debug.Log("Loaded map: " + mapData.mapName);

        currentMapIndex++;
    }
}