using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls for a camera that we can move through the environment. Purely for development testing.
public class DevCameraController : MonoBehaviour
{
    public int moveSpeed;

    // Mouse Look Properties.
    float lookSpeedH = 2.0f;
    float lookSpeedV = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    Transform devCamPosition;
    bool cameraFixed = true;

    void Start()
    {
        devCamPosition = GetComponent<Transform>();
    }

    void Update()
    {
        // Fix and un-fix camera.
        if (Input.GetKeyUp(KeyCode.F)) cameraFixed = !cameraFixed;

        if (!cameraFixed)
        {
            MouseLook();
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        // Sprint (Hard-coded for now.)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 40;
        }

        // Forward
        if (Input.GetKey(KeyCode.W))
        {
            devCamPosition.position += devCamPosition.forward * moveSpeed * Time.deltaTime;
        }

        // Backward
        if (Input.GetKey(KeyCode.S))
        {
            devCamPosition.position -= devCamPosition.forward * moveSpeed * Time.deltaTime;
        }

        // Left
        if (Input.GetKey(KeyCode.A))
        {
            devCamPosition.position -= devCamPosition.right * moveSpeed * Time.deltaTime;
        }

        // Right
        if (Input.GetKey(KeyCode.D))
        {
            devCamPosition.position += devCamPosition.right * moveSpeed * Time.deltaTime;
        }

        // Upwards
        if (Input.GetKey(KeyCode.Space))
        {
            devCamPosition.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        }

        // Downwards
        if (Input.GetKey("left ctrl"))
        {
            devCamPosition.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
        }
    }

    // https://www.youtube.com/watch?v=lYIRm4QEqro
    void MouseLook()
    {
        yaw += lookSpeedH * Input.GetAxis("Mouse X");
        pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

        devCamPosition.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
