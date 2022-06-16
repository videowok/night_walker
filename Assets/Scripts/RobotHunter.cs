using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotHunter : Robot
{
    // Start is called before the first frame update
    void Start()
    {
        hitsToKill = 2;
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
        if (!SetRandomPosition(GameObject.FindGameObjectWithTag(Director.PLAYER_TAG)))
            return;

        HandleArrival();    // also starts moving

        shotDelay = Random.Range(3, 6); // don't shoot right away, or all at the same time

        nextTargetEvaluation = GetRetargetSteps();
    }

    // POLYMORPHISM
    override protected void HandleDefaultUpdate()
    {
        base.HandleDefaultUpdate();

        // shoot at player

        ShootPlayer();
    }

    // POLYMORPHISM
    override protected int GetRetargetSteps()
    {
        return Random.Range(TARGET_EVALUATION_STEPS_MIN, TARGET_EVALUATION_STEPS_MAX + 1);
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
    override protected void HandleCollision(Collision col)
    {
        if (col.collider.tag.Equals(Director.SHOT_PLAYER_TAG))
        {
            --hitsToKill;

            if (hitsToKill <= 0)
            {
                Director.Instance.AddScore(500);
                BlowUp();
            }
            else
            {
                Damage();
            }
        }
    }
}
