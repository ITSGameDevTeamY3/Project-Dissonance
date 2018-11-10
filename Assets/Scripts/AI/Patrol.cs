using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.AI;

// This code has been seen in the Unity User Manual, I am using it to test enemy patrol functionality.
// "U:" In front of a comment means that the comment came from the Unity User Manual.

// TODO: Get the enemy to pause briefly at each patrol point.
public class Patrol : MonoBehaviour
{
    // You can assign gameobjects to this array of positions in the Unity Inspector.
    public Transform[] points;

    // This will keep track of which point the enemy is currently heading towards.
    private int destPoint = 0;

    // The agent that this script will be assigned to.
    private NavMeshAgent agent;

    // How long the enemy waits before proceeding to the next point.
    public float restTime;   

    // Instantiate time manager so that we can access the countdown method.
    TimeManager tm = new TimeManager();

    void Start()
    {
        // The agent is automatically assigned when you add this script as a component to it.
        agent = GetComponent<NavMeshAgent>();

        // U: Disabling auto-braking allows for continuous movement
        //    between points (ie, the agent doesn't slow down as it
        //    approaches a destination point).
        agent.autoBraking = false;
      
        GotoNextPoint();
    }    

    void Update()
    {       
        ChooseNextPointAndMove(agent);      
    }

    void ChooseNextPointAndMove(NavMeshAgent agent)
    {
        // U: Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.isStopped = true;

            if (tm.TimeCount(restTime))
            {
                agent.isStopped = false;
                GotoNextPoint();
            }
        }
    }

    void GotoNextPoint()
    {
        // U: Returns if no points have been set up
        if (points.Length == 0) return;

        // U: Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // U: Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }
}