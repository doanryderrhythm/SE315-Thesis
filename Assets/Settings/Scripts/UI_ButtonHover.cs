using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scale_rate = 1.5f;
    public float rotation_rate = 1.5f;
    public float speed = 10f;

    private Vector3 original_scale;
    private Vector3 target_scale;
    private Quaternion original_rotation;
    private Quaternion target_rotation;

    private static bool flip = false;

    void Awake()
    {
        original_scale = transform.localScale;
        original_rotation = transform.localRotation;
    }

    void OnEnable()
    {
        ResetButton();
    }

    void OnDisable()
    {
        ResetButton();
    }

    private void ResetButton()
    {
        transform.localScale = original_scale;
        transform.localRotation = original_rotation;

        target_scale = original_scale;
        target_rotation = original_rotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        target_scale = original_scale * scale_rate;

        float current_rotation = flip ? rotation_rate : -rotation_rate;
        flip = !flip;
        target_rotation = original_rotation * Quaternion.Euler(0, 0, current_rotation);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target_scale = original_scale;
        target_rotation = original_rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, target_scale, Time.unscaledDeltaTime * speed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.unscaledDeltaTime * speed);
    }
}
