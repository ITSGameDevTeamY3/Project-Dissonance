using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.AI;

// This code has been seen in the Unity User Manual, I am using it to test enemy patrol functionality.
// "U:" In front of a comment means that the comment came from the Unity User Manual.

// An enemy controller is needed first in order for this script to work.
[RequireComponent(typeof(EnemyController))]
public class Patrol : MonoBehaviour
{
    // You can assign gameobjects to this array of positions in the Unity Inspector.
    public Transform[] PatrolPoints;
    // How long the enemy waits before proceeding to the next point.
    public float RestTime;

    // This will keep track of which point the enemy is currently heading towards.
    private int destPoint = 0;
    // The agent that this script will be assigned to.
    private NavMeshAgent agent;

    // Instantiate time manager so that we can access the countdown method.
    TimeManager tm = new TimeManager();

    // This bool will determine whether or not the enemy is currently patrolling.
    private bool onPatrol;

    void Start()
    {
        // This script needs an enemy controller, an enemy controller needs an agent. In short, we're assured that we can acess the agent component here too.
        agent = GetComponent<NavMeshAgent>();

        // U: Disabling auto-braking allows for continuous movement
        //    between points (ie, the agent doesn't slow down as it
        //    approaches a destination point).
        agent.autoBraking = false;      

        GotoNextPoint();
    }

    void Update()
    {
        if (onPatrol)
        {
            //print("Patrol route's being updated!");
            ChooseNextPointAndMove(agent);
        }
    }

    public void ChooseNextPointAndMove(NavMeshAgent agent)
    {
        //print("Choosing next point and moving!");

        // U: Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.isStopped = true;

            if (tm.TimeCount(RestTime))
            {
                agent.isStopped = false;
                GotoNextPoint();
            }
        }
    }

    void GotoNextPoint()
    {       
        // U: Returns if no points have been set up
        if (PatrolPoints.Length == 0) return;

        // U: Set the agent to go to the currently selected destination.
        agent.destination = PatrolPoints[destPoint].position;

        // U: Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % PatrolPoints.Length;
    }

    public void StartPatrol()
    {        
        onPatrol = true;
        //agent.isStopped = false;
    }

    public void StopPatrol()
    {        
        onPatrol = false;
        //agent.isStopped = true;
    }

    public void Halt()
    {

    }
}