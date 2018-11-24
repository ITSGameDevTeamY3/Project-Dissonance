using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This script will only work if our enemy has an Agent component.
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    #region Enemy Properties
    // Properties that can be altered in the Unity inspector. Some of these might be moved to other scripts for the sake of cleanliness.
    public int Health;
    public float shootCooldown;
    public float movementSpeed;
    public float turningSpeed;
    public float stoppingDistance;
    public float haltTime;

    // This camera is used to determine where the user has clicked on-screen. It'll be removed when disturbance investigation testing over.
    public Camera disturbanceCam;
    // This variable will keep track of the object that you clicked.
    public RaycastHit hit;
    // Bool used to keep track of whether the enemy's halted. May be replaced with enum.
    private bool halted;
    // This variable stores where the agent was originally headed.
    private Vector3 originalDestination;

    // Properties that are automatically set when the object is created.
    NavMeshAgent agent;
    Patrol patrolRoute;
    Vector3 post;
    TimeManager tm = new TimeManager();
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

    private Phase enemyState; 

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();

        // Set the enemy's patrol route if we have given them one in the editor.
        if (GetComponent<Patrol>() != null)
        {
            patrolRoute = GetComponent<Patrol>();
            enemyState = Phase.PATROL;
        }

        // If we haven't given them a patrol route, assign their starting location as their post. (An area they've been assigned to keep watch at.)
        else
        {
            post = GetComponent<Transform>().position;
            enemyState = Phase.VIGIL;
        }
	}
	
	
	void Update ()
    {
		switch(enemyState)
        {
            case Phase.PATROL:
                patrolRoute.ChooseNextPointAndMove(agent);
                CheckForDisturbances();
                break;

            case Phase.VIGIL:
                break;

            case Phase.HALT:
                break;

            case Phase.INVESTIGATE:
                break;

            case Phase.ALERT:
                break;

            case Phase.DECEASED:
                break;
        }
	}

    void CheckForDisturbances()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Send out a ray to the position where you clicked.
            Ray ray = disturbanceCam.ScreenPointToRay(Input.mousePosition);

            // This if will determine whether or not you clicked an object.
            if (Physics.Raycast(ray, out hit))
            {
                enemyState = Phase.HALT;
            }
        }
    }

}
