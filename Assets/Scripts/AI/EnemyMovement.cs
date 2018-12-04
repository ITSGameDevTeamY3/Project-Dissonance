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


    private Transform walkTarget, runTarget, rotateTarget;

    public enum MovementPhase
    {
        WALK,
        RUN,
        ROTATE
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
        }
    }

    // WALK
    public void SetWalkTarget(Transform target)
    {
        walkTarget = target;
        agent.speed = walkSpeed;
        movementPhase = MovementPhase.WALK;
    }

    public void WalkTowards(Transform target)
    {
        agent.SetDestination(target.position);

        //if()
    }

    // RUN

    // ROTATION
    public void RotateTowards(Transform target)
    {
        // Get the difference in position between the agent and the disturbance.
        Vector3 direction =
            (target.position - transform.position).normalized;

        direction.y = 0;

        // Get the rotation the agent needs to have in order to be facing the disturbance.
        Quaternion lookRotation = Quaternion.LookRotation(
            new Vector3(direction.x, direction.y, direction.z)); // We make y = 0 to flatten the position we will rotate to...
                                                                 // ...this means that our agent will only rotate its Y position (As an enemy in a game would).          

        // We use the slerp method to get our enemy to rotate towards the direction of the disturbance.
        transform.rotation = Quaternion.Lerp(
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
}
