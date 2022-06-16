using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotFlier : Robot
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 3;
        HandleDefaultStart();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDefaultUpdate();
    }

    // POLYMORPHISM
    override protected void HandleCollision(Collision col)
    {
        if (col.collider.tag.Equals(Director.SHOT_PLAYER_TAG))
        {
            Director.Instance.AddScore(100);    // killed by player
            BlowUp();
        }
        else if (col.collider.tag.Equals(Director.PLAYER_TAG))
        {
            BlowUp();
        }
    }
}
