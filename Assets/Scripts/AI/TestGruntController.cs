using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestGruntController : MonoBehaviour
{
    // We can assign the camera, the agent and the halt time in the Unity Inspector window.

    // This camera is used to determine where the user has clicked on-screen.
    public Camera cam;

    // Instantiate the agent variable so that we can tell it where to go.
    public NavMeshAgent agent;

    // This variable will keep track of the object that you clicked.
    public RaycastHit hit;

    public float haltTime;

    TimeManager tm = new TimeManager();
   
    void Update()
    {

        // https://unity3d.com/learn/tutorials/topics/navigation/basics?playlist=17105
        // Some of the basic code for this functionality was seen in Brackeys' "Unity NavMesh Tutorial - Basics" video.
        // I'm just using it as a test for the grunt's AI.

        if(agent.isStopped == true)
        {
            Investigate();
        }

        // If you click the mouse on-screen. This will be taken out soon, it's only for testing purposes.
        if (Input.GetMouseButtonDown(0))
        {
            // Send out a ray to the position where you clicked.
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);            

            // This if will determine whether or not you clicked an object.
            if (Physics.Raycast(ray, out hit))
            {
                Halt();                                         
            }
        }
    }

    void Halt()
    {
        // The enemy stops in his tracks upon hearing a noise.
        agent.isStopped = true;      

        // I'd really like to get him to face the direction of the disturbance here. I still have to work it out.

        // We'll hopefully have something like this appear in-game soon.
        print("What was that?");
    }

    void Investigate()
    {   
        // Time count keeps track of the seconds that have passed.
        //timeCount += Time.deltaTime;

        // If the enemy has been halted for the specified halt time...
        if (tm.TimeCount(haltTime))
        {                        
            // The agent can now move again.
            agent.isStopped = false;            

            // If you clicked an object. The agent should head towards its' position.
            // TODO: I should work out how to get the enemy to stop a few feet in front of where they're headed.
            // https://docs.unity3d.com/Manual/DirectionDistanceFromOneObjectToAnother.html

            // The agent moves toward the source of the disturbance.
            agent.SetDestination(hit.point);
        }
    }
}
