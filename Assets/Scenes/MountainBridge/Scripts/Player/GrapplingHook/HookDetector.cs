using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour {

    public GameObject Player;

    private void Start()
    {
        GetComponent<LineRenderer>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hookable"))
        {
            Player.GetComponent<GrapplingHook>().IsHooked = true;
            Player.GetComponent<GrapplingHook>().HookedObj = other.gameObject;
        }
    }
}
