using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private bool initialized = false;
    [Header("Current Match")]
    public int currentMapIndex = 0;

    [Header("Players")]
    public List<PlayerMatchData> players = new List<PlayerMatchData>();

    [Header("Data")]
    public MapRegistry mapRegistry;

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

        if (PhotonNetwork.IsMasterClient)
        {
            selectedMaps = GameSettings.selectedMaps;
        }
        else
        {
            selectedMaps = LoadMapsFromRoomProperties();
        }

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
    List<MapData> LoadMapsFromRoomProperties()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (!props.ContainsKey("selectedMaps"))
        {
            Debug.LogError("No map data found in room properties!");
            return null;
        }

        string serialized = (string)props["selectedMaps"];
        string[] mapNames = serialized.Split(',');

        var result = new List<MapData>();
        foreach (var name in mapNames)
        {
            MapData map = mapRegistry.GetByName(name);
            if (map != null)
                result.Add(map);
            else
                Debug.LogWarning($"Map not found in registry: {name}");
        }

        return result;
    }

    void LoadCurrentMap()
    {
        MapData mapData = selectedMaps[currentMapIndex];

        currentMap = Instantiate(mapData.mapPrefab);
    }

    // ========================
    // ROUND FLOW
    // ========================
    public void EndCurrentRound()
    {
        Debug.Log("Round finished");

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        bool isLastRound = currentMapIndex >= selectedMaps.Count - 1;

        if (isLastRound)
        {
            PhotonNetwork.LoadLevel("MatchResultScene");
        }
        else
        {
            PhotonNetwork.LoadLevel("RoundResultScene");
        }
    }

    public void LoadNextRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        currentMapIndex++;

        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new ExitGames.Client.Photon.Hashtable { { "currentMapIndex", currentMapIndex } }
        );

        if (NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();

        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnGameplaySceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;
    }

    void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;

        if (!PhotonNetwork.IsMasterClient)
        {
            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            if (props.ContainsKey("currentMapIndex"))
                currentMapIndex = (int)props["currentMapIndex"];
        }

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