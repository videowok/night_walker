using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotHunter : Robot
{
    private const float TARGET_SHOT_RANGE = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        HandleDefaultStart();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDefaultUpdate();
    }

    // POLYMORPHISM
    override protected void HandleDefaultStart()
    {
        maze = Director.Instance.maze;

        behavior = BEHAVIOR.IDLE;

        if (!SetRandomPosition())
            return;

        HandleArrival();    // also starts moving

        shotDelay = Random.Range(3, 6); // don't shoot right away, or all at the same time
    }

    // POLYMORPHISM
    override protected void HandleDefaultUpdate()
    {
        base.HandleDefaultUpdate();

        // shoot at player

        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");

        if (player == null)
            return;

        if (shotDelay > 0)
            return;

        // shoot
        
        if (Mathf.Abs(player.transform.position.z - transform.position.z) < TARGET_SHOT_RANGE)
        {
            FireShot(transform.position, player.transform.position.x < transform.position.x ? Vector3.left : Vector3.right);
        }
        else if (Mathf.Abs(player.transform.position.x - transform.position.x) < TARGET_SHOT_RANGE)
        {
            FireShot(transform.position, player.transform.position.z < transform.position.z ? Vector3.back : Vector3.forward);
        }
    }

    // POLYMORPHISM
    override public void HandleArrival()
    {
        RemoveMarkers();

        if (!FindPlayer())
        {
            FindRandomDestination();
        }
    }

    // POLYMORPHISM
    override protected void HandleGettingHit(Collision col)
    {
        if (col.collider.tag.Equals("ShotTag"))
        {
            Director.Instance.AddScore(500);
            BlowUp();
        }
    }
}
