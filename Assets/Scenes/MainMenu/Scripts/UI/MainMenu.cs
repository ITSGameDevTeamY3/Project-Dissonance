using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public EventSystem EventSystem;
    public GameObject OptionsMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene("SightJackTest");
    }

    public void Options()
    {
        gameObject.SetActive(false);
        OptionsMenu.SetActive(true);
        EventSystem.SetSelectedGameObject(OptionsMenu.transform.GetChild(0).gameObject);        
    }

    public void BackToMainMenu()
    {
        OptionsMenu.SetActive(false);
        gameObject.SetActive(true);
        EventSystem.SetSelectedGameObject(transform.GetChild(0).gameObject);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
