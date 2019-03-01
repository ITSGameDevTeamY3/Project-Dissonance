using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAimController : MonoBehaviour // Not working at the minute, attempting to get enemy weapon pointing at player.
{
    public EnemyController enemyAI; // Set in Inspector.

    private PlayerTracker playerTracker;
    private Transform playerTransform;

    void Start()
    {
        if (enemyAI != null)
        {
            playerTracker = enemyAI.PlayerTracker;
            playerTransform = enemyAI.Player.transform;
        }
    }

    void Update()
    {
        if (playerTracker.PlayerWithinView && playerTracker.PlayerInSuspicionRange)
        {
            transform.LookAt(playerTransform);
        }
    }
}