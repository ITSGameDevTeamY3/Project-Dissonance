using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityStandardAssets.CrossPlatformInput;

public class InputTracking : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    UnityEngine.XR.InputTracking.disablePositionalTracking = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Tab) || CrossPlatformInputManager.GetButtonDown("CONTROLLER_LEFT_STICK_CLICK"))
	    {
	        UnityEngine.XR.InputTracking.Recenter();
	    }
	}
}
