using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MapCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image previewImage;
    public TMP_Text nameText;
    public GameObject highlight;
    public Image backgroundImage;

    private MapGroup data;
    private MapSelectionUIManager manager;
    private bool isRandom = false;

    public void Init(MapGroup mapGroup, MapSelectionUIManager mgr)
    {
        data = mapGroup;
        manager = mgr;

        nameText.text = data.groupName;
        previewImage.sprite = data.previewImage;
        backgroundImage.color = data.themeColor;

        SetSelected(false);
    }

    public void InitRandom(MapSelectionUIManager mgr)
    {
        manager = mgr;
        isRandom = true;

        nameText.text = "Random";
        previewImage.sprite = manager.randomPreviewSprite;
        SetSelected(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager != null)
        {
            if (isRandom)
            {
                manager.SelectRandom(this);
                manager.OnRandomSelected(); // 🔥 thêm
            }
            else
            {
                manager.SelectMap(this, data);
                manager.OnMapSelected(data); // 🔥 thêm
            }
        }
    }


    public void SetSelected(bool value)
    {
        if (highlight != null)
            highlight.SetActive(value);

        transform.localScale = value ? Vector3.one * 1.1f : Vector3.one;
    }
}