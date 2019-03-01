using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfViewController : MonoBehaviour { // This was the "SightJackTest" class, it is likely obsolete at this point.
    public Camera GameCam;
    private Camera POV;
    private EnemyController AI;
    private PlayerTracker playerTracker;
    private Transform playerTransform;

	void Start ()
    {        
        POV = GetComponent<Camera>();
        POV.enabled = false;
        AI = transform.parent.gameObject.GetComponent<EnemyController>();
        playerTracker = AI.playerTracker;
        playerTransform = AI.Player.transform;
        GameCam = AI.disturbanceCam;
    }
	
	void Update ()
    {
		if(Input.GetKeyUp(KeyCode.P) && AI.DebugMode) // NOTE: Remove this when we start to use David's sight-jacking instead. Also, this shouldn't be used when there's more than one enemy in the scene.
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