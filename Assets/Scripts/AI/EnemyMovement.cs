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
    private Vector3 walkTarget, runTarget;
    private Transform rotateTarget;


    public enum MovementPhase
    {
        WALK,
        RUN,
        ROTATE,
        SURVEY
    }

    private MovementPhase movementPhase;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        walkSpeed = enemyController.walkSpeed;
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
                
                break;
        }
    }

    // WALK
    public void SetWalkTarget(Vector3 target)
    {
        walkTarget = target;
        agent.speed = walkSpeed;
        // Set the agent's stopping distance.
        agent.stoppingDistance = stoppingDistance;
        movementPhase = MovementPhase.WALK;
    }

    public void WalkTowards(Vector3 target)
    {
        // Set the agent's destination.
        agent.SetDestination(target);
        
        if(DestinationReached())
        {
            print("Destination reached!");
        }
    }

    // RUN

    // ROTATION
    public void RotateTowards(Transform target)
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
            Time.deltaTime * turningSpeed);

        //print("My Y: " + transform.rotation.y);
        //print("My Target Y: " + lookRotation.y);

        //if (Mathf.Approximately(transform.rotation.y, lookRotation.y))
        //{
        //if (Vector3.Distance(transform.position, target.position) <= stoppingDistance)
        //{
        //    agent.transform.LookAt(direction);
        //}
        //return true;
        //}

        //else return false;
    }

    public void SetRotationTarget(Transform target)
    {
        rotateTarget = target;
        movementPhase = MovementPhase.ROTATE;
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