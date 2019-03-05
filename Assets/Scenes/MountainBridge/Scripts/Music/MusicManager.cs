using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    public enum Conditions { Normal, Alerted, Spotted, SightJack }
    public Conditions Condition = Conditions.Normal;

    [EventRef]
    private string _music = "event:/Master/Music_Events/AdaptiveMusic";
    private EventInstance _musicEvent;

    void Start()
    {
        _musicEvent = RuntimeManager.CreateInstance(_music);
        _musicEvent.start();
    }

    void Update()
    {
        CheckMusicCondition();

        if (PauseMenuManager.PausedGame)
            _musicEvent.setPaused(true);
        else
            _musicEvent.setPaused(false);
    }

    private void CheckMusicCondition()
    {
        switch (Condition)
        {
            case Conditions.Normal:
                _musicEvent.setParameterValue("DrumsAlert", 0);
                _musicEvent.setParameterValue("SpottedDrumLoop", 0);
                _musicEvent.setParameterValue("MusicWhenSightJacking", 0);
                break;
            case Conditions.Alerted:
                _musicEvent.setParameterValue("DrumsAlert", 1);
                _musicEvent.setParameterValue("SpottedDrumLoop", 0);
                _musicEvent.setParameterValue("MusicWhenSightJacking", 0);
                break;
            case Conditions.Spotted:
                _musicEvent.setParameterValue("DrumsAlert", 0);
                _musicEvent.setParameterValue("SpottedDrumLoop", 1);
                _musicEvent.setParameterValue("MusicWhenSightJacking", 0);
                break;
            case Conditions.SightJack:
                _musicEvent.setParameterValue("MusicWhenSightJacking", 1);
                break;
            default:
                _musicEvent.setParameterValue("DrumsAlert", 0);
                _musicEvent.setParameterValue("SpottedDrumLoop", 0);
                _musicEvent.setParameterValue("MusicWhenSightJacking", 0);
                break;
        }
    }
}
