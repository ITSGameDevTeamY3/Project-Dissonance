using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlay : MonoBehaviour {

    EventInstance playSelectSound;

    void PlaySound()
    {
        playSelectSound = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/Select");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
