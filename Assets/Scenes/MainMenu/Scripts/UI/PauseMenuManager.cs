using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PauseMenuManager : MonoBehaviour {

    public static bool PausedGame = false;
    public GameObject UI;
    public GameObject PersistentObjects;

	void Update ()
    {
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
        Destroy(PersistentObjects);
        SceneManager.LoadScene("MainMenu");
    }
}
