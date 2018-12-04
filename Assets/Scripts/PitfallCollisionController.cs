using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitfallCollisionController : MonoBehaviour
{
    //we can place this on the player later, but for now, lets get down and dirty bois
    public Transform StartPosition;


	// Use this for initialization
	void Start ()
    {
        //StartPosition = GetComponent<PitfallCollisionController>().StartPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // This is a built in Unity method for handling collision.
    private void OnCollisionEnter(Collision collision)
    {
        print("Falling!");

        // When the player collides with this object, we want the scene to be reset.
        if (collision.gameObject.tag == "Player")
        {
            print("Fell!");
            collision.transform.position = StartPosition.position;

            //collision.transform.position = new Vector3(32, 67, 57);
        }
    }
}
