using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private Maze maze;  // 2DO: use director instance

    private int mazeX;      // current grip pos
    private int mazeY;

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

    private Vector3 Velocity = new Vector3();
    
    private enum BEHAVIOR
    {
        IDLE = 0,
        MOVING,

        COUNT
    };

    private BEHAVIOR behavior;


    // Start is called before the first frame update
    void Start()
    {
        maze = GameObject.Find("/Maze").GetComponent<Maze>();

        behavior = BEHAVIOR.IDLE;

        int[] testPos = maze.GetEmptySpaceInMaze();

        if (testPos != null)
        {
            mazeX = testPos[0];
            mazeY = testPos[1];

            Vector3 pos = Maze.GetMazePositionFromIndex(mazeX, mazeY);
            pos.y = gameObject.transform.position.y;
            gameObject.transform.position = pos;

            FindRandomDestination();
        }
    }

    // Update is called once per frame
    void Update()
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

    private void HandleMove()
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

            pos = Maze.GetMazePositionFromIndex(mazeX, mazeY);
            pos.y = gameObject.transform.position.y;
            gameObject.transform.position = pos;

            if ((mazeX == mazeFinalX) && (mazeY == mazeFinalY))     // arrived at target?
            {
                maze.RemoveTargetMarkers();
                maze.RemoveTrailMarkers();
                FindRandomDestination();
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

    private void MoveToNextPosition()
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

        Velocity = Vector3.Normalize(toDest);
    }

    private void CreateTargetPath()
    {
        currentPathLength = maze.FindBestPath(mazeX, mazeY, mazeFinalX, mazeFinalY, currentPath);
        currentPathIndex = 1;   // 0 is current pos

        MoveToNextPosition();

        // show path
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

    private void FindRandomDestination()
    {
        int[] testPos = maze.GetEmptySpaceInMaze();

        mazeFinalX = testPos[0];
        mazeFinalY = testPos[1];

        // show test marker
        maze.ShowTargetMarker(mazeFinalX, mazeFinalY);

        CreateTargetPath();
    }
}
