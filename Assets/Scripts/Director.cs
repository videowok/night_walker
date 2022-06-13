using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    public static Director Instance { get; private set; }    // ENCAPSULATION
    public AudioManager audioManager { get; private set; }    // ENCAPSULATION
    public CameraManager cameraManager { get; private set; }    // ENCAPSULATION
    public Maze maze { get; private set; }   // ENCAPSULATION

    // ui

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Button startGame;
    [SerializeField] Button switchCam;
    [SerializeField] Button newMaze;

    // game

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject robotFlierPrefab;
    [SerializeField] GameObject robotHunterPrefab;
    [SerializeField] GameObject shotPrefab;
    [SerializeField] GameObject hitPrefab;
    [SerializeField] GameObject hitBigPrefab;

    private const float SPAWN_INTERVAL = 1;

    private int maxHunters = 2;
    private int maxFliers = 4;
    private float spawnInterval = 0;

    private enum GAMEMODE
    {
        DEMO = 0,
        GAME,
        PLAYER_SPWAN_INTERVAL,
        GAME_OVER
    };

    private GAMEMODE gameMode;

    private const float PLAYER_KILLED_INTERVAL = 2;
    private const float GAME_OVER_INTERVAL = 3;

    private float timer;
    private int score = 0;
    private int highScore = 0;
    private int lives = 0;

    public int updateCount;



    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioManager = GameObject.Find("/AudioManager").GetComponent<AudioManager>();

        cameraManager = GameObject.Find("/Main Camera").GetComponent<CameraManager>();

        maze = GameObject.Find("/Maze").GetComponent<Maze>();
        maze.BuildMaze();

        StartDemo();
    }

    // Update is called once per frame
    void Update()
    {
        updateCount++;

        switch (gameMode)
        {
            case GAMEMODE.DEMO:
            case GAMEMODE.GAME:

                spawnInterval -= Time.deltaTime;

                if (spawnInterval <= 0)
                //if (false)
                {
                    spawnInterval = SPAWN_INTERVAL;

                    GameObject[] gobs;

                    gobs = GameObject.FindGameObjectsWithTag("Hunter");

                    for (int i = 0; i < maxHunters - gobs.Length; i++)
                        Instantiate(robotHunterPrefab);

                    gobs = GameObject.FindGameObjectsWithTag("Flier");

                    for (int i = 0; i < maxFliers - gobs.Length; i++)
                        Instantiate(robotFlierPrefab);
                }

                break;

            case GAMEMODE.PLAYER_SPWAN_INTERVAL:

                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    ContinueGame();
                }

                break;

            case GAMEMODE.GAME_OVER:

                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    StartDemo();
                }

                break;

            default: break;
        }
    }

    private void SetGameMode(GAMEMODE mode)
    {
        gameMode = mode;
    }

    private void RemoveObjectsWithTag(string tag)
    {
        GameObject[] gobs = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in gobs)
            Destroy(obj);
    }

    private void ClearAllDynamicObjects()
    {
        RemoveObjectsWithTag("PlayerTag");
        RemoveObjectsWithTag("Hunter");
        RemoveObjectsWithTag("Flier");
        RemoveObjectsWithTag("ShotTag");
    }

    public void RebuildMaze()
    {
        ClearAllDynamicObjects();
        RemoveObjectsWithTag("WallTag");

        maze.BuildMaze();

        StartDemo();
    }

    public void StartGame()
    {
        SetGameMode(GAMEMODE.GAME);

        ClearAllDynamicObjects();

        SetTitleVisibility(false);
        SetGameOverVisibility(false);
        SetNewMazeButtonVisibility(false);
        SetSwitchCameraButtonVisibility(true);

        SetScore(0);
        SetLives(3);

        Instantiate(playerPrefab);

        cameraManager.SetType(CameraManager.TYPE.ISOMETRIC);    // 2DO - last user camera

        //audioManager.Play(AudioManager.SFX.GAME_START);
    }

    private void ContinueGame()
    {
        ClearAllDynamicObjects();
        Instantiate(playerPrefab);
        spawnInterval = SPAWN_INTERVAL;
        SetGameMode(GAMEMODE.GAME);

        //audioManager.Play(AudioManager.SFX.GAME_START);
    }

    public void StartDemo()
    {
        SetGameMode(GAMEMODE.DEMO);

        SetTitleVisibility(true);
        SetGameOverVisibility(false);
        SetNewMazeButtonVisibility(true);
        SetSwitchCameraButtonVisibility(false);

        cameraManager.SetType(CameraManager.TYPE.DEMO);
    }

    public void SetGameOver()
    {
        //audioManager.Play(AudioManager.SFX.GAME_START);

        timer = GAME_OVER_INTERVAL;
        SetGameMode(GAMEMODE.GAME_OVER);
        SetGameOverVisibility(true);
    }

    public void HandlePlayerKilled()
    {
        SetLives(lives - 1);

        if (lives > 0)
        {
            timer = PLAYER_KILLED_INTERVAL;
            SetGameMode(GAMEMODE.PLAYER_SPWAN_INTERVAL);
        }
        else
        {
            SetGameOver();
        }
    }

    public void SetScore(int s)
    {
        score = s;
        SetScoreText();

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "HIGH SCORE: " + highScore;
        }
    }

    public void AddScore(int ds)
    {
        if (gameMode == GAMEMODE.GAME)  // only add during game
            SetScore(score + ds);
    }

    public void SetScoreText()
    {
        scoreText.text = "SCORE: " + score;
    }

    public void SetLives(int l)
    {
        lives = l;
        livesText.text = "LIVES: " + lives;
    }

    public void SetTitleVisibility(bool bActive)
    {
        titleText.gameObject.SetActive(bActive);
    }

    public void SetGameOverVisibility(bool bActive)
    {
        gameOverText.gameObject.SetActive(bActive);
    }
    public void SetNewMazeButtonVisibility(bool bActive)
    {
        newMaze.gameObject.SetActive(bActive);
    }
    public void SetSwitchCameraButtonVisibility(bool bActive)
    {
        switchCam.gameObject.SetActive(bActive);
    }
    public GameObject GetShotPrefab() { return shotPrefab; }
    public GameObject GetHitPrefab() { return hitPrefab; }
    public GameObject GetHitBigPrefab() { return hitBigPrefab; }
}


