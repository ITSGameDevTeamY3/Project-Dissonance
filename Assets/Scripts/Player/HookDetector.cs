using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour {

    public GameObject Player;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hookable"))
        {
            Player.GetComponent<GrapplingHook>().IsHooked = true;
            Player.GetComponent<GrapplingHook>().HookedObj = other.gameObject;
        }
    }
}
