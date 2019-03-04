using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMOD.Studio;
using FMODUnity;

[RequireComponent(typeof(EnemyController))]
public class EnemySounds : MonoBehaviour
{
    [EventRef]
    public string footsteps = "event:/Master/SFX_Events/FootSteps/footsteps3D";

    [EventRef]
    public string gunShotSound = "event:/Master/SFX_Events/Gunshot/GunshotRifle";

    EventInstance gunShotEvnt;

    bool isMoving;
    public float moveSpeed;

    NavMeshAgent agent;
    EnemyController enemyController;

 

    void Start()
    {
        gunShotEvnt = RuntimeManager.CreateInstance(gunShotSound);
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
        InvokeRepeating("GetFootSteps", 0, moveSpeed);
    }
    void Update()
    {
        GetFootSteps();
        PlayRifleShots();
    }

    void GetFootSteps()
    {
        if (enemyController.enemyPhase != EnemyController.Phase.HALT && !agent.isStopped)
        {
            FMODUnity.RuntimeManager.PlayOneShot(footsteps, transform.position);
        }
    }

    public void PlayRifleShots()
    {
        gunShotEvnt.start();
    }


    void OnDisable()
    {
        isMoving = false;
    }
}
