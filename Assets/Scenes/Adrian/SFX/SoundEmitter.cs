using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class SoundEmitter : MonoBehaviour {
  
    EventInstance soundSFX;

	// Use this for initialization
	void Start () {
        soundSFX = RuntimeManager.CreateInstance("event:/Master/SFX_Events/Foley/Wind");
	}
	
	// Update is called once per frame
	void Update () {
        RuntimeManager.AttachInstanceToGameObject(soundSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
	}

    void OnTriggerEnter(Collider other)
    {
        soundSFX.start();
    }

    void OnTriggerExit(Collider other)
    {
        soundSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
