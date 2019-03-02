using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class MusicManager : MonoBehaviour {

    public enum Conditions { Normal, Alerted, Spotted }

    public Conditions condition = Conditions.Normal;

    [EventRef]
    public string music = "event:/Master/Music_Events/WorldMusic";
    EventInstance musicEvnt;

    // Use this for initialization
    void Start() {
        musicEvnt = RuntimeManager.CreateInstance(music);
        
        musicEvnt.start();
    } 
	
    private void CheckMusicCondition()
    {
        switch (condition)
        {
            case Conditions.Normal:
                musicEvnt.setParameterValue("alerted", 1f);
                musicEvnt.setParameterValue("Caught", 0f);
                musicEvnt.setParameterValue("Normal", 0f);
                break;
            case Conditions.Alerted:
                musicEvnt.setParameterValue("alerted", 0f);
                musicEvnt.setParameterValue("Caught", 0f);
                musicEvnt.setParameterValue("Normal", 1f);
                break;
            case Conditions.Spotted:
                musicEvnt.setParameterValue("alerted", 0f);
                musicEvnt.setParameterValue("Cuaght", 1f);
                musicEvnt.setParameterValue("Normal", 0f);
                break;
            default:
                musicEvnt.setParameterValue("Normal", 1f);
                break;
        }      
    }

    // Update is called once per frame
    void Update () {
        CheckMusicCondition();
	}
}
