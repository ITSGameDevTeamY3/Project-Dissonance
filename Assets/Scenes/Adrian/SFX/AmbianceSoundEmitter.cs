using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AmbianceSoundEmitter : MonoBehaviour {

    [FMODUnity.EventRef]

    public string inputSound;

    // Use this for initialization
    void Start () {
        RuntimeManager.PlayOneShot(inputSound, transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
