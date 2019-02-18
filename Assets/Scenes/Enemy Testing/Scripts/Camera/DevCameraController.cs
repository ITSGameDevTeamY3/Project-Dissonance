using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls for a camera that we can move through the environment. Purely for development testing.
[RequireComponent(typeof(Light))]
public class DevCameraController : MonoBehaviour
{
    #region Dev Cam Properties.
    public int moveSpeed;

    // Mouse Look Properties.
    float lookSpeedH = 2.0f;
    float lookSpeedV = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // These properties are set automatically.
    Transform devCamPosition;
    Light devCamLight;
    bool cameraFixed = true;
    int normalSpeed;
    int sprintSpeed;
    #endregion

    void Start()
    {
        devCamPosition = GetComponent<Transform>();
        devCamLight = GetComponent<Light>();
        devCamLight.enabled = false;
        normalSpeed = moveSpeed;
        sprintSpeed = normalSpeed * 2;
    }

    void Update()
    {
        // Fix and un-fix camera.
        if (Input.GetKeyUp(KeyCode.V)) cameraFixed = !cameraFixed;

        // Toggle dev cam light on and off.
        if (Input.GetKeyUp(KeyCode.F)) devCamLight.enabled = !devCamLight.enabled;

        if (!cameraFixed)
        {
            MouseLook();
            HandleMovement();            
        }
    }

    void HandleMovement()
    {
        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = sprintSpeed;
        else moveSpeed = normalSpeed;

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
