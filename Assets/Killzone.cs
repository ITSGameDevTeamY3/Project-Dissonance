using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour
{
    public GameObject MainCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainCamera.GetComponent<CinemachineBrain>().enabled = false;
            SceneManager.LoadScene("MissionFailed");
        }
    }
}
