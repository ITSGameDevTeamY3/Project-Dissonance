using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class MenuAudioSettings : MonoBehaviour
{
    EventInstance SFXVolumeEvent;
    EventInstance playSelectSound;
    EventInstance backSelectSound;
    EventInstance startGameSound;

    Bus Music;
    Bus SFX;
    Bus Master;
    private float MusicVolume = 0.5f;
    private float SFXVolume = 0.5f;
    private float MasterVolume = 1f;

    void Awake()
    {
        Music = RuntimeManager.GetBus("bus:/Master/Music");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        Master = RuntimeManager.GetBus("bus:/Master");
        SFXVolumeEvent = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/Nav");
        playSelectSound = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/Select");
        backSelectSound = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/Back");
        startGameSound = RuntimeManager.CreateInstance("event:/Master/SFX_Events/UI/startGame");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Music.setVolume(MusicVolume);
        SFX.setVolume(SFXVolume);
        Master.setVolume(MasterVolume);
    }

    public void MasterVolumeLevel(float master)
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
    public void Select()
    {

        PLAYBACK_STATE PbState;
        playSelectSound.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            playSelectSound.start();
        }
    }

    public void BackSelect()
    {

        PLAYBACK_STATE PbState;
        backSelectSound.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            backSelectSound.start();
        }
    }

    public void StartGame()
    {

        PLAYBACK_STATE PbState;
        startGameSound.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            startGameSound.start();
        }
    }
}
