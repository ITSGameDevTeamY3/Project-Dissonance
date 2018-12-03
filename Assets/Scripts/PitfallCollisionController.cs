using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallCollisionController : MonoBehaviour
{



	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // This is a built in Unity method for handling collision.
    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with this object, we want the scene to be reset.
    }
}
