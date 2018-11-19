using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Footsteps : MonoBehaviour
{
    [FMODUnity.EventRef]

    public string inputSound;
    bool isMoving;
    public float moveSpeed;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("GetFootSteps", 0, moveSpeed);
    }

    void Update()
    {
        //GetFootSteps();
    }

    void GetFootSteps()
    {
        if (!agent.isStopped)
        {
            FMODUnity.RuntimeManager.PlayOneShot(inputSound);
        }
    }

    void OnDisable()
    {
        isMoving = false;
    }
}
