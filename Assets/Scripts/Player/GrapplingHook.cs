using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject Hook;
    public GameObject HookHolder;

    public float HookTravelSpeed;
    public float PlayerTravelSpeed;

    public static bool IsFired;
    public bool IsHooked;
    public GameObject HookedObj;

    public float MaxDistance;
    private float CurrentDistance;

    private bool IsGrounded;

    void Update()
    {
        // Firing the hook
        if (Input.GetMouseButtonDown(1) && IsFired == false)
            IsFired = true;

        if (IsFired)
        {
            LineRenderer rope = Hook.GetComponent<LineRenderer>();

            // From hook holder to hook
            rope.positionCount = 2;
            rope.SetPosition(0, HookHolder.transform.position);
            rope.SetPosition(1, Hook.transform.position);
        }

        if (IsFired && !IsHooked)
        {
            Hook.transform.Translate(Vector3.forward * Time.deltaTime * HookTravelSpeed);
            CurrentDistance = Vector3.Distance(transform.position, Hook.transform.position);

            if (CurrentDistance >= MaxDistance)
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
                if (!IsGrounded)
                {                    
                    this.GetComponent<Rigidbody>().AddForce(0, 1f, 0, ForceMode.Impulse); // Spring
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

    IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }

    void ReturnHook()
    {
        Hook.transform.rotation = HookHolder.transform.rotation;
        Hook.transform.position = HookHolder.transform.position;
        IsFired = false;
        IsHooked = false;

        // Remove rope
        LineRenderer rope = Hook.GetComponent<LineRenderer>();
        rope.positionCount = 0;
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, -1);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }
}
