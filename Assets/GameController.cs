using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class GameController : MonoBehaviour {

    public static GameController instance;
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

    //void Start () {

    //}

    // Update is called once per frame
    void Update () {
        if (ThirdPersonScript.Health <= 0) EndGame();
	}

    public void EndGame()
    {
        SceneManager.LoadScene("MissionFailed");

        // Stop music and fade might be good here.
    }
}