using Assets.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement : MonoBehaviour
{
    // These 4 properties are obtained from the enemy controller script. They are set in the Inspector window.
    private float walkSpeed, runSpeed, turningSpeed, stoppingDistance;

    private EnemyController enemyController;
    private NavMeshAgent agent;
    private Vector3 walkTarget, runTarget, rotateTarget;

    public enum MovementPhase
    {
        WALK,   // When the enemy is moving in to investigate a disturbance.
        RUN,    // When the enemy is moving in to investigate a disturbance during the Alert Phase.
        ROTATE, // When the enemy has to turn and face something.
        SURVEY  // When the enemy is investigating a disturbance zone.
    }
    public MovementPhase movementPhase;

    List<Transform> surveyPoints = new List<Transform>();
    Transform povDirection, rotater;
    int spCounter = 0;
    TimeManager tm = new TimeManager();
    bool leftTest; // Gonna remove.

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        walkSpeed = agent.speed;
        runSpeed = walkSpeed * 2;
        turningSpeed = enemyController.turningSpeed;
        stoppingDistance = enemyController.stoppingDistance;

        // Survey Points
        foreach (Transform child in transform)
        {
            if (child.tag == "POV" && povDirection == null) povDirection = child; // Get access to the enemy's POV.
            if (child.tag == "SurveyPoint") surveyPoints.Add(child); // Get the enemy's survey points.          
        }
    }

    void Update()
    {
        switch (movementPhase)
        {
            case MovementPhase.WALK:
                if (DestinationReached()) movementPhase = MovementPhase.SURVEY;
                break;

            case MovementPhase.RUN:
                break;

            case MovementPhase.ROTATE:
                RotateTowards("Enemy", rotateTarget);
                break;

            case MovementPhase.SURVEY:
                SurveyArea();
                break;
        }
    }

    public void SetWalkTarget(Vector3 target)
    {
        walkTarget = target;
        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(target); // Set the walk target as our NavMesh Agent's target, this will have the agent moving of its own accord.
        movementPhase = MovementPhase.WALK;
    }

    // RUN

    #region ROTATION
    public void SetRotationTarget(Vector3 target)
    {
        rotateTarget = target;
        movementPhase = MovementPhase.ROTATE;
    }

    public void RotateTowards(string observer, Vector3 target)
    {
        if (observer == "Enemy") rotater = transform;
        else if (observer == "POV") rotater = povDirection;

        // Get the difference in position between the agent and the disturbance.
        Vector3 direction =
            (target - rotater.position).normalized;
        // Target.position or target.point? Interesting...


        // Get the rotation the agent needs to have in order to be facing the disturbance.
        Quaternion lookRotation = Quaternion.LookRotation(
            new Vector3(direction.x, 0, direction.z)); // We make y = 0 to flatten the position we will rotate to...
                                                       // ...this means that our agent will only rotate its Y axis (As an enemy in a game would).          

        // We use the slerp method to get our enemy to rotate towards the direction of the disturbance.
        rotater.rotation = Quaternion.Slerp(
            rotater.rotation,
            lookRotation,
            Time.deltaTime * turningSpeed);
    }
    #endregion

    // SURVEY
    // This method will be rather simple for now.
    public void SurveyArea()
    {
        if (agent.hasPath) agent.ResetPath();

        // NOTE: Code below is super hard-coded on purpose for testing and showcase purposes. I'll have a much better implementation in soon.
        if (!leftTest) RotateTowards("POV", surveyPoints[0].position);
        else RotateTowards("POV", surveyPoints[1].position);

        if (tm.TimeCount(2) && !leftTest) leftTest = true;
    }

    bool DestinationReached() // Check if the agent has reached their destination.
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