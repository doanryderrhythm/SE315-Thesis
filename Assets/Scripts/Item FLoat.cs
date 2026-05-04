using UnityEngine;

public class FloatItem : MonoBehaviour
{
    [SerializeField] float floatHeight = 0.25f;
    [SerializeField] float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}