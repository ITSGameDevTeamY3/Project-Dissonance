using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerTracker : MonoBehaviour
{
    #region Tracker Properties.
    // Public properties set in the inspector.
    public bool DrawLine;
    public Material DebugMaterial;
    public float Distance = 100;
    public bool UseTrackingDistance = false;

    // Public properties set automatically.
    public bool PlayerGlimpsed = false;
    public bool PlayerFound = false;
    public bool PlayerWithinView = false;
    public bool PlayerInSuspicionRange = false;
    public bool PlayerInAlertRange = false;
    public RaycastHit result, PlayerHit;
    public Vector3 PlayerGlimpsedPosition;

    // Private properties set automatically.
    EnemyController AI;
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.
    float trackingDistance, alertDistance;
    public LineRenderer LineOfSight;
    private Material[] debugLineMaterials;
    #endregion

    void Start()
    {
        LineOfSight = GetComponent<LineRenderer>();
        AI = transform.parent.gameObject.GetComponent<EnemyController>();
        trackablePlayerTransform = AI.Player.transform;
        trackingDistance = AI.SuspicionDistance;
        alertDistance = AI.AlertDistance;
    }

    void Update()
    {
        CheckPlayerRanges();

        if (PlayerInSuspicionRange && PlayerWithinView)
        {
            // Transform
            transform.LookAt(trackablePlayerTransform); // The enemy's eye will always look at the player if they are within the enemy's tracking distance and view.           
           
            DrawSightLineInDebug(0, transform.position); // Send out a LineOfSight from the enemy's eye.

            if (Physics.Raycast(transform.position, transform.forward, out result, Distance)) // If the LineOfSight hit something...
            {
                if (result.collider.tag != "Player") // If the LineOfSight doesn't hit the player...
                {
                    DrawSightLineInDebug(1, result.point); // Let the LineOfSight land on the collider that's been hit.
                    LineOfSight.enabled = false;
                    PlayerGlimpsed = false;
                }

                else // If the ray does hit the player...
                {
                    DrawSightLineInDebug(1, trackablePlayerTransform.position); // Let the LineOfSight land on the player.                   
                    LineOfSight.enabled = true; // Enable the LineOfSight once it has hit the player.
                                     
                    PlayerGlimpsed = true;

                    if (PlayerGlimpsed) // If the player has been glimpsed and we haven't captured the position the player was at...
                    {
                        PlayerGlimpsedPosition = trackablePlayerTransform.position; // Capture the player's position at that moment.                            
                    }
                }
            }
        }

        else // If the player is out of the enemy's tracking range.
        {
            if (PlayerGlimpsed) PlayerGlimpsed = false; // Reset this bool so the player's position can be recaptured if the enemy glimpses them again.

            LineOfSight.enabled = false; // Disable the line.
        }

        if (PlayerInAlertRange && PlayerWithinView && LineOfSight.enabled)
        {
            PlayerFound = true;
        }
    }

    private void CheckPlayerRanges()
    {
        if (Vector3.Distance(transform.position, trackablePlayerTransform.position) <= trackingDistance) PlayerInSuspicionRange = true;
        else PlayerInSuspicionRange = false;

        if (Vector3.Distance(transform.position, trackablePlayerTransform.position) <= alertDistance) PlayerInAlertRange = true;
        else PlayerInAlertRange = false;
    }

    private void DrawSightLineInDebug(int indexIn, Vector3 positionIn)
    {
        if (AI.DebugMode) LineOfSight.SetPosition(indexIn, positionIn); 
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