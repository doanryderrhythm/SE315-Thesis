using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MapSelectionUIManager : MonoBehaviour
{
    [Header("UI - Preview")]
    public Image bigPreviewImage;
    public TMP_Text bigMapNameText;

    [Header("UI - Input")]
    public TMP_InputField roundInput;
    public TMP_Text warningText;
    public Button startButton;

    [Header("Cards")]
    public MapCardUI cardPrefab;
    public Transform cardParent;

    [Header("Data")]
    public List<MapGroup> allMapGroups;

    private MapGroup selectedGroup;
    private MapCardUI currentCard;

    public Sprite randomPreviewSprite;
    [SerializeField] private bool includeRandomCard = true;

    void Start()
    {
        GenerateCards();
        OnHoverMap(allMapGroups[0]);

        warningText.gameObject.SetActive(false);

        if (startButton != null)
            startButton.interactable = false;
    }

    void GenerateCards()
    {
        // tạo 4 biome
        foreach (var group in allMapGroups)
        {
            var card = Instantiate(cardPrefab, cardParent);
            card.Init(group, this);
        }
        // thêm random card
        if (includeRandomCard)
        {
            var randomCard = Instantiate(cardPrefab, cardParent);
            randomCard.InitRandom(this);
        }
    }

    public void SelectRandom(MapCardUI card)
    {
        if (currentCard != null)
            currentCard.SetSelected(false);

        currentCard = card;
        selectedGroup = null;

        currentCard.SetSelected(true);

        ValidateRounds();
    }

    // ========================
    // HOVER
    // ========================
    public void OnHoverMap(MapGroup data)
    {
        if (bigPreviewImage != null)
            bigPreviewImage.sprite = data.previewImage;

        if (bigMapNameText != null)
            bigMapNameText.text = data.groupName;
    }

    // ========================
    // SELECT
    // ========================
    public void SelectMap(MapCardUI card, MapGroup data)
    {
        if (currentCard != null)
            currentCard.SetSelected(false);

        currentCard = card;
        OnHoverMap(data);
        selectedGroup = data;

        currentCard.SetSelected(true);

        ValidateRounds();
    }

    // ========================
    // INPUT CHANGE
    // ========================
    public void OnRoundChanged()
    {
        ValidateRounds();
    }

    // ========================
    // VALIDATION
    // ========================
    void ValidateRounds()
    {
        int rounds;

        if (!int.TryParse(roundInput.text, out rounds) || rounds <= 0)
        {
            warningText.text = "Invalid number";
            warningText.gameObject.SetActive(true);
            SetStartButton(false);
            return;
        }

        // Random mode
        if (selectedGroup == null)
        {
            int max = allMapGroups.Max(g => g.MaxRounds);

            if (rounds > max)
            {
                warningText.text = "Max number selected";
                warningText.gameObject.SetActive(true);
                SetStartButton(false);
            }
            else
            {
                warningText.gameObject.SetActive(false);
                SetStartButton(true);
            }

            return;
        }

        // Normal mode
        if (rounds > selectedGroup.MaxRounds)
        {
            warningText.text = "Max number selected";
            warningText.gameObject.SetActive(true);
            SetStartButton(false);
        }
        else
        {
            warningText.gameObject.SetActive(false);
            SetStartButton(true);
        }
    }

    void SetStartButton(bool value)
    {
        if (startButton != null)
            startButton.interactable = value;
    }

    // ========================
    // START GAME
    // ========================
    public void OnStartGame()
{
    int rounds = int.Parse(roundInput.text);

    List<MapData> selectedMaps;

    if (selectedGroup != null)
    {
        // chọn biome cụ thể
        selectedMaps = selectedGroup.maps
            .OrderBy(x => Random.value)
            .Take(rounds)
            .ToList();
    }
    else
    {
        // RANDOM biome
        selectedMaps = new List<MapData>();

        for (int i = 0; i < rounds; i++)
        {
            var randomGroup = allMapGroups[Random.Range(0, allMapGroups.Count)];
            var randomMap = randomGroup.maps[Random.Range(0, randomGroup.maps.Count)];

            selectedMaps.Add(randomMap);
        }
    }

    // 🔥 truyền sang GameManager
    GameSettings.selectedMaps = selectedMaps;

    Debug.Log("=== START GAME ===");
    foreach (var map in selectedMaps)
    {
        Debug.Log(map.mapName);
    }

    // 👉 load gameplay scene (nếu có)
    // SceneManager.LoadScene("Gameplay");
}
}