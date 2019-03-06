using Assets.Scripts.Managers;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
// This script will only work if our enemy has an Agent component.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // https://forum.unity.com/threads/rigidbody-keeps-sliding.32965/ Something to keep an eye on.

    #region Enemy Properties           
    // Properties that can be altered in the Unity inspector. Some of these might be moved to other scripts for the sake of cleanliness.
    public bool NuclearMode;
    public bool DebugMode = false;
    public int Health;
    public GameObject Player;
    public float ShootCooldown, WalkSpeed, RunMultiplier, TurningSpeed, HaltTime, SurveyTime, SuspicionDistance, AlertDistance, AlertDuration;
    // GravMultiplier, PlayerTracker and Alerted are public but they are set automatically.
    public float GravMultiplier = 0.2f;
    public PlayerTracker PlayerTracker;
    public bool Alerted = false;
    // The following public properties were visible in the Inspector but they are set automatically. Make these public if any of them give trouble and you need to debug.
    public Light Flashlight;
    public GameObject POV_GO;
    public SphereCollider BackupCallZone;
    List<Transform> surveyPoints = new List<Transform>();
    public HitScanner HitScanner;
    public Renderer playerRenderer;   
    // Properties that are automatically set when the object is created.
    NavMeshAgent agent;
    Patrol patrolRoute;
    Vector3 post, disturbanceZone;
    EnemyMovement movement;
    TimeManager tm = new TimeManager();
    Camera POV;
    float defaultStoppingDistance;

    // This camera is used to determine where the user has clicked on-screen. It'll be removed when disturbance investigation testing is over.
    public Camera disturbanceCam;
    public RaycastHit hit; // This variable will keep track of the object that you clicked.
    private Vector3 originalDestination; // This variable stores where the agent was originally headed.   

    // Bools.    
    private bool vigil;
    private EventInstance gunShotEvnt;

    // MusicManager controller
    private MusicManager _musicManager;
    private EnemySounds enemySounds;
    #endregion

    #region Phases
    public enum Phase
    {
        PATROL,
        VIGIL,
        HALT,
        INVESTIGATE,
        ALERT,
        PURSUIT,
        DECEASED
    }
    public Phase enemyPhase, previousPhase;
    #endregion

    void Start()
    {
        #region Use the GravMultiplier to adjust enemy speeds accordingly.
        WalkSpeed = WalkSpeed * GravMultiplier;
        TurningSpeed = TurningSpeed * GravMultiplier;
        #endregion

        agent = GetComponent<NavMeshAgent>();
        agent.speed = WalkSpeed;
        defaultStoppingDistance = agent.stoppingDistance;
        enemySounds = GetComponent<EnemySounds>();
        Player = GameObject.FindGameObjectWithTag("Player"); // The player is found in the scene automatically and set here.
        disturbanceCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); // Find the main camera and set it here. NOTE: This will be removed soon.        
        _musicManager = GameObject.FindWithTag("MusicManager").GetComponent<MusicManager>();

        // Get access to the enemy object's children.
        foreach (Transform child in transform)
        {
            if (child.tag == "EnemyPOV" && POV_GO == null) POV_GO = child.gameObject; // Get access to the enemy's POV.
            if (child.tag == "BackupCall" && BackupCallZone == null) BackupCallZone = child.gameObject.GetComponent<SphereCollider>();
            if (child.tag == "SurveyPoint") surveyPoints.Add(child); // Get the enemy's survey points.
            if (child.tag == "PlayerTracker" && PlayerTracker == null) PlayerTracker = child.GetComponent<PlayerTracker>(); // Get the enemy's player tracker script.
            if (child.tag == "HitScanner") HitScanner = child.GetComponent<HitScanner>();
        }

        POV = POV_GO.GetComponent<Camera>();
        if (Player != null)
        {
            playerRenderer = Player.transform.Find("Protagonist/Mesh").GetComponent<Renderer>();
        }

        movement = GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Get Enemy Flashlight.
        Flashlight = transform.Find("PAC-AUG/Flashlight Attachment").GetComponent<Light>();

        BackupCallZone.enabled = false; // Disable the backup call zone since it should only be active when the enemy is alerted.

        #region Set the enemy's patrol route, if we haven't given them one, assign them a post.           
        if (GetComponent<Patrol>() != null) // Set the enemy's patrol route if we have given them one in the editor.    
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
        // These two should be in sync with the enemy movement speeds.
        Debug.Log("Controller Walk speed:" + WalkSpeed);
        Debug.Log("Controller run speed:" + WalkSpeed * RunMultiplier);

        PhaseCheck();
        NeutralMovementCheck();
        // The enemy will always check for disturbances regardless of its current phase. (Unless it knows where the player is).
        if (enemyPhase != Phase.ALERT || enemyPhase == Phase.ALERT && !PlayerTracker.PlayerWithinView) CheckForDisturbances();
        // The enemy will of course constantly look for the player regardless of its phase.
        CheckForIntruder();
        if (movement.enabled) PursueGlimpsedPlayer();
        HandleAlertCooldown();
    }

    void UpdateBehaviour()
    {
        previousPhase = enemyPhase; // Ensure that enemyPhase and previousPhase are once again in sync.

        switch (enemyPhase)
        {
            case Phase.PATROL: // TO FIX: Enemy speed issue here. Movement speed above 4 causes enemy to stop. Tweak REST_DISTANCE on the Patrol script.  
                _musicManager.Condition = MusicManager.Conditions.Normal;
                AlterFlashlightColourDebug(Color.white);
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
                if (NuclearMode) SceneManager.LoadScene("MissionFailed");
                else StartCoroutine("Alert");
                break;            
            case Phase.DECEASED:
                // For Adrian - I still have to implement the enemy's death, but a "death" sound would be used somewhere here.

                Flashlight.enabled = false; // For Adrian - Whenever the enemy's flashlight is switched on or off, we could play a little "click" sound.
                break;
        }
    }

    private void CheckForDisturbances()
    {
        if (Input.GetMouseButtonDown(0) && disturbanceCam != null && DebugMode) // If a disturbance was heard...
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

        else if (PlayerTracker.PlayerGlimpsed && enemyPhase != Phase.INVESTIGATE) // If a disturbance was seen...
        {
            disturbanceZone = PlayerTracker.PlayerGlimpsedPosition;
            SetPhase(Phase.HALT);
        }
    }

    private void CheckForIntruder()
    {
        if (PlayerTracker != null)
        {
            // Here's where we detect whether or not the player is within the enemy's camera. https://answers.unity.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
            if (playerRenderer.IsVisibleFrom(POV))
            {
                PlayerTracker.PlayerWithinView = true;
            }

            else if (PlayerTracker.PlayerWithinView) PlayerTracker.PlayerWithinView = false;

            if (PlayerTracker.PlayerFound == true)
            {
                SetPhase(Phase.ALERT);
            }
        }
    }

    private void PursueGlimpsedPlayer() // I'll need to keep an eye on this method.
    {
        if (PlayerTracker.PlayerWithinView && PlayerTracker.PlayerInSuspicionRange && enemyPhase == Phase.INVESTIGATE)
        {
            movement.SetWalkTarget(Player.transform.position);
        }
    }

    private void HandleAlertCooldown() // Gotta keep an eye on this one too.
    {
        if (Alerted)
        {
            if (!PlayerTracker.LineOfSight.enabled)
            {
                if (tm.AlertTimeCount(AlertDuration))
                {
                    CallOffSearch();
                }
            }
            else tm.ResetAlertClock(); // Reset the clock!
        }
    }
   
    #region COROUTINES
    IEnumerator Halt()
    {
        _musicManager.Condition = MusicManager.Conditions.Suspicious;

        AlterFlashlightColourDebug(Color.yellow);

        StorePatrolEnableMovement();

        // Turn towards the direction of the disturbance.        
        movement.SetRotationTarget(disturbanceZone);

        // Wait for the specified halt time before investigating.       
        yield return new WaitForSeconds(HaltTime);

        SetPhase(Phase.INVESTIGATE);
    }

    IEnumerator Alert()
    {
        _musicManager.Condition = MusicManager.Conditions.Alerted; // Play the alert track.
        Alerted = true;
        EnableBackupCall(true);

        AlterFlashlightColourDebug(Color.red);

        StorePatrolEnableMovement();

        // Turn towards the direction of the disturbance.
        movement.SetRotationTarget(Player.transform.position);

        Shoot();

        // Wait for the specified halt time before investigating.
        yield return new WaitForSeconds(HaltTime / 2);

        movement.SetRunTarget(Player.transform.position);
        StartCoroutine("PursueTarget", Player.transform);
    }

    IEnumerator PursueTarget(Transform target) 
    {
        // https://answers.unity.com/questions/1032018/how-to-follow-moving-destibation-with-navmeshagent.html The following coroutine was found at this link,
        // It enables the NavMeshAgent to chase a moving target.
        Vector3 previousTargetPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity);
        while (Vector3.SqrMagnitude(transform.position - target.position) > 0.1f)
        {
            // did target move more than at least a minimum amount since last destination set?
            if (Vector3.SqrMagnitude(previousTargetPosition - target.position) > 0.1f)
            {
                agent.SetDestination(target.position);
                previousTargetPosition = target.position;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    #endregion

    public void Shoot()
    {
        enemySounds.PlayRifleShots(); // Doesn't seem to work yet.
        HitScanner.Active = true;
    }

    public void EnableBackupCall(bool option)
    {
        if (option == true) BackupCallZone.enabled = true;
        else BackupCallZone.enabled = false;
    }

    public void CallOffSearch()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var enemy = go.GetComponent<EnemyController>();
            enemy.EndSearch();
        }
    }

    public void EndSearch()
    {
        movement.movementPhase = EnemyMovement.MovementPhase.NEUTRAL;
        StopCoroutine("PursueTarget");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BackupCall")
        {
            SetPhase(Phase.ALERT);            
        }
    }

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
            if (PlayerTracker.PlayerFound) PlayerTracker.PlayerFound = false; // I''ll keep an eye on this line.
            if (!vigil) SetPhase(Phase.PATROL); // Set the enemy back to their PATROL/VIGIL phase.
            else SetPhase(Phase.VIGIL);
        }
    }

    // Debug Methods
    private void AlterFlashlightColourDebug(Color newColor)
    {
        if (DebugMode && Flashlight != null) Flashlight.color = newColor;
    }
}