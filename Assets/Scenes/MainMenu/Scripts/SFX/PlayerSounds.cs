using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {

    //bool playerISMoving;
    public float PlayerWalkingSpeed;
    [EventRef]
    public string FootStepEvent = "event:/Master/SFX_Events/FootSteps/PlayerNormalFootSteps";  

	private void Start ()
	{
        InvokeRepeating("PlayFootSteps", 0, PlayerWalkingSpeed * Time.deltaTime);
	}
	
	private void Update ()
	{
	    if (Input.GetAxis("Vertical") >= 0.9f || Input.GetAxis("Horizontal") >= 0.9f)
	    {
            RuntimeManager.PlayOneShot(FootStepEvent, transform.position);
	    }
    }
}