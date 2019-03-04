using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {

    bool playerISMoving;
    public float playerWalkingSpeed;
    [EventRef]
    public string footStepEvent = "event:/Master/SFX_Events/FootSteps/PlayerNormalFootSteps";

	// Use this for initialization
	void Start () {
        InvokeRepeating("PlayFootSteps",0, playerWalkingSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Vertical") >= 0.1f || Input.GetAxis("Horizontal") >= 0.1f || Input.GetAxis("Vertical") <= 0.1f || Input.GetAxis("Horizontal") <= 0.1f)
        {
            playerISMoving = true;
        }
        else if (Input.GetAxis("Vertical") == 0 || Input.GetAxis("Horizontal") == 0)
        {
            playerISMoving = false;
        }
	}

    void PlayFootSteps()
    {
        if (playerISMoving == true)
        {
            RuntimeManager.PlayOneShot(footStepEvent);
        }
    }

    void StopFootSteps()
    {
        playerISMoving = false;
    }
}
