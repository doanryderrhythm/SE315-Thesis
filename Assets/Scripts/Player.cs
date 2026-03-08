using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;     
    public float rotateSpeed = 120f; 

    void Update()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            move = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move = -1f;
        }

        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);

        float rotate = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            rotate = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotate = 1f;
        }

        transform.Rotate(Vector3.up * rotate * rotateSpeed * Time.deltaTime);
    }
}