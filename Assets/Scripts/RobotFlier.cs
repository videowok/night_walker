using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITANCE
public class RobotFlier : Robot
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 2;
        HandleDefaultStart();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDefaultUpdate();
    }
}
