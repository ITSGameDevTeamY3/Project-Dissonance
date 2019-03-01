using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerC : MonoBehaviour {

    public int Health;
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);

        }

        // Down
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
            
        }

        // Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }

        // Left
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(0, 0, -moveSpeed * Time.deltaTime);
        }
    }
}
