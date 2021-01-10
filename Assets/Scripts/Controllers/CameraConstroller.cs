using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstroller : MonoBehaviour
{

    private Transform playerTransform;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float speed = 2.5f;
    private float rotationBound = 45.0f;

    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform.parent;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxis("Mouse Y");

        yaw += speed * horizontalInput;
        pitch -= speed * verticalInput;

        if (Mathf.Abs(pitch) > rotationBound)
        {
            pitch = rotationBound * Mathf.Sign(pitch);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        playerTransform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
    }
}
