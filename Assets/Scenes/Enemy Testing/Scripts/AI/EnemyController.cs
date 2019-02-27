using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// This script will only work if our enemy has an Agent component.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // https://forum.unity.com/threads/rigidbody-keeps-sliding.32965/ Something to keep an eye on.

    #region Enemy Properties
    // Properties that can be altered in the Unity inspector. Some of these might be moved to other scripts for the sake of cleanliness.
    public int Health;
    public float ShootCooldown, WalkSpeed, TurningSpeed, HaltTime, SuspicionDistance, AlertDistance;
    public Light Flashlight;
    public GameObject Player;
    // The following public properties are visible in the Inspector but they are set automatically.
    public GameObject POV_GO;
    public List<Transform> surveyPoints = new List<Transform>();

    // This camera is used to determine where the user has clicked on-screen. It'll be removed when disturbance investigation testing is over.
    public Camera disturbanceCam;
    public RaycastHit hit; // This variable will keep track of the object that you clicked.
    private Vector3 originalDestination; // This variable stores where the agent was originally headed.   

    // Bools.
    private bool alerted = false;
    private bool vigil, disturbanceEncounteredPreviously, disturbanceInvestigated;

    // Properties that are automatically set when the object is created.
    NavMeshAgent agent;
    Patrol patrolRoute;
    Vector3 post, disturbanceZone;
    EnemyMovement movement;
    TimeManager tm = new TimeManager();
    Camera POV;
    PlayerTracker playerTracker;
    Renderer playerRenderer;
    float defaultStoppingDistance;

    //Footsteps footSteps; For Adrian - SFX variable for enemy can be added here.
    #endregion

    #region Phases
    public enum Phase
    {
        PATROL,
        VIGIL,
        HALT,
        INVESTIGATE,
        ALERT,
        DECEASED
    }
    //[SerializeField]
    public Phase enemyPhase, previousPhase;
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = WalkSpeed;
        defaultStoppingDistance = agent.stoppingDistance;
        //footSteps = GetComponent<Footsteps>();

        // Get access to the enemy object's children.
        foreach (Transform child in transform)
        {
            if (child.tag == "POV" && POV_GO == null) POV_GO = child.gameObject; // Get access to the enemy's POV.
            if (child.tag == "SurveyPoint") surveyPoints.Add(child); // Get the enemy's survey points.
            if (child.tag == "PlayerTracker" && playerTracker == null) playerTracker = child.GetComponent<PlayerTracker>(); // Get the enemy's player tracker script.
        }

        POV = POV_GO.GetComponent<Camera>();
        if (Player != null)
        {
            playerRenderer = Player.GetComponent<Renderer>();
        }

        movement = GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        #region Set the enemy's patrol route, if we haven't given them one, assign them a post.
        // Set the enemy's patrol route if we have given them one in the editor.

        // The following commented if statement is an alternate choice to checking if the component is there. The uncommented one is what we'll use.
        // if (GetComponent<Patrol>().enabled != false)
        if (GetComponent<Patrol>() != null)
        {
            patrolRoute = GetComponent<Patrol>();
            // NOTE: We can only set enemy phase directly (Like the line below.) in this Start method. Everywhere else, we should use the SetPhase(newPhase) method.
            enemyPhase = Phase.PATROL;
            UpdateBehaviour();
        }

        // If we haven't given them a patrol route, assign their starting location as their post. (An area they've been assigned to keep watch at.)
        else
        {
            post = GetComponent<Transform>().position;
            // REMINDER: Everywhere else, we use the SetPhase(newPhase) method.
            enemyPhase = Phase.VIGIL;
            vigil = true;
            UpdateBehaviour();
        }
        #endregion        
    }

    void Update()
    {
        PhaseCheck();
        NeutralMovementCheck();
        // The enemy will always check for disturbances regardless of its current phase. (Unless it knows where the player is).
        if (enemyPhase != Phase.ALERT || enemyPhase == Phase.ALERT && !playerTracker.PlayerWithinView) CheckForDisturbances();
        // The enemy will of course constantly look for the player regardless of its phase.
        CheckForIntruder();
        if(movement.enabled) PursueGlimpsedPlayer();
    }

    void UpdateBehaviour()
    {
        previousPhase = enemyPhase; // Ensure that enemyPhase and previousPhase are once again in sync.

        switch (enemyPhase)
        {
            case Phase.PATROL: // TO FIX: Enemy speed issue here. Movement speed above 4 causes enemy to stop. Tweak REST_DISTANCE on the Patrol script.                             
                Flashlight.color = Color.white; // For Adrian - Whenever the enemy toggles their flashlight/changes light color, we could have a little "click" sound.
                patrolRoute.StartPatrol();
                break;

            case Phase.VIGIL:
                break;

            case Phase.HALT:
                StartCoroutine("Halt");
                break;

            case Phase.INVESTIGATE:
                StopCoroutine("Halt");                
                movement.SetWalkTarget(disturbanceZone);
                break;

            case Phase.ALERT:
                StartCoroutine("Alert");
                break;

            case Phase.DECEASED:
                // For Adrian - I still have to implement the enemy's death, but a "death" sound would be used somewhere here.

                Flashlight.enabled = false;
                break;
        }
    }   

    private void CheckForDisturbances()
    {             
        if (Input.GetMouseButtonDown(0) && disturbanceCam != null) // If a disturbance was heard...
        {
            // Send out a ray to the position where you clicked.
            Ray ray = disturbanceCam.ScreenPointToRay(Input.mousePosition);

            // This if will determine whether or not you clicked an object.
            if (Physics.Raycast(ray, out hit))
            {
                disturbanceZone = hit.point;
                SetPhase(Phase.HALT);
            }
        }

        else if (playerTracker.PlayerGlimpsed && enemyPhase != Phase.INVESTIGATE) // If a disturbance was seen...
        {
            disturbanceZone = playerTracker.PlayerGlimpsedPosition;
            SetPhase(Phase.HALT); 
        }
    }

    private void CheckForIntruder()
    {
        if (playerTracker != null)
        {
            // Here's where we detect whether or not the player is within the enemy's camera. https://answers.unity.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
            if (playerRenderer.IsVisibleFrom(POV))
            {
                playerTracker.PlayerWithinView = true;
            }

            else if (playerTracker.PlayerWithinView) playerTracker.PlayerWithinView = false;

            if (playerTracker.PlayerFound == true)
            {
                SetPhase(Phase.ALERT);
            }
        }
    }

    private void PursueGlimpsedPlayer() // I'll need to keep an eye on this method.
    {
        if (playerTracker.PlayerWithinView && playerTracker.PlayerInSuspicionRange && enemyPhase == Phase.INVESTIGATE)
        {
            movement.SetWalkTarget(Player.transform.position);
        }
    }

    #region COROUTINES
    IEnumerator Halt()
    {
        // For Adrian - A "Hm? What was that?" sound effect or something like that could be played here.

        Flashlight.color = Color.yellow;
        
        StorePatrolEnableMovement();

        // Turn towards the direction of the disturbance.        
        movement.SetRotationTarget(disturbanceZone);

        // Wait for the specified halt time before investigating.
        if(!disturbanceEncounteredPreviously) yield return new WaitForSeconds(HaltTime);

        disturbanceEncounteredPreviously = true;
        SetPhase(Phase.INVESTIGATE);
    }

    IEnumerator Alert()
    {
        // For Adrian - A "Who's that?" or something similar could be played here.
        Flashlight.color = Color.red;

        StorePatrolEnableMovement();

        // Turn towards the direction of the disturbance.
        movement.SetRotationTarget(Player.transform.position);

        // Wait for the specified halt time before investigating.
        yield return new WaitForSeconds(HaltTime / 2);

        print("The enemy will likely shoot at this point!"); // For Adrian - It'll have to wait until I have the enemy shooting the player, but definitely load up some shoot sounds into the project. :)
    }
    #endregion

    private void PhaseCheck() // Check if current phase and previous phase are in sync. If not, update enemy behaviour.
    {
        if (enemyPhase != previousPhase)
        {
            UpdateBehaviour();
        }
    }

    private void SetPhase(Phase newPhase)
    {
        if (newPhase != enemyPhase)
        {
            previousPhase = enemyPhase;
            enemyPhase = newPhase;
        }
    }

    private void StorePatrolEnableMovement()
    {
        if (agent.hasPath)
        {
            // Keep track of where the enemy was originally headed.
            originalDestination = agent.destination;
            // ...stop the enemy's patrol.
            patrolRoute.StopPatrol();
            // Clear the agent's path.
            agent.ResetPath();
        }
        movement.enabled = true; // The enemy's movement (independent of any patrol routes) will begin now.
    }

    private void NeutralMovementCheck() // If the movement is enabled and neutral, it will be disabled here.
    {
        if (movement.enabled && movement.Neutral)
        {
            movement.enabled = false;
            agent.destination = originalDestination;
            agent.stoppingDistance = defaultStoppingDistance; // Reset stopping distance.
            disturbanceEncounteredPreviously = false;
            if (!vigil) SetPhase(Phase.PATROL); // Set the enemy back to their PATROL/VIGIL phase.
            else SetPhase(Phase.VIGIL);
        }
    }
}