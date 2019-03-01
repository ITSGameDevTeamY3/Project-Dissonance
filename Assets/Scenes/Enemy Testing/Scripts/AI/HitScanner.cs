using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HitScanner : MonoBehaviour
{
    #region Hit Scanner Properties.
    // Public properties set in the inspector.
    public EnemyController AI;
    public float Distance = 100;
    public bool UseTrackingDistance = false;

    // Public properties set automatically.    
    public RaycastHit result, PlayerHit;
    public bool Active = false;

    // Private properties set automatically.
    Transform trackablePlayerTransform; // This transform will be obtained from the enemy controller script.   
    DummyPlayerC player;
    public LineRenderer HitLine;
    #endregion

    void Start()
    {
        if (AI != null)
        {
            trackablePlayerTransform = AI.Player.transform;
            player = AI.Player.GetComponent<DummyPlayerC>();
        }
        HitLine = GetComponent<LineRenderer>();

    }

    void Update()
    {
        if (Active)
        {
            StopCoroutine("HitPlayer");
        }

        print(Active);

        // Transform
        transform.LookAt(trackablePlayerTransform); // The enemy's will always aim at the player if they are within the enemy's tracking distance and view.

        HitLine.SetPosition(0, transform.position); // Send out a HitLine from the enemy's eye.

        if (Physics.Raycast(transform.position, transform.forward, out result, Distance)) // If the HitLine hit something...
        {
            if (result.collider.tag != "Player") // If the HitLine doesn't hit the player...
            {
                HitLine.SetPosition(1, result.point); // Let the HitLine land on the collider that's been hit.
                HitLine.enabled = false;
            }

            else // If the ray does hit the player...
            {
                StartCoroutine("HitPlayer");
            }
        }
    }

    IEnumerator HitPlayer()
    {
        HitLine.SetPosition(1, trackablePlayerTransform.position); // Let the HitLine land on the player.
        HitLine.enabled = true; // Enable the HitLine once it has hit the player.  

        player.Health -= 1; // Take some health from the player.

        yield return new WaitForSeconds(0.5f); // The HitLine will appear for half a second, simulating a bullet trace.
        Active = false;
    }
}