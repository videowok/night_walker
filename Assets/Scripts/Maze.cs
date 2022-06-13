using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public const int PATH_LENGTH = 10;

    private const float MAZE_SCALE = 1.0f;

    private const int MAZE_SIZE_X = 30;
    private const int MAZE_SIZE_Y = 30;

    [SerializeField] int TUNNEL_SECTIONS = 30;
    [SerializeField] int TUNNEL_MARGIN = 6;
    [SerializeField] int TUNNEL_SIZE = 8;

    [SerializeField] GameObject wallBlockPrefab;

    // TEST
    [SerializeField] GameObject trailMarkerPrefab;
    [SerializeField] GameObject targetMarkerPrefab;


    private int[,]  mapArray;

    private List<int[]> freeSpaceList = new List<int[]>();


    // path

    private const float LONG_DISTANCE = 10000;

    private int bestPathSteps;
    private int[,] testPathUsedSteps = new int[PATH_LENGTH, 2];

    public enum MAZE_DIRECTION
    {
        UP = 0,
        UP_RIGHT,
        RIGHT,
        DOWN_RIGHT,
        DOWN,
        DOWN_LEFT,
        LEFT,
        UP_LEFT,

        COUNT,

        NONE,
    };

    private static int[,] mazeDirection =  // indexed by MAZE_DIRECTION
    {
        {0, 1},     // UP
        {1, 1},     // UP_RIGHT
        {1, 0},     // RIGHT
        {1, -1},    // DOWN_RIGHT
        {0, -1},    // DOWN
        {-1, -1},   // DOWN_LEFT
        {-1, 0},    // LEFT
        {-1, 1}     // UP_LEFT
    };


    // Start is called before the first frame update
    void Start()
    {
        mapArray = new int[MAZE_SIZE_X, MAZE_SIZE_Y];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    static public Vector3 GetMazeCenterPosition()
    {
        return new Vector3(MAZE_SIZE_X / 2 * MAZE_SCALE, 0, MAZE_SIZE_Y / 2 * MAZE_SCALE);
    }

    // 2DO: ADD BOUNDARY CHECK
    static public Vector3 GetMazePositionFromIndex(int x, int y)
    {
        return new Vector3(x * MAZE_SCALE, 0, y * MAZE_SCALE);
    }

    static public int GetMazeDX(MAZE_DIRECTION dir)
    {
        return mazeDirection[(int)dir, 0];
    }
    static public int GetMazeIndexFromCoord(float xy)
    {
        return (int)Mathf.Round(xy / MAZE_SCALE);
    }

    static public int GetMazeDY(MAZE_DIRECTION dir)
    {
        return mazeDirection[(int)dir, 1];
    }

    private bool IsEmpty(int x, int y)
    {
        if ((x < 0) || (x >= MAZE_SIZE_X))
            return false;

        if ((y < 0) || (y >= MAZE_SIZE_Y))
            return false;

        return mapArray[x, y] == 0;
    }

    private bool CanTravel(int x, int y, MAZE_DIRECTION dir)
    {
        int d = (int)dir;

        if (IsEmpty(x + GetMazeDX(dir), y + GetMazeDY(dir)))
        {
            // for diagonals, let's not go through walls on the way

            switch (dir)
            {
                case MAZE_DIRECTION.UP_RIGHT:

                    return ((mapArray[x, y + 1] == 0)  && (mapArray[x + 1, y] == 0)) ? true : false;

                case MAZE_DIRECTION.DOWN_RIGHT:

                    return ((mapArray[x, y - 1] == 0) && (mapArray[x + 1, y] == 0)) ? true : false;

                case MAZE_DIRECTION.DOWN_LEFT:

                    return ((mapArray[x, y - 1] == 0) && (mapArray[x - 1, y] == 0)) ? true : false;

                case MAZE_DIRECTION.UP_LEFT:

                    return ((mapArray[x, y + 1] == 0) && (mapArray[x - 1, y] == 0)) ? true : false;

                default:

                    return true;
            }
        }

        return false;
    }

    public int[] GetEmptySpaceInMaze()
    {
        return freeSpaceList.Count == 0 ? null :freeSpaceList[ Random.Range(0, freeSpaceList.Count) ];
    }

    private void CarveBlock(int x, int y)
    {
        x = Clamp(x, 1, MAZE_SIZE_X - 2);
        y = Clamp(y, 1, MAZE_SIZE_Y - 2);

        //if ((x > 0) && (x <= MAZE_SIZE_X - 2) && (y > 0) && (y <= MAZE_SIZE_Y - 2))
        {
            mapArray[x, y] = 0;
        }
    }

    private void CarveTunnel(int xPos, int yPos)
    {
        for (int i = 0; i <= TUNNEL_SIZE; i++)
        {
            CarveBlock(xPos + i, yPos);
            CarveBlock(xPos + i, yPos + TUNNEL_SIZE);
            CarveBlock(xPos, yPos + i);
            CarveBlock(xPos + TUNNEL_SIZE, yPos + i);
        }
    }

    public void BuildMaze()
    {
        Vector3 pos = new Vector3(0, wallBlockPrefab.transform.position.y, 0);
        GameObject block;

        // make everything wall

        for (int y = 0; y < MAZE_SIZE_Y; y++)
            for (int x = 0; x < MAZE_SIZE_X; x++)
                mapArray[x, y] = 1;

        // carve tunnels

        for (int i = 0; i < TUNNEL_SECTIONS; i++)
        {
            int x = Random.Range(-TUNNEL_MARGIN, MAZE_SIZE_X - TUNNEL_SIZE - 1 + TUNNEL_MARGIN);
            int y = Random.Range(-TUNNEL_MARGIN, MAZE_SIZE_X - TUNNEL_SIZE - 1 + TUNNEL_MARGIN);

            CarveTunnel(x, y);
        }

        // add wall prefabs

        for (int y = 0; y < MAZE_SIZE_Y; y++)
        {
            for (int x = 0; x < MAZE_SIZE_X; x++)
            {
                if (mapArray[x, y] == 1)
                {
                    pos.x = x * MAZE_SCALE;
                    pos.z = y * MAZE_SCALE;

                    block = Instantiate(wallBlockPrefab);
                    block.transform.position = pos;
                }
                else // add to free space list (for spawning)
                {
                    freeSpaceList.Add(new int[] {x, y});
                }
            }
        }
    }

    private void CachePathPosition(int[,] path, int index, int x, int y)
    {
        path[index, 0] = x;
        path[index, 1] = y;
    }

    // have we already travelled on this point?
    private bool HasTraveled(int x, int y, int pathIndex)
    {
        for (int i = 0; i < pathIndex; i++)
        {
            if ((testPathUsedSteps[i, 0] == x) && (testPathUsedSteps[i, 1] == y))
                return true;
        }

        return false;
    }

    private void CreatePath(int x, int y, int destX, int destY, MAZE_DIRECTION[] path, int pathIndex)
    {
        MAZE_DIRECTION bestDirection = MAZE_DIRECTION.NONE;
        float bestDistance = LONG_DISTANCE;
        int bestX = 0;
        int bestY = 0;

        for (MAZE_DIRECTION dir = MAZE_DIRECTION.UP; dir < MAZE_DIRECTION.COUNT; dir++)
        {
            int testX = x + GetMazeDX(dir);
            int testY = y + GetMazeDY(dir);

            if (CanTravel(x, y, dir) && !HasTraveled(testX, testY, pathIndex))
            {
                Vector2 v0 = new Vector2(testX, testY);
                Vector2 v1 = new Vector2(destX, destY);
                Vector2 vd = v1 - v0;
                float dist = vd.magnitude;

                if (dist < bestDistance)
                {
                    bestDirection = dir;
                    bestDistance = dist;
                    bestX = testX;
                    bestY = testY;
                }
            }
        }

        if (bestDirection == MAZE_DIRECTION.NONE)
            return;

        path[pathIndex] = bestDirection;
        CachePathPosition(testPathUsedSteps, pathIndex, bestX, bestY);

        if (((pathIndex + 1) >= PATH_LENGTH) || ((bestX == destX) && (bestY == destY)))
        {
            bestPathSteps = pathIndex + 1;  // get correct path length
            return;
        }

        CreatePath(bestX, bestY, destX, destY, path, pathIndex + 1);   // recursive
    }

    public int FindBestPath(int x, int y, int destX, int destY, MAZE_DIRECTION[] path)
    {
        CachePathPosition(testPathUsedSteps, 0, x, y);  // starting pos is 0
        path[0] = MAZE_DIRECTION.NONE;                  // ... (no direction)
        bestPathSteps = 1;                              // ... (start from 1)

        CreatePath(x, y, destX, destY, path, 1);        // recursive

        //OutputPath(path, bestPathSteps);

        return bestPathSteps;
    }



    //------------------------------------------------
    //
    // DEBUG / TEST CODE

    /*
        private void OutputPath(MAZE_DIRECTION[] path, int steps)
        {
            string test = "PATH found: ";

            for (int i = 1; i < steps; i++) // 0 is starting pos
            {
                test += path[i] + " * ";
            }

            Debug.Log(test);
        }
    */

    public void ShowTargetMarker(int x, int y)
    {
        GameObject gobMarker = Instantiate(targetMarkerPrefab);
        
        Vector3 pos = Maze.GetMazePositionFromIndex(x, y);

        pos.y = gobMarker.transform.position.y;

        gobMarker.transform.position = pos;
    }

    public void ShowPathMarker(int x, int y)
    {
        GameObject gobMarker = Instantiate(trailMarkerPrefab);

        Vector3 pos = Maze.GetMazePositionFromIndex(x, y);

        pos.y = gobMarker.transform.position.y;

        gobMarker.transform.position = pos;
    }

    public void RemoveTargetMarkers()
    {
        GameObject[] gobs = GameObject.FindGameObjectsWithTag("TargetMarker");
        foreach (GameObject obj in gobs)
            Destroy(obj);
    }

    public void RemoveTrailMarkers()
    {
        GameObject[] gobs = GameObject.FindGameObjectsWithTag("TrailMarker");
        foreach (GameObject obj in gobs)
            Destroy(obj);
    }

    /*
    private GameObject GetBlockByType(string type)
    {
        if (type.Equals("tmetal1"))
            return wallBlockPrefab;

        if (type.Equals("twallh1"))
            return wallXPrefab;

        if (type.Equals("twallv1"))
            return wallZPrefab;

        return null;
    }
    */
}
