using Assets.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    #region Movement Properties
    public bool Neutral; // Whether or not the enemy's movement is neutral (i.e. not moving outside of their assigned patrol/post).

    // These 4 properties are obtained from the enemy controller script. They are set in the Inspector window.
    private float walkSpeed, runSpeed, turningSpeed, stoppingDistance, surveyDistance, attackDistance, alertDuration;
    const float VIEW_DISTANCE = 4; 

    private EnemyController AI;
    private NavMeshAgent agent;
    private Transform rotater;
    private GameObject POV_GO;
    private List<Transform> surveyPoints;
    private Vector3 walkTarget, runTarget, rotateTarget;
    private bool alerted, coroutineStarted;

    public enum MovementPhase
    {
        WALK,    // When the enemy is moving in to investigate a disturbance.
        RUN,     // When the enemy is moving in to investigate a disturbance during the Alert Phase.
        ROTATE,  // When the enemy has to turn and face something.
        SURVEY,  // When the enemy is investigating a disturbance zone.
        ATTACK,  // When the enemy's got the player in their sights.
        NEUTRAL  // When the enemy is to return to their post/patrol.
    }
    public MovementPhase movementPhase;
           
    TimeManager tm = new TimeManager();   
    #endregion

    void Start()
    {
        #region Obtain variables from the scripts we require.
        AI = GetComponent<EnemyController>(); // Get our EC script and the variables we need.
        POV_GO = AI.POV_GO;        
        surveyPoints = AI.surveyPoints;
        turningSpeed = AI.TurningSpeed;
        agent = GetComponent<NavMeshAgent>(); // Get our NavMesh Agent script and the variables we need.
        walkSpeed = agent.speed;
        runSpeed = walkSpeed * 1.2f;
        stoppingDistance = agent.stoppingDistance;
        surveyDistance = stoppingDistance + VIEW_DISTANCE;
        attackDistance = AI.AlertDistance;
        alertDuration = AI.AlertDuration;
        alerted = false;
        #endregion

        movementPhase = MovementPhase.NEUTRAL; // Enemies will initially have no independent movement outside their patrol/post.       
    }

    void Update()
    {
        if (alerted && !AI.PlayerTracker.LineOfSight.enabled && !coroutineStarted)
        {
            StartCoroutine("CallOffSearch");
        }

        else if (alerted && coroutineStarted)
        {
            StopCoroutine("CallOffSearch");
        }

        switch (movementPhase)
        {
            case MovementPhase.WALK:
                if (DestinationReached()) movementPhase = MovementPhase.SURVEY;
                break;

            case MovementPhase.RUN:               
                if (DestinationReached()) movementPhase = MovementPhase.ATTACK;                                        
                break;

            case MovementPhase.ROTATE:
                RotateTowards("Enemy", rotateTarget);
                break;

            case MovementPhase.SURVEY:
                SurveyArea();
                break;

            case MovementPhase.ATTACK:
                agent.stoppingDistance = attackDistance;

                if(tm.TimeCount(AI.ShootCooldown)) AI.HitScanner.Active = true;

                if(PlayerInAttackRange()) RotateTowards("Enemy", AI.Player.transform.position);

                if (!PlayerInAttackRange())
                {
                    SetRunTarget(AI.Player.transform.position);                    
                }                
                break;

            case MovementPhase.NEUTRAL:
                if (!Neutral) Neutral = true;
                break;
        }
    }

    #region ROTATION
    public void SetRotationTarget(Vector3 target)
    {
        rotateTarget = target;
        movementPhase = MovementPhase.ROTATE;
        if (Neutral) Neutral = false;
    }

    private void RotateTowards(string observer, Vector3 target)
    {
        if (observer == "Enemy") rotater = transform;
        else if (observer == "EnemyPOV") rotater = POV_GO.transform;

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

    #region WALK
    public void SetWalkTarget(Vector3 target)
    {
        walkTarget = target;
        agent.speed = walkSpeed;
        agent.stoppingDistance = surveyDistance;
        agent.SetDestination(target); // Set the walk target as our NavMesh Agent's target, this will have the agent moving of its own accord.
        movementPhase = MovementPhase.WALK;
        if (Neutral) Neutral = false;
    }
    #endregion

    #region RUN
    public void SetRunTarget(Vector3 target)
    {
        if (!alerted) alerted = true;
        if (movementPhase != MovementPhase.ATTACK)
        {
            runTarget = target;
            agent.speed = runSpeed;
            agent.stoppingDistance = attackDistance;
        }

        if (!AI.PlayerTracker.LineOfSight.enabled) agent.stoppingDistance = surveyDistance;

        
        agent.SetDestination(target); // Set the walk target as our NavMesh Agent's target, this will have the agent moving of its own accord.
        movementPhase = MovementPhase.RUN;
        if (Neutral) Neutral = false;
    }
    #endregion

    // SURVEY - This method will be rather simple for now.
    private void SurveyArea()
    {
        if (agent.hasPath) agent.ResetPath();       

        // If the enemy sees nothing of interest in the disturbance zone, his movement will return to neutral after 3 seconds.
        if (tm.TimeCount(3)) movementPhase = MovementPhase.NEUTRAL;
    }


    private bool DestinationReached() // Check if the agent has reached their destination.
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

    private bool PlayerInAttackRange()
    {
        if (Vector3.Distance(transform.position, AI.Player.transform.position) >= agent.stoppingDistance) return false;
        else return true;
    }

    IEnumerator CallOffSearch()
    {
        yield return new WaitForSeconds(alertDuration);
        if(alerted && !AI.PlayerTracker.LineOfSight.enabled)
        {
            coroutineStarted = false;
            movementPhase = MovementPhase.NEUTRAL;            
        }        
    }
}