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
    TimeManager tm = new TimeManager();
    #endregion

    public enum Phase
    {
        PATROL,
        HALT,
        INVESTIGATE,
        ALERT,
        DECEASED
    }

    private Phase enemyState; 

    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
	}
	
	
	void Update ()
    {
		switch(enemyState)
        {
            case Phase.PATROL:
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
}
