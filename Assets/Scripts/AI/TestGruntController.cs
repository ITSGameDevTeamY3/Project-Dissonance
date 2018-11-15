using Assets.Scripts.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TestGruntController : MonoBehaviour
{
    // We can assign the camera, the agent and the halt time in the Unity Inspector window.
    #region Variables
    // This camera is used to determine where the user has clicked on-screen.
    public Camera cam;

    // Instantiate the agent variable so that we can tell it where to go.
    public NavMeshAgent agent;
    public float stopDistance;

    // This variable will keep track of the object that you clicked.
    public RaycastHit hit;

    public float haltTime;
    private bool halted;

    // This variable stores where the agent was originally headed.
    private Vector3 originalDestination;

    TimeManager tm = new TimeManager();

    //  Rotation experimentation here.
    float directionOfDisturbance;
    float rotationSpeed = 10;

    bool isRotating;
    #endregion

    void Update()
    {
        // https://unity3d.com/learn/tutorials/topics/navigation/basics?playlist=17105
        // Some of the basic code for this functionality was seen in Brackeys' "Unity NavMesh Tutorial - Basics" video.
        // I'm just using it as a test for the grunt's AI.

        if (halted) Halt();
        #region Old code.
        //{
        // TODO: At the minute, I can either have it turn towards the click correctly (while still moving though!) or I can have it stop completely.
        // If I can get the agent to rotate even though they're stopped, we should be in business.

        //agent.isStopped = true;
        //agent.updateRotation = false;
        //RotateTowards(hit.transform);
        //}            
        #endregion

        if (DestinationReached() && !halted)
        {
            agent.SetDestination(originalDestination);

            agent.stoppingDistance = 0;
            agent.isStopped = false;
        }

        // If you click the mouse on-screen. This will be taken out soon, it's only for testing purposes.
        if (Input.GetMouseButtonDown(0))
        {
            // Send out a ray to the position where you clicked.
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // This if will determine whether or not you clicked an object.
            if (Physics.Raycast(ray, out hit))
            {
                halted = true;

                //agent.ResetPath();

                //StartCoroutine(RotateAgent());
            }
        }
    }

    void Halt()
    {
        // If the agent was already on their way to somewhere...
        if (agent.hasPath)
        {
            // ...Keep track of where the enemy was originally headed so that they can resume moving toward it later.
            originalDestination = agent.destination;

            // The enemy stops in his tracks upon hearing a noise.           
            agent.ResetPath();

            // We'll hopefully have something like this appear in-game soon.
            print("What was that?");

            // Set the specified stop distance so that the enemy stops in front of the disturbance.
            agent.stoppingDistance = stopDistance;
        }

        RotateTowards(hit.transform);

        // The enemy will then move in to investigate.
        if (tm.TimeCount(haltTime)) Investigate();
    }

    void Investigate()
    {
        // The agent can now move again.
        halted = false;
        agent.isStopped = false;

        // If you clicked an object. The agent should head towards its' position.
        // https://docs.unity3d.com/Manual/DirectionDistanceFromOneObjectToAnother.html                             

        print("Checking it out.");

        // The agent moves toward the source of the disturbance.
        //if (isRotating == true)
        //{
        agent.SetDestination(hit.point);
        //}
    }

    bool DestinationReached()
    {
        // https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html?childToView=746157#answer-746157 Answer #2 from here used as a reference. 

        // Check if we've reached the destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    // https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html
    bool RotateTowards(Transform target)
    {
        // Get the difference in position between the agent and the disturbance.
        Vector3 direction =
            (target.position - transform.position).normalized;

        // Get the rotation the agent needs to have in order to be facing the disturbance.
        Quaternion lookRotation = Quaternion.LookRotation(
            new Vector3(direction.x, 0, direction.z)); // We make y = 0 to flatten the position we will rotate to...
                                                       // ...this means that our agent will only rotate its Y position (As an enemy in a game would).      

        // We use the slerp method to get our enemy to rotate towards the direction of the disturbance.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed);

        if (transform.rotation == lookRotation) return true;

        else return false;
    }

    // Testing example seen on the web.
    IEnumerator RotateAgent()
    {
        isRotating = true;

        //RotateTowards(hit.transform);

        while (RotateTowards(hit.transform) == false)
        {
            print("Agent rotating: " + isRotating);

            yield return null;
        }

        isRotating = false;
        print("Agent rotating: " + isRotating);
    }
}
