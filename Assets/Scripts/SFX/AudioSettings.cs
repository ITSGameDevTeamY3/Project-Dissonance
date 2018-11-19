using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class AudioSettings : MonoBehaviour {

    EventInstance SFXVolumeEvent;

    Bus Music;
    Bus SFX;
    Bus Master;
    float MusicVolume = 0.5f;
    float SFXVolume = 0.5f;
    float MasterVolume = 1f;

    void Awake ()
    {
        Music = RuntimeManager.GetBus("bus:/Master/Music");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        Master = RuntimeManager.GetBus("bus:/Master");
        SFXVolumeEvent = RuntimeManager.CreateInstance("event:/SFX_Events/UI/Nav");
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Music.setVolume(MusicVolume);
        SFX.setVolume(SFXVolume);
        Master.setVolume(MasterVolume);
    }

    public void MasterVolumeLevel (float master)
    {
        MasterVolume = master;
    }

    public void MusicVolumeLevel(float music)
    {
        MusicVolume = music;
    }
    public void SFXVolumeLevel(float sfx)
    {
        SFXVolume = sfx;

        PLAYBACK_STATE PbState;
        SFXVolumeEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeEvent.start();
        }
    }
}
