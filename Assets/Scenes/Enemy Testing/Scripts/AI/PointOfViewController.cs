using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfViewController : MonoBehaviour {
    public Camera GameCam;
    private Camera POV;
    private EnemyController enemy;
    private PlayerTracker playerTracker;
    private Transform playerTransform;

	void Start ()
    {
        POV = GetComponent<Camera>();
        POV.enabled = false;
        enemy = transform.parent.gameObject.GetComponent<EnemyController>();
        playerTracker = enemy.playerTracker;
        playerTransform = enemy.Player.transform;
    }
	
	void Update ()
    {
		if(Input.GetKeyUp(KeyCode.P)) // NOTE: Remove this when we start to use David's sight-jacking instead.
        {
            GameCam.enabled = !GameCam.enabled;
            POV.enabled = !POV.enabled;
        }

        //if(playerTracker.PlayerFound && playerTracker.PlayerWithinView && playerTracker.PlayerInSuspicionRange)
        //{
        //    transform.LookAt(playerTransform);
        //}
	}
}