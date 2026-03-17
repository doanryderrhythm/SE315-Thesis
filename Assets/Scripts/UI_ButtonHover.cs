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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_scale = transform.localScale;
        original_rotation = transform.localRotation;
        target_scale = transform.localScale;
        target_rotation = transform.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        target_scale = original_scale * scale_rate;

        rotation_rate = flip ? rotation_rate : -rotation_rate;
        flip = !flip;
        target_rotation = original_rotation * Quaternion.Euler(0, 0, rotation_rate);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target_scale = original_scale;
        target_rotation = original_rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, target_scale, Time.deltaTime * speed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * speed);
    }
}
