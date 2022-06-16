using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    public static Director Instance { get; private set; }    // ENCAPSULATION
    public AudioManager audioManager { get; private set; }    // ENCAPSULATION
    public CameraManager cameraManager { get; private set; }    // ENCAPSULATION
    public Maze maze { get; private set; }   // ENCAPSULATION

    // ui
    // ENCAPSULATION
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Button startGame;
    [SerializeField] Button switchCam;
    [SerializeField] Button newMaze;

    public const string SHOT_PLAYER_TAG = "ShotPlayerTag";
    public const string SHOT_ENEMY_TAG  = "ShotEnemyTag";
    public const string PLAYER_TAG      = "PlayerTag";
    public const string FLIER_TAG       = "FlierTag";
    public const string HUNTER_TAG      = "HunterTag";
    public const string ROVER_TAG       = "RoverTag";
    public const string WALL_TAG        = "WallTag";

    // ENCAPSULATION
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject robotFlierPrefab;
    [SerializeField] GameObject robotHunterPrefab;
    [SerializeField] GameObject robotRoverPrefab;
    [SerializeField] GameObject shotPlayerPrefab;
    [SerializeField] GameObject shotEnemyPrefab;
    [SerializeField] GameObject hitPrefab;
    [SerializeField] GameObject hitMedPrefab;
    [SerializeField] GameObject hitBigPrefab;

    // ENCAPSULATION...

    private const float SPAWN_INTERVAL = 1;
    private const float DIFF_CHECK_INTERVAL = 5;    // recalculate enemy counts
    private const float THUMP_INTERVAL = 2;

    private const int initialHunters = 2;
    private const int initialRovers = 2;
    private const int initialFliers = 2;

    private const int initialDemoHunters = 3;
    private const int initialDemoRovers = 6;
    private const int initialDemoFliers = 9;

    private int maxHunters;
    private int maxRovers;
    private int maxFliers;

    private float spawnInterval = 0;
    private float diffCheckInterval = 0;

    private enum GAMEMODE
    {
        DEMO = 0,
        GAME,
        PLAYER_SPWAN_INTERVAL,
        GAME_OVER
    };

    private GAMEMODE gameMode;

    private const float PLAYER_KILLED_INTERVAL = 3;
    private const float GAME_OVER_INTERVAL = 4;

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

        LoadData();
        SetHighScoreText(highScore);

        audioManager = GameObject.Find("/AudioManager").GetComponent<AudioManager>();

        cameraManager = GameObject.Find("/Main Camera").GetComponent<CameraManager>();

        maze = GameObject.Find("/Maze").GetComponent<Maze>();
        maze.BuildMaze();

        InvokeRepeating("PlayThumpSFX", THUMP_INTERVAL, THUMP_INTERVAL);

        StartDemo();
    }

    // Update is called once per frame
    void Update()
    {
        updateCount++;

        // UI keys (mouse pointer not visible during game)

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartGame();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            RebuildMaze();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            cameraManager.SwitchCamera();
        }

        switch (gameMode)
        {
            case GAMEMODE.DEMO:

                HandleSpawning();
                break;

            case GAMEMODE.GAME:

                diffCheckInterval -= Time.deltaTime;

                if (diffCheckInterval <= 0)
                {
                    // recalculate enemy counts

                    diffCheckInterval = DIFF_CHECK_INTERVAL;

                    maxHunters = initialHunters + (score / 5000);
                    maxRovers = initialRovers + (score / 3000);
                    maxFliers = initialFliers + (score / 2000);
                }

                HandleSpawning();
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

    // ABSTRACTION
    private void PlayThumpSFX()
    {
        audioManager.Play(AudioManager.SFX.THUMP);
    }

    // ABSTRACTION
    private void HandleSpawning()
    {
        spawnInterval -= Time.deltaTime;

        if (spawnInterval <= 0)
        //if (false)
        {
            spawnInterval = SPAWN_INTERVAL;

            InstantiateGobs(HUNTER_TAG, maxHunters, robotHunterPrefab);
            InstantiateGobs(ROVER_TAG, maxRovers, robotRoverPrefab);
            InstantiateGobs(FLIER_TAG, maxFliers, robotFlierPrefab);
        }
    }

    // ABSTRACTION
    private void SetEnemyCount(int countHunter, int countRover, int countFlier)
    {
        maxHunters = countHunter;
        maxRovers = countRover;
        maxFliers = countFlier;
    }

    // ABSTRACTION
    private void InstantiateGobs(string name, int maxCount, GameObject prefab)
    {
        GameObject[] gobs;

        gobs = GameObject.FindGameObjectsWithTag(name);

        for (int i = 0; i < maxCount - gobs.Length; i++)
            Instantiate(prefab);
    }

    // ABSTRACTION
    private void SetGameMode(GAMEMODE mode)
    {
        gameMode = mode;
    }

    // ABSTRACTION
    private void RemoveObjectsWithTag(string tag)
    {
        GameObject[] gobs = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in gobs)
            Destroy(obj);
    }

    // ABSTRACTION
    private void ClearAllDynamicObjects()
    {
        RemoveObjectsWithTag(PLAYER_TAG);
        RemoveObjectsWithTag(HUNTER_TAG);
        RemoveObjectsWithTag(ROVER_TAG);
        RemoveObjectsWithTag(FLIER_TAG);
        RemoveObjectsWithTag(SHOT_PLAYER_TAG);
        RemoveObjectsWithTag(SHOT_ENEMY_TAG);
    }

    public void RebuildMaze()
    {
        ClearAllDynamicObjects();
        RemoveObjectsWithTag(WALL_TAG);

        maze.BuildMaze();

        StartDemo();
    }

    // ABSTRACTION
    private void StartGame()
    {
        Cursor.visible = false;

        SetGameMode(GAMEMODE.GAME);

        ClearAllDynamicObjects();

        SetTitleVisibility(false);
        SetGameOverVisibility(false);
        SetNewMazeButtonVisibility(false);
        SetSwitchCameraButtonVisibility(true);

        SetScore(0);
        SetLives(3);

        Instantiate(playerPrefab);
        SetEnemyCount(initialHunters, initialRovers, initialFliers);

        cameraManager.SetTypeToLsetUsed();

        audioManager.Play(AudioManager.SFX.GAME_START);
    }

    // ABSTRACTION
    private void ContinueGame()
    {
        SetGameMode(GAMEMODE.GAME);

        ClearAllDynamicObjects();
        Instantiate(playerPrefab);
        spawnInterval = SPAWN_INTERVAL;

        audioManager.Play(AudioManager.SFX.GAME_START);
    }

    public void StartDemo()
    {
        Cursor.visible = true;

        SetGameMode(GAMEMODE.DEMO);

        SetEnemyCount(initialHunters, initialRovers, initialFliers);

        SetTitleVisibility(true);
        SetGameOverVisibility(false);
        SetNewMazeButtonVisibility(true);
        SetSwitchCameraButtonVisibility(false);

        SetEnemyCount(initialDemoHunters, initialDemoRovers, initialDemoFliers);

        cameraManager.SetType(CameraManager.TYPE.DEMO);
    }

    // ABSTRACTION
    private void SetGameOver()
    {
        SetGameMode(GAMEMODE.GAME_OVER);

        timer = GAME_OVER_INTERVAL;
        SetGameOverVisibility(true);

        audioManager.Play(AudioManager.SFX.GAME_OVER);
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

    // ABSTRACTION
    private void SetScore(int s)
    {
        score = s;
        SetScoreText();

        if (score > highScore)
        {
            SetHighScoreText(score);
        }
    }

    // ABSTRACTION
    private void SetHighScoreText(int s)
    {
        highScore = s;
        highScoreText.text = "HIGH SCORE: " + highScore;
    }

    public void AddScore(int ds)
    {
        if (gameMode == GAMEMODE.GAME)  // only add during game
            SetScore(score + ds);
    }

    // ABSTRACTION
    private void SetScoreText()
    {
        scoreText.text = "SCORE: " + score;
    }

    // ABSTRACTION
    private void SetLives(int l)
    {
        lives = l;
        livesText.text = "LIVES: " + lives;
    }

    // ABSTRACTION
    private void SetTitleVisibility(bool bActive)
    {
        titleText.gameObject.SetActive(bActive);
    }

    // ABSTRACTION
    private void SetGameOverVisibility(bool bActive)
    {
        gameOverText.gameObject.SetActive(bActive);
    }

    // ABSTRACTION
    private void SetNewMazeButtonVisibility(bool bActive)
    {
        newMaze.gameObject.SetActive(bActive);
    }
    
    // ABSTRACTION
    private void SetSwitchCameraButtonVisibility(bool bActive)
    {
        switchCam.gameObject.SetActive(bActive);
    }

    public GameObject GetPlayerShotPrefab() { return shotPlayerPrefab; }
    public GameObject GetEnemyShotPrefab() { return shotEnemyPrefab; }
    public GameObject GetHitPrefab() { return hitPrefab; }
    public GameObject GetHitMediumPrefab() { return hitMedPrefab; }
    public GameObject GetHitBigPrefab() { return hitBigPrefab; }

    // ABSTRACTION
    private bool IsButtonClickValid()   // avoid invisible (game) mouse clicking on menu buttons
    {
        return gameMode == GAMEMODE.DEMO ? true : false;
    }

    public void OnStartGameButtonClicked()
    {
        if (IsButtonClickValid())
            StartGame();
    }

    public void OnExitButtonClicked()
    {
        if (IsButtonClickValid())
            Exit();
    }

    public void OnCameraButtonClicked()
    {
        if (IsButtonClickValid())
            cameraManager.SwitchCamera();
    }

    public void OnMazeButtonClicked()
    {
        if (IsButtonClickValid())
            RebuildMaze();
    }

    // ABSTRACTION
    private void Exit()
    {
        if (gameMode != GAMEMODE.DEMO)
        {
            StartDemo();
        }
        else
        {
            SaveData();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit(); // original code to quit Unity player
#endif
        }
    }

    [System.Serializable]
    class NightWalkerSaveData
    {
        public int HighScore;
    }

    // ABSTRACTION
    private void SaveData()
    {
        NightWalkerSaveData data = new NightWalkerSaveData();

        data.HighScore = highScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    // ABSTRACTION
    private void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NightWalkerSaveData data = JsonUtility.FromJson<NightWalkerSaveData>(json);

            highScore = data.HighScore;
        }
    }
}


