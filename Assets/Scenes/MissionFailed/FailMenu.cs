using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour {
	
    // Start() and Update() have been removed, feel free to re-add them if need be.

    public void Retry()
    {
        Debug.Log("Return to main menu for now.");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
