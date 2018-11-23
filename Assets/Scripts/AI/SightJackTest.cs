using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightJackTest : MonoBehaviour {

    public Camera GameCam;
    private Camera POV; 

	void Start ()
    {
        POV = GetComponent<Camera>();
        POV.enabled = false;
	}
	
	void Update ()
    {
		if(Input.GetKeyUp(KeyCode.P))
        {
            GameCam.enabled = !GameCam.enabled;
            POV.enabled = !POV.enabled;
        }
	}
}
