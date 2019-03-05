using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PauseMenuManager : MonoBehaviour {

    public static bool PausedGame = false;
    public GameObject UI;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || CrossPlatformInputManager.GetButtonDown("CONTROLLER_RIGHT_MENU"))
        {
            if (PausedGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
	}

    public void Resume()
    {
        UI.SetActive(false);
        Time.timeScale = 1f;
        PausedGame = false;
    }

    private void Pause()
    {
        UI.SetActive(true);
        Time.timeScale = 0f;
        PausedGame = true;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Adrian Menu");
    }
}
