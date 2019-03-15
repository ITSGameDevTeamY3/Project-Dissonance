using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public GameObject MainCamera;
    public GameObject Player;
    public ThirdPersonCharacter ThirdPersonScript;

    public int timeTaken, alerts;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        ThirdPersonScript = Player.GetComponent<ThirdPersonCharacter>();
    }

    void Update ()
    {
        if (ThirdPersonScript.Health <= 0) EndGame();
	}

    public void EndGame()
    {
        MainCamera.GetComponent<CinemachineBrain>().enabled = false;
        SceneManager.LoadScene("MissionFailed");
    }
}