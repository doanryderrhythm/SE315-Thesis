using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Current Match")]
    public int currentMapIndex = 0;

    [Header("Players")]
    public List<PlayerMatchData> players = new List<PlayerMatchData>();

    private GameObject currentMap;

    public List<MapData> selectedMaps;

    void Awake()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Lấy danh sách map từ GameSettings (host đã set trước)
        selectedMaps = GameSettings.selectedMaps;

        if (selectedMaps == null || selectedMaps.Count == 0)
        {
            Debug.LogError("No maps selected!");
            return;
        }

        CreateMockPlayers(); //thanh
        if (currentMap == null)
        {
            LoadCurrentMap();
        }
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

        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        UnityEngine.SceneManagement.SceneManager
            .LoadScene("RoundResultScene");
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

    public void LoadNextRound()
    {
        currentMapIndex++;

        UnityEngine.SceneManagement.SceneManager
            .LoadScene("GameScene");
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

    void CreateMockPlayers()
    {
        players.Clear();

        players.Add(new PlayerMatchData
        {
            playerName = "sunflower",
            totalScore = 500,
            gainedScore = 200,
            totalKills = 12,
            gainedKills = 5,
            totalDeaths = 9,
            gainedDeaths = 6,
            rank = 1,
            isLocalPlayer = true
        });

        players.Add(new PlayerMatchData
        {
            playerName = "ghost",
            totalScore = 450,
            gainedScore = 100,
            totalKills = 10,
            gainedKills = 2,
            totalDeaths = 7,
            gainedDeaths = 1,
            rank = 2,
            isLocalPlayer = false
        });

        players.Add(new PlayerMatchData
        {
            playerName = "blaze",
            totalScore = 350,
            gainedScore = 50,
            totalKills = 6,
            gainedKills = 1,
            totalDeaths = 11,
            gainedDeaths = 2,
            rank = 3,
            isLocalPlayer = false
        });
    }
}