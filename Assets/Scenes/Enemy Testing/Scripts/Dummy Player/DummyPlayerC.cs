using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerC : MonoBehaviour {

    public int moveSpeed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        HandleMovement();
	}

    void HandleMovement()
    {
        // Up
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);

        }

        // Down
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
            
        }

        // Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }

        // Left
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0, 0, -moveSpeed * Time.deltaTime);
        }
    }
}
