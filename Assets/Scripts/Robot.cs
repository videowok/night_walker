#define SHOW_MARKERS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    protected Maze maze;  // 2DO: use director instance

    protected int mazeX;    // current grip pos
    protected int mazeY;

    private int mazeNextX;  // next grid pos
    private int mazeNextY;

    private float distanceToNext;
    private float distanceToNextTravelled;

    private int mazeFinalX; // final target grid pos
    private int mazeFinalY;

    // movement path

    private int currentPathIndex;
    private int currentPathLength;
    private Maze.MAZE_DIRECTION[] currentPath = new Maze.MAZE_DIRECTION[Maze.PATH_LENGTH];

    public float speed = 1;

    private Vector3 Velocity = new Vector3();

    protected enum BEHAVIOR
    {
        IDLE = 0,
        MOVING,

        COUNT
    };

    protected BEHAVIOR behavior;


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

    public virtual void HandleDefaultStart()
    {
        maze = GameObject.Find("/Maze").GetComponent<Maze>();

        behavior = BEHAVIOR.IDLE;

        if (SetRandomPosition())
        {
            FindRandomDestination();
        }
    }

    public virtual void HandleDefaultUpdate()
    {
        switch (behavior)
        {
            case BEHAVIOR.MOVING:

                HandleMove();

                break;

            default:    // IDLE

                break;
        }
    }

    protected void ShowMarkers()
    {
#if SHOW_MARKERS
        if (gameObject.tag.Equals("Hunter"))
        {
            // show path markers
            int x = mazeX;
            int y = mazeY;

            for (int i = currentPathIndex; i < currentPathLength; i++)
            {
                x += Maze.GetMazeDX(currentPath[i]);
                y += Maze.GetMazeDY(currentPath[i]);

                //Debug.Log("Path entry: " + currentPath[i]);

                // test marker
                maze.ShowPathMarker(x, y);
            }
        }
#endif
    }

    protected void RemoveMarkers()
    {
#if SHOW_MARKERS
        if (gameObject.tag.Equals("Hunter"))
        {
            maze.RemoveTargetMarkers();
            maze.RemoveTrailMarkers();
        }
#endif
    }

    protected void SetPositionFromIndex()
    {
        Vector3 pos = Maze.GetMazePositionFromIndex(mazeX, mazeY);
        pos.y = gameObject.transform.position.y;    // restore y
        gameObject.transform.position = pos;
    }

    public virtual void HandleArrival()
    {
        RemoveMarkers();
        FindRandomDestination();
    }

    protected void HandleMove()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 dVel = Velocity * Time.deltaTime;

        pos += dVel;

        distanceToNextTravelled += Vector3.Magnitude(dVel);

        gameObject.transform.position = pos;

        if (distanceToNextTravelled >= distanceToNext)
        {
            mazeX = mazeNextX;
            mazeY = mazeNextY;
            SetPositionFromIndex();

            if ((mazeX == mazeFinalX) && (mazeY == mazeFinalY))     // arrived at target?
            {
                HandleArrival();
                return;
            }

            currentPathIndex++;
            if (currentPathIndex >= currentPathLength)
            {
                CreateTargetPath();
                return;
            }

            MoveToNextPosition();
        }
    }

    protected void MoveToNextPosition()
    {
        behavior = BEHAVIOR.MOVING;

        Maze.MAZE_DIRECTION dir = currentPath[currentPathIndex];

        mazeNextX = mazeX + Maze.GetMazeDX(dir);
        mazeNextY = mazeY + Maze.GetMazeDY(dir);

        Vector3 pos = Maze.GetMazePositionFromIndex(mazeX, mazeY);
        Vector3 dest = Maze.GetMazePositionFromIndex(mazeNextX, mazeNextY);
        Vector3 toDest = dest - pos;

        distanceToNextTravelled = 0;
        distanceToNext = Vector3.Magnitude(toDest);

        Velocity = Vector3.Normalize(toDest) * speed;
    }

    protected void CreateTargetPath()
    {
        currentPathLength = maze.FindBestPath(mazeX, mazeY, mazeFinalX, mazeFinalY, currentPath);
        currentPathIndex = 1;   // 0 is current pos

        MoveToNextPosition();

        ShowMarkers();
    }

    protected bool SetRandomPosition()
    {
        int[] testPos = maze.GetEmptySpaceInMaze();

        if (testPos != null)
        {
            mazeX = testPos[0];
            mazeY = testPos[1];
            SetPositionFromIndex();

            return true;
        }

        return false;
    }

    protected void FindRandomDestination()
    {
        int[] testPos = maze.GetEmptySpaceInMaze();

        mazeFinalX = testPos[0];
        mazeFinalY = testPos[1];

#if SHOW_MARKERS
        // show destination marker
        //maze.ShowTargetMarker(mazeFinalX, mazeFinalY);
#endif

        CreateTargetPath();
    }

    protected bool FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");

        if (player == null)
            return false;

        Player p = player.GetComponent<Player>();

        if (p == null)
            return false;

        mazeFinalX = p.mazeX;
        mazeFinalY = p.mazeY;

        //Debug.Log("Player found");

        CreateTargetPath();

        return true;
    }
}
