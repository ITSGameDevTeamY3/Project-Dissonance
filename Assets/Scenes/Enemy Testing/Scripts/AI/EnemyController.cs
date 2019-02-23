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
    private float defaultStoppingDistance;
    private bool alerted = false;
    private bool vigil;
    private Renderer playerRenderer;

    // Properties that are automatically set when the object is created.
    NavMeshAgent agent;
    Patrol patrolRoute;
    Vector3 post;
    EnemyMovement movement;
    TimeManager tm = new TimeManager();
    Camera POV;
    PlayerTracker playerTracker;

    //Footsteps footSteps; We can add FMOD SFX later.
    #endregion

    public enum Phase
    {
        PATROL,
        VIGIL,
        HALT,
        INVESTIGATE,
        ALERT,
        DECEASED
    }

    [SerializeField]
    public Phase enemyPhase, previousPhase;

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
            if (child.tag == "PlayerTracker") playerTracker = child.GetComponent<PlayerTracker>(); // Get the enemy's player tracker script.
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
        if (enemyPhase != previousPhase)
        {
            UpdateBehaviour();
        }

        if (movement.enabled && movement.Neutral)
        {
            movement.enabled = false;
            agent.destination = originalDestination;
            agent.stoppingDistance = defaultStoppingDistance; // Reset stopping distance.
            if (!vigil) SetPhase(Phase.PATROL); // Set the enmy back to their PATROL/VIGIL phase.
            else SetPhase(Phase.VIGIL);
        }

        // The enemy will always check for disturbances regardless of its current phase. (Unless it knows where the player is).
        CheckForDisturbances();

        // The enemy will of course constantly look for the player regardless of its phase.
        CheckForIntruder();
    }

    void UpdateBehaviour()
    {
        previousPhase = enemyPhase; // Ensure that enemyPhase and previousPhase are once again in sync.

        switch (enemyPhase)
        {
            case Phase.PATROL: // TO FIX: Enemy speed issue here. Movement speed above 4 causes enemy to stop. Tweak REST_DISTANCE on the Patrol script.                             
                Flashlight.color = Color.white;
                patrolRoute.StartPatrol();
                break;

            case Phase.VIGIL:
                break;

            case Phase.HALT:
                StartCoroutine("Halt");
                break;

            case Phase.INVESTIGATE:
                StopCoroutine("Halt");

                // Set the disturbance location as the enemy's destination.
                if(!playerTracker.PlayerGlimpsed) movement.SetWalkTarget(hit.point);
                else movement.SetWalkTarget(playerTracker.PlayerGlimpsedPosition);
                break;

            case Phase.ALERT:
                StartCoroutine("Alert");
                break;

            case Phase.DECEASED:
                Flashlight.enabled = false;
                break;
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

    private void CheckForDisturbances()
    {
        if (Input.GetMouseButtonDown(0) && disturbanceCam != null)
        {
            // Send out a ray to the position where you clicked.
            Ray ray = disturbanceCam.ScreenPointToRay(Input.mousePosition);

            // This if will determine whether or not you clicked an object.
            if (Physics.Raycast(ray, out hit))
            {
                SetPhase(Phase.HALT);
            }
        }

        else if (playerTracker.PlayerGlimpsed && enemyPhase != Phase.HALT)
        {            
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
        }

        if (playerTracker.PlayerFound == true)
        {
            SetPhase(Phase.ALERT);
        }
    }

    #region COROUTINES
    IEnumerator Halt()
    {
        Flashlight.color = Color.yellow;

        #region If the enemy was on patrol...
        if (agent.hasPath)
        {
            // Keep track of where the enemy was originally headed.
            originalDestination = agent.destination;
            // ...stop the enemy's patrol.
            patrolRoute.StopPatrol();
            // Clear the agent's path.
            agent.ResetPath();
        }

        // The enemy's movement (independent of any patrol routes) will begin now.
        movement.enabled = true;
        #endregion

        // Turn towards the direction of the disturbance.
        if (!playerTracker.PlayerGlimpsed) movement.SetRotationTarget(hit.point);
        else movement.SetRotationTarget(playerTracker.PlayerHit.point); // PlayerHit or PlayerGlimpsedPosition? Another conundrum to solve.

        // Wait for the specified halt time before investigating.
        yield return new WaitForSeconds(HaltTime);

        playerTracker.PlayerGlimpsed = false;

        SetPhase(Phase.INVESTIGATE);
    }

    IEnumerator Alert()
    {
        print("Who's that?");
        Flashlight.color = Color.red;

        #region If the enemy was on patrol...
        if (agent.hasPath)
        {
            // Keep track of where the enemy was originally headed.
            originalDestination = agent.destination;
            // ...stop the enemy's patrol.
            patrolRoute.StopPatrol();
            // Clear the agent's path.
            agent.ResetPath();
        }

        // The enemy's movement (independent of any patrol routes) will begin now.
        movement.enabled = true;
        #endregion

        // Turn towards the direction of the disturbance.
        movement.SetRotationTarget(Player.transform.position);

        // Wait for the specified halt time before investigating.
        yield return new WaitForSeconds(HaltTime / 2);

        print("The enemy will likely shoot at this point!");
    }
    #endregion
}