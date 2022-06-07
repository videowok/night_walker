using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Director : MonoBehaviour
{
    public Maze maze;


    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject robotFlierPrefab;
    [SerializeField] GameObject robotHunterPrefab;

    //private GameObject robotTest;


    // Start is called before the first frame update
    void Start()
    {
        //maze = GameObject.Find("/Maze").GetComponent<Maze>();

        maze.BuildMaze();

        //robotTest = Instantiate(robotPrefab);

        Instantiate(playerPrefab);

        Instantiate(robotHunterPrefab);

        Instantiate(robotFlierPrefab);
        Instantiate(robotFlierPrefab);
        Instantiate(robotFlierPrefab);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class PATH_NODE
    {
        List<PATH_NODE> coonectNodeList;
        Vector3         Position;
    }
}


