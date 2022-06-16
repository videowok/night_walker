using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotRover : Robot
{
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
    override protected void HandleDefaultUpdate()
    {
        base.HandleDefaultUpdate();

        // shoot at player

        ShootPlayer();
    }
}
