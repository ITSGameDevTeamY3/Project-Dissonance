using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerTracker : MonoBehaviour
{    
    public bool DrawLine;
    public float Distance = 100;   
    public bool UseTrackingDistance = false;
    public bool PlayerGlimpsed = false;
    public bool PlayerFound = false;
    public bool PlayerWithinView = false;

    public Camera mainCamera;

    EnemyController AI;
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.
    float trackingDistance, alertDistance;
    LineRenderer line;
    public RaycastHit result, PlayerHit;


    public Vector3 PlayerGlimpsedPosition;
  
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
            transform.LookAt(trackablePlayerTransform);

            if (DrawLine)
            {
                line.SetPosition(0, transform.position);

                if (Physics.Raycast(transform.position, transform.forward, out result, Distance))
                {
                    if (result.collider.tag != "Player")
                    {
                        line.SetPosition(1, result.point);
                        line.enabled = false;
                    }

                    else
                    {
                        line.SetPosition(1, trackablePlayerTransform.position);
                        line.enabled = true;
                        PlayerGlimpsed = true;
                        Ray ray = mainCamera.ScreenPointToRay(trackablePlayerTransform.position); // I have to successfully capture the player's last known location.

                        if (Physics.Raycast(ray, out PlayerHit))
                        {
                            PlayerGlimpsedPosition = PlayerHit.point;
                        }
                    }
                }
            }
            // Visualise line of sight.
            // Line Renderer
            // Positions
            // Z+
            // DoDrawline
        }
        else line.enabled = false;

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