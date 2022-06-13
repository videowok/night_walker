using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Player : Robot
{
    private const float SHOT_DELAY = .5f;

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
        HandleDefaultUpdate();
    }

    // POLYMORPHISM
    override protected void HandleDefaultUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        if (!Input.GetKey(KeyCode.LeftShift))
        {   // rotate
            transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);
        }
        else
        {   // strafe
            float s = horizontalInput * speed * Time.deltaTime;
            transform.position += transform.right * s;
        }

        float v = forwardInput * speed * Time.deltaTime;
        transform.position += transform.forward * v;

        // mark indexed pos

        mazeX = Maze.GetMazeIndexFromCoord(transform.position.x);
        mazeY = Maze.GetMazeIndexFromCoord(transform.position.z);   // z is world, y in maze (map)

        // shooting

        shotDelay -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && shotDelay <= 0)
        {
            Vector3 gunPos = transform.position;
            gunPos.y += .25f;   // gun is higher

            FireShot(gunPos, transform.forward, SHOT_DELAY, AudioManager.SFX.SHOT00);
        }
    }

    // POLYMORPHISM
    override protected void HandleGettingHit(Collision col)
    {
        if (col.collider.tag.Equals("ShotTag"))
        {
            Director.Instance.HandlePlayerKilled();

            BlowUp();
        }
    }
}
