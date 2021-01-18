using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* First person camera */
public class CameraController : MonoBehaviour
{

    private Transform playerTransform;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float speed = 2.5f;
    private float rotationBound = 60.0f;

    void Start()
    {
        playerTransform = transform.parent;
    }

    void Update()
    {
        yaw += speed * Input.GetAxis("Mouse X"); ;
        pitch -= speed * Input.GetAxis("Mouse Y");

        if (Mathf.Abs(pitch) > rotationBound)
        {
            pitch = rotationBound * Mathf.Sign(pitch);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        playerTransform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
    }
}
