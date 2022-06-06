using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


public class Director : MonoBehaviour
{
    public Maze maze;

    [SerializeField] GameObject robotPrefab;

    private GameObject robotTest;


    // Start is called before the first frame update
    void Start()
    {
        //maze = GameObject.Find("/Maze").GetComponent<Maze>();

        maze.BuildMaze();

        robotTest = Instantiate(robotPrefab);
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


