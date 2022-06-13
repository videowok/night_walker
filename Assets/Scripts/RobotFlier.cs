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
    override protected void HandleGettingHit(Collision col)
    {
        if (col.collider.tag.Equals("ShotTag"))
        {
            Director.Instance.AddScore(100);
            BlowUp();
        }
    }
}
