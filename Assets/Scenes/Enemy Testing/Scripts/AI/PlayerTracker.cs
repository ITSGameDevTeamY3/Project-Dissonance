using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerTracker : MonoBehaviour
{    
    public bool DrawLine;
    public float Distance = 100;
    public float TrackingDistance = 10;
    public bool UseTrackingDistance = false;

    EnemyController AI;
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.
    LineRenderer line;
    RaycastHit result;

    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        AI = GetComponent<EnemyController>();
        trackablePlayerTransform = AI.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Transform
        //transform.LookAt(trackablePlayerTransform);

        if (UseTrackingDistance && Vector3.Distance(transform.position, trackablePlayerTransform.position) <= TrackingDistance)
        {         
            if (DrawLine)
            {
                line.SetPosition(0, transform.position);

                if (Physics.Raycast(transform.position, transform.forward, out result, Distance))
                {
                    if (result.collider.tag != "Player")
                    {
                        line.SetPosition(1, result.point);
                        //line.enabled = false;
                    }

                    else
                    {
                        line.SetPosition(1, trackablePlayerTransform.position);
                        line.enabled = true;
                    }
                }
            }

            // Visualise line of sight.

            // Line Renderer

            // Positions

            // Z+
            // DoDrawline
        }
    }

    private void OnDrawGizmos()
    {
        if (gameObject.tag != "MainCamera")
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, TrackingDistance);
        }
    }
}
