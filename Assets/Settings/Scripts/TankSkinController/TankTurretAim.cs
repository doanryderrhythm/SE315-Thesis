using UnityEngine;
using UnityEngine.InputSystem;

public class TurretAim : MonoBehaviour
{
    public float rotationSpeed = 5f;
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = mousePos - transform.position;

        // Calculate the angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create a rotation object (Quaternion)
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // Smoothly rotate toward that target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}