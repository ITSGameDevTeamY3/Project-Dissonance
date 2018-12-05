using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement : MonoBehaviour
{
    // These 3 properties are set in the enemy controller script.
    private float walkSpeed, runSpeed, turningSpeed, stoppingDistance;

    private EnemyController enemyController;
    private NavMeshAgent agent;
    private Vector3 walkTarget, runTarget, rotateTarget;
    //private Transform rotateTarget;


    public enum MovementPhase
    {
        WALK,
        RUN,
        ROTATE,
        SURVEY
    }

    public MovementPhase movementPhase;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        walkSpeed = agent.speed;
        runSpeed = walkSpeed * 2;
        turningSpeed = enemyController.turningSpeed;
        stoppingDistance = enemyController.stoppingDistance;
    }

    void Update()
    {
        switch (movementPhase)
        {
            case MovementPhase.WALK:               
                WalkTowards(walkTarget);
                
                break;

            case MovementPhase.RUN:
                break;

            case MovementPhase.ROTATE:
                RotateTowards(rotateTarget);
                break;

            case MovementPhase.SURVEY:             
                SurveyArea();
                break;
        }
    }

    #region WALK
    public void SetWalkTarget(Vector3 target)
    {
        walkTarget = target;
        agent.speed = walkSpeed;
        // Set the agent's stopping distance.
        agent.stoppingDistance = stoppingDistance;
        //print("Set phase to walk.");
        movementPhase = MovementPhase.WALK;
    }

    public void WalkTowards(Vector3 target)
    {
        // Set the agent's destination.   
        agent.SetDestination(target);
        
        if(DestinationReached())
        {
            movementPhase = MovementPhase.SURVEY;
        }
    }
    #endregion

    // RUN

    #region ROTATION
    public void SetRotationTarget(Vector3 target)
    {
        rotateTarget = target;
        movementPhase = MovementPhase.ROTATE;
    }

    public void RotateTowards(Vector3 target)
    {
        // Get the difference in position between the agent and the disturbance.
        Vector3 direction =
            (target - transform.position).normalized;
        // Target.position or target.point? Interesting...
        

        // Get the rotation the agent needs to have in order to be facing the disturbance.
        Quaternion lookRotation = Quaternion.LookRotation(
            new Vector3(direction.x, 0, direction.z)); // We make y = 0 to flatten the position we will rotate to...
                                                                 // ...this means that our agent will only rotate its Y position (As an enemy in a game would).          

        // We use the slerp method to get our enemy to rotate towards the direction of the disturbance.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * turningSpeed);      
    }     
    #endregion

    // SURVEY
    // This method will be rather simple for now.
    public void SurveyArea()
    {
        print("Test");
        print("Agent stopped: " + agent.isStopped);

        agent.isStopped = false;

        print("Agent stopped: " + agent.isStopped);

        // Look to the left.
        Quaternion lookRotation = Quaternion.LookRotation(
           new Vector3(0, 10, 0));

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * turningSpeed);

        // Look to the right.
    }

    // Check if the agent has reached their destination.
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
}