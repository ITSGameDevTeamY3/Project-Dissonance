//using FMOD.Studio;
//using FMODUnity;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlaySelect : MonoBehaviour {

//    EventInstance playSelectSound;

//    void Awake()
//    {
//        playSelectSound = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/Select");
//    }

//    public void SelectSound()
//    {
//        PLAYBACK_STATE PbState;
//        playSelectSound.getPlaybackState(out PbState);
//        if (PbState != PLAYBACK_STATE.PLAYING)
//        {
//            playSelectSound.start();
//        }
//    }

//    // Use this for initialization
//    void Start () {
		
//	}
	
//	// Update is called once per frame
//	void Update () {
//        SelectSound();
//	}
//}
