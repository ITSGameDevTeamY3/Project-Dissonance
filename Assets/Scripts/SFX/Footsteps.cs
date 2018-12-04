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
    public string AlertTheme;
    bool isMoving;
    public float moveSpeed;

    NavMeshAgent agent;
    EnemyController enemyController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
        InvokeRepeating("GetFootSteps", 0, moveSpeed);

        if (enemyController.enemyPhase == EnemyController.Phase.ALERT)
        {
            GetAttackingTheme();
        }
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

    void GetAttackingTheme()
    {
       RuntimeManager.PlayOneShot(AlertTheme);
    }

    void GetPatrolingTheme()
    {
        RuntimeManager.PlayOneShot(AlertTheme);
    }

    void OnDisable()
    {
        isMoving = false;
    }
}
