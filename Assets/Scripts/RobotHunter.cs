using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotHunter : Robot
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
    override public void HandleDefaultStart()
    {
        maze = GameObject.Find("/Maze").GetComponent<Maze>();

        behavior = BEHAVIOR.IDLE;

        if (!SetRandomPosition())
            return;

        HandleArrival();
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
}
