using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Current Match")]
    private int currentMapIndex = 0;
    private GameObject currentMap;

    private List<MapData> selectedMaps;

    void Start()
    {
        // Lấy danh sách map từ GameSettings (host đã set trước)
        selectedMaps = GameSettings.selectedMaps;

        if (selectedMaps == null || selectedMaps.Count == 0)
        {
            Debug.LogError("No maps selected!");
            return;
        }

        currentMapIndex = 0;

        LoadCurrentMap();
    }

    void Update()
    {
        // Restart game (debug)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartMatch();
        }

        // TEMP: next map (debug)
        if (Input.GetKeyDown(KeyCode.N))
        {
            EndCurrentRound();
        }
    }

    // ========================
    // LOAD MAP
    // ========================
    void LoadCurrentMap()
    {
        if (currentMapIndex >= selectedMaps.Count)
        {
            OnMatchFinished();
            return;
        }

        MapData mapData = selectedMaps[currentMapIndex];

        currentMap = Instantiate(mapData.mapPrefab);

        Debug.Log($"Round {currentMapIndex + 1} / {selectedMaps.Count}");
        Debug.Log("Loaded map: " + mapData.mapName);
    }

    // ========================
    // ROUND FLOW
    // ========================
    public void EndCurrentRound()
    {
        Debug.Log("Round finished");

        // TODO:
        // - show round result UI
        // - calculate score

        // destroy current map
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        currentMapIndex++;

        LoadCurrentMap();
    }

    // ========================
    // MATCH END
    // ========================
    void OnMatchFinished()
    {
        Debug.Log("=== MATCH FINISHED ===");

        // TODO:
        // - show final result UI
        // - return to lobby or restart
    }

    // ========================
    // RESTART
    // ========================
    void RestartMatch()
    {
        Debug.Log("Restart Match");

        currentMapIndex = 0;

        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        LoadCurrentMap();
    }
}