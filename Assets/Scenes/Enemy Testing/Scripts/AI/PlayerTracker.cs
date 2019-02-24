using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerTracker : MonoBehaviour
{
    #region Tracker Properties.
    // Public properties set in the inspector.
    public bool DrawLine;
    public float Distance = 100;
    public bool UseTrackingDistance = false;
    public Camera mainCamera;

    // Public properties set automatically.
    public bool PlayerGlimpsed = false;
    public bool PlayerFound = false;
    public bool PlayerWithinView = false;
    public bool PlayerPositionCaptured = false;
    public RaycastHit result, PlayerHit;
    public Vector3 PlayerGlimpsedPosition;

    // Private properties set automatically.
    EnemyController AI;
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.
    float trackingDistance, alertDistance;
    LineRenderer line;
    #endregion

    void Start()
    {
        line = GetComponent<LineRenderer>();
        AI = transform.parent.gameObject.GetComponent<EnemyController>();
        trackablePlayerTransform = AI.Player.transform;
        trackingDistance = AI.SuspicionDistance;
        alertDistance = AI.AlertDistance;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, trackablePlayerTransform.position) <= trackingDistance && PlayerWithinView)
        {
            // Transform
            transform.LookAt(trackablePlayerTransform); // The enemy's eye will always look at the player if they are within the enemy's tracking distance.

            if (DrawLine)
            {
                line.SetPosition(0, transform.position); // Send out a line from the enemy's eye.

                if (Physics.Raycast(transform.position, transform.forward, out result, Distance)) // If the line hit something...
                {
                    if (result.collider.tag != "Player") // If the line doesn't hit the player...
                    {
                        line.SetPosition(1, result.point); // Let the line land on the collider that's been hit.
                        line.enabled = false;
                    }

                    else // If the ray does hit the player...
                    {
                        line.SetPosition(1, trackablePlayerTransform.position); // Let the line land on the player.
                        line.enabled = true; // Enable the line once it has hit the player.
                        PlayerGlimpsed = true;

                        if (PlayerGlimpsed) // If the player has been glimpsed and we haven't captured the position the player was at...
                        {                           
                            PlayerGlimpsedPosition = trackablePlayerTransform.position; // Capture the player's position at that moment.
                            //PlayerPositionCaptured = true;
                        }
                    }
                }
            }                      
        }

        else // If the player is out of the enemy's tracking range.
        {
            if (PlayerGlimpsed) PlayerGlimpsed = false; // Reset this bool so the player's position can be recaptured if the enemy glimpses them again.

            line.enabled = false; // Disable the line.
        }


        if (Vector3.Distance(transform.position, trackablePlayerTransform.position) <= alertDistance && PlayerWithinView)
        {
            PlayerFound = true;
        }
    }

    private void OnDrawGizmos()
    {
        //if (gameObject.tag != "MainCamera" && gameObject.tag != "POV")
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, TrackingDistance);
        //}
    }
}