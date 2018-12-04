using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with this goal object,
        // The scene will either end or reset if the player has eliminated enough enemies.
        // If not enough enemies are eliminated, nothing happens.

        // When the player collides with this object, we want the scene to be reset.
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("Donnacha");
        }
    }
}
