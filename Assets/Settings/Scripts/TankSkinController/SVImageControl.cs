using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Image pickerImage;
    [SerializeField] private TankColorController CC;

    private RawImage svImage;
    private RectTransform rectTransform, pickerTransform;

    private float xNorm, yNorm;

    private void Awake()
    {
        svImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        pickerTransform = pickerImage.GetComponent<RectTransform>();

        xNorm = 0f;
        yNorm = 1f;

        pickerTransform.localPosition = new Vector2(-(rectTransform.rect.width * 0.5f), rectTransform.rect.height * 0.5f);
    }

    void UpdatePicker(PointerEventData eventData)
    {
        /* Screen space Overlay
        Vector3 pos = rectTransform.InverseTransformPoint(eventData.position); */

        // Screen Space - Camera
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        Vector3 pos = new Vector3(localPoint.x, localPoint.y, 0);

        float deltaX = rectTransform.rect.width * 0.5f;
        float deltaY = rectTransform.rect.height * 0.5f;

        pos.x = Mathf.Clamp(pos.x, -deltaX, deltaX);
        pos.y = Mathf.Clamp(pos.y, -deltaY, deltaY);

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        xNorm = x / rectTransform.rect.width;
        yNorm = y / rectTransform.rect.height;

        pickerTransform.localPosition = pos;
        UpdatePickerColor();
    }
    public void UpdatePickerColor()
    {
        pickerImage.color = Color.HSVToRGB(CC.currentHue, xNorm, yNorm);
        CC.SetSV(xNorm, yNorm);
    }
    public void SetPickerFromHex()
    {
        xNorm = CC.currentSat;
        yNorm = CC.currentVal;

        float deltaX = rectTransform.rect.width * 0.5f;
        float deltaY = rectTransform.rect.height * 0.5f;

        float posX = xNorm*deltaX*2 - deltaX;
        float posY = yNorm*deltaY*2 - deltaY;

        pickerTransform.localPosition = new Vector3(posX, posY, 0);
        UpdatePickerColor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePicker(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdatePicker(eventData);
    }
}
