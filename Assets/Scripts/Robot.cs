//#define SHOW_MARKERS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Robot : MonoBehaviour
{
    private const float DEFAULT_SHOT_DELAY = 1.0f;
    private const float PLAYER_SAFE_RANGE = 5.0f;              // no enemy spawns closer

    protected const float TARGET_SHOT_RANGE = 1.5f;             // far aim range
    protected const float TARGET_AIMED_SHOT_RANGE = 5.0f;       // far near range

    protected const int TARGET_EVALUATION_STEPS_MIN = 4;        // (in grid points)
    protected const int TARGET_EVALUATION_STEPS_MAX = 10;       // (in grid points)
    protected const int RANDOM_EVALUATION_STEPS = 18;           // (in grid points)

    protected int mazeX;    // current grip pos
    protected int mazeY;

    private int mazeNextX;  // next grid pos
    private int mazeNextY;

    private float distanceToNext;
    private float distanceToNextTravelled;

    private int mazeFinalX; // final target grid pos
    private int mazeFinalY;

    protected float speed = 1;
    private Vector3 Velocity = new Vector3();

    protected float shotDelay = 0;
    protected int hitsToKill = 1;

    // movement path

    private int currentPathIndex;
    private int currentPathLength;
    private Maze.MAZE_DIRECTION[] currentPath = new Maze.MAZE_DIRECTION[Maze.PATH_LENGTH];

    protected int nextTargetEvaluation;   // in grid points

    // behavior

    protected enum BEHAVIOR
    {
        IDLE = 0,
        MOVING,

        COUNT
    };

    protected BEHAVIOR behavior = BEHAVIOR.IDLE;


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
    protected virtual void HandleDefaultStart()
    {
        if (!SetRandomPosition(GameObject.FindGameObjectWithTag(Director.PLAYER_TAG)))
            return;

        FindRandomDestination();

        nextTargetEvaluation = GetRetargetSteps(); // if target not found, get a new destination
    }

    // POLYMORPHISM
    protected virtual void HandleDefaultUpdate()
    {
        shotDelay -= Time.deltaTime;

        switch (behavior)
        {
            case BEHAVIOR.MOVING:

                HandleMove();

                break;

            default:    // IDLE

                break;
        }
    }

    // POLYMORPHISM (overloading)
    protected void FireShot(Vector3 pos, Vector3 dir, GameObject prefab, float delay, AudioManager.SFX sfx)
    {
        shotDelay = delay;

        GameObject gob = Instantiate(prefab);
        Shot shot = gob.GetComponent<Shot>();
        shot.Init(pos, dir, sfx);
    }

    // POLYMORPHISM (overloading)
    protected void FireShot(Vector3 pos, Vector3 dir)
    {
        FireShot(pos, dir, Director.Instance.GetEnemyShotPrefab(), DEFAULT_SHOT_DELAY, AudioManager.SFX.SHOT01);
    }

    // ABSTRACTION
    protected void ShowHit(Vector3 pos)
    {
        GameObject gob = Instantiate(Director.Instance.GetHitBigPrefab());
        HitBig hit = gob.GetComponent<HitBig>();
        hit.Init(pos);
    }

    // ABSTRACTION
    protected void ShowHitMedium(Vector3 pos)
    {
        GameObject gob = Instantiate(Director.Instance.GetHitMediumPrefab());
        HitMed hit = gob.GetComponent<HitMed>();
        hit.Init(pos);
    }

    // ABSTRACTION
    protected void BlowUp()
    {
        ShowHit(gameObject.transform.position);
        Destroy(gameObject);

        Director.Instance.audioManager.PlayExplosion();
    }

    // ABSTRACTION
    protected void Damage()
    {
        ShowHitMedium(gameObject.transform.position);

        Director.Instance.audioManager.PlayExplosion(.5f);
    }

    // POLYMORPHISM
    protected virtual void HandleCollision(Collision col)
    {
        if (col.collider.tag.Equals(Director.SHOT_PLAYER_TAG))
        {
            --hitsToKill;

            if (hitsToKill <= 0)
                BlowUp();
            else
                Damage();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("ROBOT hit " + col.collider.name);

        HandleCollision(col);
    }

    // ABSTRACTION
    protected void SetPositionFromIndex()
    {
        Vector3 pos = Maze.GetMazePositionFromIndex(mazeX, mazeY);
        pos.y = gameObject.transform.position.y;    // restore y
        gameObject.transform.position = pos;
    }

    // POLYMORPHISM
    public virtual void HandleArrival()
    {
        RemoveMarkers();
        FindRandomDestination();
    }

    // POLYMORPHISM
    protected virtual int GetRetargetSteps()
    {
        return RANDOM_EVALUATION_STEPS;
    }

    // POLYMORPHISM
    protected bool ShouldRetarget()
    {
        --nextTargetEvaluation;

        if (nextTargetEvaluation > 0)
        {
            return false;
        }
        else
        {
            nextTargetEvaluation = GetRetargetSteps();
            return true;
        }
    }

    // ABSTRACTION
    protected void HandleMove()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 dVel = Velocity * Time.deltaTime;

        pos += dVel;

        distanceToNextTravelled += Vector3.Magnitude(dVel);

        gameObject.transform.position = pos;

        if (distanceToNextTravelled >= distanceToNext)  // arrived at next grid point
        {
            mazeX = mazeNextX;
            mazeY = mazeNextY;
            SetPositionFromIndex();

            if ((mazeX == mazeFinalX) && (mazeY == mazeFinalY))     // arrived at target?
            {
                HandleArrival();
                return;
            }

            if (ShouldRetarget())       // hunters: evaluate new path to target
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

    // ABSTRACTION
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

    // ABSTRACTION
    protected void CreateTargetPath()
    {
        currentPathLength = Director.Instance.maze.FindBestPath(mazeX, mazeY, mazeFinalX, mazeFinalY, currentPath);
        currentPathIndex = 1;   // 0 is current pos

        MoveToNextPosition();

        ShowMarkers();
    }

    // ABSTRACTION
    protected bool SetRandomPosition()
    {
        int[] testPos = Director.Instance.maze.GetEmptySpaceInMaze();

        if (testPos == null)
            return false;

        mazeX = testPos[0];
        mazeY = testPos[1];
        SetPositionFromIndex();

        return true;
    }

    // POLYMORPHISM (overloading)
    protected bool SetRandomPosition(GameObject player)
    {
        if (player == null)
            return SetRandomPosition();

        bool bDistOk;   // don't spawn too close to player

        do
        {
            bool bOk = SetRandomPosition();
            if (!bOk)
                return false;

            Vector3 d = player.transform.position - gameObject.transform.position;
            bDistOk = d.magnitude >= PLAYER_SAFE_RANGE ? true : false;

        } while (!bDistOk);

        return true;
    }

    // ABSTRACTION
    protected void FindRandomDestination()
    {
        int[] testPos = Director.Instance.maze.GetEmptySpaceInMaze();

        mazeFinalX = testPos[0];
        mazeFinalY = testPos[1];

#if SHOW_MARKERS
        // show destination marker
        //maze.ShowTargetMarker(mazeFinalX, mazeFinalY);
#endif

        CreateTargetPath();
    }

    // ABSTRACTION
    protected bool FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Director.PLAYER_TAG);

        if (player == null)
            return false;

        Player p = player.GetComponent<Player>();

        if (p == null)
            return false;

        mazeFinalX = p.mazeX;
        mazeFinalY = p.mazeY;

        CreateTargetPath();

        return true;
    }

    // ABSTRACTION
    protected void ShootPlayer()
    {
        // shoot at player?

        if (shotDelay > 0)
            return;

        GameObject player = GameObject.FindGameObjectWithTag(Director.PLAYER_TAG);

        if (player == null)
            return;

        // shoot if possible

        Vector3 d = player.transform.position - gameObject.transform.position;

        if (d.magnitude > TARGET_AIMED_SHOT_RANGE)  // far shot -> only up/down/left/right shots
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) < TARGET_SHOT_RANGE)
            {
                FireShot(transform.position, player.transform.position.x < transform.position.x ? Vector3.left : Vector3.right);
            }
            else if (Mathf.Abs(player.transform.position.x - transform.position.x) < TARGET_SHOT_RANGE)
            {
                FireShot(transform.position, player.transform.position.z < transform.position.z ? Vector3.back : Vector3.forward);
            }
        }
        else
        {
            // near shot - aim

            d.Normalize();
            FireShot(transform.position, d);
        }
    }


    //------------------------------------------------
    //
    // DEBUG / TEST CODE

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
}
