using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 0.1f;
    public float smoothTime = 0.3f;

    private Vector3 startPosition;
    private Vector3 velocity;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Vector3 targetPosition = new Vector3(
            startPosition.x + (offset.x) * offsetMultiplier,
            startPosition.y + (offset.y) * offsetMultiplier,
            startPosition.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}