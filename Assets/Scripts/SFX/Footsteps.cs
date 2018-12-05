using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMOD.Studio;
using FMODUnity;

[RequireComponent(typeof(EnemyController))]
public class Footsteps : MonoBehaviour
{
    [EventRef]

    public string inputSound;
    [EventRef]
    public string AlertTheme;
    bool isMoving;
    public float moveSpeed;

    NavMeshAgent agent;
    EnemyController enemyController;
    public bool alerted = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
        InvokeRepeating("GetFootSteps", 0, moveSpeed);

        //if (enemyController.enemyPhase == EnemyController.Phase.ALERT)
        //{
        //    GetAttackingTheme();
        //}
    }
    void Update()
    {
        //GetFootSteps();
    }

    void GetFootSteps()
    {
        if (enemyController.enemyPhase != EnemyController.Phase.HALT && !agent.isStopped)
        {
            FMODUnity.RuntimeManager.PlayOneShot(inputSound, transform.position);
        }
    }

    //void GetAttackingTheme()
    //{
    //   RuntimeManager.PlayOneShot(AlertTheme);
    //}

    //public void GetPatrolingTheme()
    //{
    //    if (!alerted)
    //    {
    //        RuntimeManager.PlayOneShot(AlertTheme);
    //        alerted = true;
    //    }        
    //}

    //public void StopPatrolingTheme()
    //{
    //    if (alerted)
    //    {
    //        // Destroys ALL sounds, we want it to destroy only one.
    //      //  RuntimeManager.Destroy();
    //      //  alerted = false;
    //    }
    //}

    void OnDisable()
    {
        isMoving = false;
    }
}
