using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Player : Robot
{
    [SerializeField] float turnSpeed = 180.0f;

    private float horizontalInput;
    private float forwardInput;

    // Start is called before the first frame update
    void Start()
    {
        speed = 2;
        HandleDefaultStart();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);

        float v = forwardInput * speed * Time.deltaTime;
        transform.position += transform.forward * v;

        // mark indexed pos

        mazeX = Maze.GetMazeIndexFromCoord(transform.position.x);
        mazeY = Maze.GetMazeIndexFromCoord(transform.position.z);   // z is world, y in maze (map)
    }
}
