using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;

    private Rigidbody2D rb;
    private float moveInput;
    private float rotateInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomAngle);
    }

    void Update()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveInput = -1f;
        }

        rotateInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            rotateInput = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateInput = -1f;
        }
    }

    void FixedUpdate()
    {
        rb.MoveRotation(rb.rotation + rotateInput * rotateSpeed * Time.fixedDeltaTime);

        Vector2 movement = transform.right * moveInput * moveSpeed;
        rb.linearVelocity = movement;
    }
}