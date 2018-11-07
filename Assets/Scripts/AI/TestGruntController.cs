using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestGruntController : MonoBehaviour {

    // This camera is used to determine where the user has clicked on-screen.
    public Camera cam;

    // Instantiate the agent variable so that we can tell it where to go.
    public NavMeshAgent agent;

    // Update is called once per frame
	void Update () {

        // https://unity3d.com/learn/tutorials/topics/navigation/basics?playlist=17105
        // This code was seen in Brackeys' "Unity NavMesh Tutorial - Basics" video.
        // I'm just using it as a test for the grunt's AI.

        // If you click the mouse on-screen.
        if(Input.GetMouseButtonDown(0))
        {
            // Send out a ray to the position where you clicked.
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // This variable will keep track of the object that you clicked.
            RaycastHit hit;

            // This if will determine whether or not you clicked an object.
            if(Physics.Raycast(ray, out hit))
            {
                // If you clicked an object. The agent should head towards its' position.
                agent.SetDestination(hit.point);
            }
        }
    }
}
