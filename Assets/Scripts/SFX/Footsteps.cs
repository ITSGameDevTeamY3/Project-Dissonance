using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
public class Footsteps : MonoBehaviour
{
    [FMODUnity.EventRef]

    public string inputSound;
    bool isMoving;
    public float moveSpeed;

    NavMeshAgent agent;
    EnemyController enemyController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
        InvokeRepeating("GetFootSteps", 0, moveSpeed);
    }

    void Update()
    {
        //GetFootSteps();
    }

    void GetFootSteps()
    {
        if (enemyController.enemyPhase != EnemyController.Phase.HALT && !agent.isStopped)
        {
            FMODUnity.RuntimeManager.PlayOneShot(inputSound);
        }
    }

    void OnDisable()
    {
        isMoving = false;
    }
}
