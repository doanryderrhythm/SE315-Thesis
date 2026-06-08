using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool initialized = false;
    [Header("Current Match")]
    public int currentMapIndex = 0;

    [Header("Players")]
    public List<PlayerMatchData> players = new List<PlayerMatchData>();

    private GameObject currentMap;

    public List<MapData> selectedMaps;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (initialized)
            return;

        initialized = true;

        selectedMaps = GameSettings.selectedMaps;

        if (selectedMaps == null || selectedMaps.Count == 0)
        {
            Debug.LogError("No maps selected!");
            return;
        }

        CreateMockPlayers();

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

        bool isLastRound = currentMapIndex >= selectedMaps.Count - 1;

        if (isLastRound)
        {
            SceneManager.LoadScene("MatchResultScene");
        }
        else
        {
            SceneManager.LoadScene("RoundResultScene");
        }
    }

    public void LoadNextRound()
    {
        currentMapIndex++;

        SceneManager.sceneLoaded += OnGameplaySceneLoaded;

        SceneManager.LoadScene("GameScene");
    }

    void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene")
            return;

        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;

        LoadCurrentMap();
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
            rank = 2,
            previousRank = 3,
            totalScore = 500,
            gainedScore = 200,
            totalKills = 12,
            gainedKills = 5,
            totalDeaths = 9,
            gainedDeaths = 6,
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

        // players.Add(new PlayerMatchData
        // {
        //     playerName = "blaze",
        //     totalScore = 350,
        //     gainedScore = 50,
        //     totalKills = 6,
        //     gainedKills = 1,
        //     totalDeaths = 11,
        //     gainedDeaths = 2,
        //     rank = 3,
        //     isLocalPlayer = false
        // });
    }
}