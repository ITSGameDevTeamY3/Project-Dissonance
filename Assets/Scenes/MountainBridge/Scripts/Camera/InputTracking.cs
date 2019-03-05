using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityStandardAssets.CrossPlatformInput;

public class InputTracking : MonoBehaviour
{
	void Start ()
	{
	    UnityEngine.XR.InputTracking.disablePositionalTracking = true;
	}
	
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Tab) || CrossPlatformInputManager.GetButtonDown("CONTROLLER_LEFT_STICK_CLICK"))
	    {
	        UnityEngine.XR.InputTracking.Recenter();
	    }
	}
}
