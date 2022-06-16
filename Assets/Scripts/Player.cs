using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Player : Robot
{
    private const float SHOT_DELAY = .5f;

    //[SerializeField] float turnSpeed = 180.0f;
    private float turnSpeed = 3.0f;

    private float horizontalInput;
    private float forwardInput;


    // Start is called before the first frame update
    void Start()
    {
        speed = 2;
        hitsToKill = 3;
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
        SetRandomPosition();
    }

    // POLYMORPHISM
    override protected void HandleDefaultUpdate()
    {
        // direction (mouse)

        horizontalInput = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, horizontalInput * turnSpeed);

        // strafe (left / right)
        float s = 0;

        if (Input.GetKey(KeyCode.A))
            s = -1;
        else if (Input.GetKey(KeyCode.D))
            s = 1;

        transform.position += (transform.right * (s * speed * Time.deltaTime));

        // movement (keys)

        forwardInput = Input.GetAxis("Vertical");
        float v = forwardInput * speed * Time.deltaTime;
        transform.position += transform.forward * v;

        // mark indexed pos

        mazeX = Maze.GetMazeIndexFromCoord(transform.position.x);
        mazeY = Maze.GetMazeIndexFromCoord(transform.position.z);   // z is world, y in maze (map)

        // shooting

        shotDelay -= Time.deltaTime;

        if (shotDelay <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                //Vector3 gunPos = transform.position;
                //gunPos.y += .25f;   // gun is higher

                FireShot(transform.position, transform.forward, Director.Instance.GetPlayerShotPrefab(), SHOT_DELAY, AudioManager.SFX.SHOT00);
            }
        }
    }

    // POLYMORPHISM
    override protected void HandleCollision(Collision col)
    {
        if (col.collider.tag.Equals(Director.SHOT_ENEMY_TAG))
        {
            --hitsToKill;

            if (hitsToKill <= 0)
            {
                Director.Instance.HandlePlayerKilled();
                BlowUp();
            }
            else
            {
                Damage();
            }
        }
        else if (col.collider.tag.Equals(Director.FLIER_TAG))
        {
            Director.Instance.HandlePlayerKilled();

            BlowUp();
        }
    }
}
