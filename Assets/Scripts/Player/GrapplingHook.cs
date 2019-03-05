using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.CrossPlatformInput;
using Cursor = HoloToolkit.Unity.InputModule.Cursor;

public class GrapplingHook : MonoBehaviour
{
    public GameObject Hook;
    public GameObject HookHolder;
    public Transform Cursor;

    private LineRenderer _cable;

    public float HookTravelSpeed;
    public float PlayerTravelSpeed;
    public float SpringForce = 1f;

    public static bool IsFired;
    public bool IsHooked;
    public GameObject HookedObj;

    public float MaxDistance;
    private float _currentDistance;

    private bool _isGrounded;

    private Vector3 _previousPosition;

    private void Start()
    {
        _cable = Hook.GetComponent<LineRenderer>();
        _cable.positionCount = 1;
        _cable.SetPosition(0, HookHolder.transform.position);
    }

    private void Update()
    {
        // Firing the hook
        if (Input.GetMouseButtonDown(1) ||
            CrossPlatformInputManager.GetButtonDown("XBOX_RIGHT_BUMPER") && IsFired == false)
        {
            // Look at the mouse cursor
            _previousPosition = Cursor.position;
            Hook.transform.LookAt(_previousPosition);
            IsFired = true;
        }

        if (IsFired)
        {
            // From hook holder to hook
            _cable.positionCount = 2;
            _cable.SetPosition(0, HookHolder.transform.position);
            _cable.SetPosition(1, Hook.transform.position);
        }

        if (IsFired && !IsHooked)
        {
            Hook.transform.Translate(Vector3.forward * Time.deltaTime * HookTravelSpeed);
            _currentDistance = Vector3.Distance(transform.position, Hook.transform.position);

            if (_currentDistance >= MaxDistance)
                ReturnHook();
        }

        // Move player towards hook
        if (IsHooked && IsFired)
        {
            Hook.transform.parent = HookedObj.transform;

            transform.position = Vector3.MoveTowards(transform.position, Hook.transform.position, Time.deltaTime * PlayerTravelSpeed);
            float distanceToHook = Vector3.Distance(transform.position, Hook.transform.position);

            this.GetComponent<Rigidbody>().useGravity = false;

            if (distanceToHook < 1)
            {
                if (!_isGrounded)
                {                    
                    this.GetComponent<Rigidbody>().AddForce(0, SpringForce, 0, ForceMode.Impulse); // Spring
                }

                StartCoroutine("Climb");
            }
        }
        else
        {
            Hook.transform.parent = HookHolder.transform;
            this.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }

    private void ReturnHook()
    {
        Hook.transform.rotation = HookHolder.transform.rotation;
        Hook.transform.position = HookHolder.transform.position;
        IsFired = false;
        IsHooked = false;

        // Remove rope
        LineRenderer rope = Hook.GetComponent<LineRenderer>();
        rope.positionCount = 0;
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, -1);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }
}
