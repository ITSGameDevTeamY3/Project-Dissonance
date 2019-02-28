using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HitScanner : MonoBehaviour
{
    #region Hit Scanner Properties.
    // Public properties set in the inspector.
    public EnemyController AI;
    public bool DrawLine;
    public float Distance = 100;
    public bool UseTrackingDistance = false;

    // Public properties set automatically.    
    public RaycastHit result, PlayerHit;
    public bool Active;

    // Private properties set automatically.
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.    
    public LineRenderer LineOfSight;
    #endregion

    void Start()
    {
        LineOfSight = GetComponent<LineRenderer>();
        if (AI != null) trackablePlayerTransform = AI.Player.transform;
    }

    void Update()
    {
        if (!Active) return;

        // Transform
        transform.LookAt(trackablePlayerTransform); // The enemy's eye will always look at the player if they are within the enemy's tracking distance and view.

        LineOfSight.SetPosition(0, transform.position); // Send out a LineOfSight from the enemy's eye.

        if (Physics.Raycast(transform.position, transform.forward, out result, Distance)) // If the LineOfSight hit something...
        {
            if (result.collider.tag != "Player") // If the LineOfSight doesn't hit the player...
            {
                LineOfSight.SetPosition(1, result.point); // Let the LineOfSight land on the collider that's been hit.
                LineOfSight.enabled = false;                
            }

            else // If the ray does hit the player...
            {
                LineOfSight.SetPosition(1, trackablePlayerTransform.position); // Let the LineOfSight land on the player.
                LineOfSight.enabled = true; // Enable the LineOfSight once it has hit the player.                
            }
        }
    }    
}