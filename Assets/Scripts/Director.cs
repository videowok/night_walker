using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


public class Director : MonoBehaviour
{
    private MapXML.Map map;
    
    enum MAZE_DIRECTION
    {
        UP = 0,
        DOWN,
        RIGHT,

        MAX,

        NOT_SET
    };

    [SerializeField] int MAZE_SIZE_X = 30;
    [SerializeField] int MAZE_SIZE_Y = 30;

    [SerializeField] float MAZE_SCALE_X = 1.0f;
    [SerializeField] float MAZE_SCALE_Y = 1.0f;

    [SerializeField] int TUNNEL_SECTIONS = 20;
    [SerializeField] int TUNNEL_MARGIN = 6;
    [SerializeField] int TUNNEL_SIZE = 6;

    [SerializeField] GameObject wallBlockPrefab;
    [SerializeField] GameObject wallXPrefab;
    [SerializeField] GameObject wallZPrefab;

    private int[,] mapArray;


    // Start is called before the first frame update
    void Start()
    {
        //map = LoadMapData();

        mapArray = new int[MAZE_SIZE_X, MAZE_SIZE_Y];

        BuildMaze();
    }

    // Update is called once per frame
    void Update()
    {

    }

    MapXML.Map LoadMapData()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "arena_map2.xml");

        if (File.Exists(fileName))
        {
            string fileText = File.ReadAllText(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(MapXML.Map));
            using (StringReader reader = new StringReader(fileText))
            {
                //MapXML.Map map = serializer.Deserialize(reader) as MapXML.Map;
                MapXML.Map map = (MapXML.Map)serializer.Deserialize(reader);

                //foreach (MapXML.MapItem item in map.MapItems)
                //{
                //    Debug.Log("Map type: " + item.Type + ". Position: " + item.Position.x + ", " + item.Position.z);
                //}

                return map;
            }
        }

        Debug.Log("No map xml file: " + fileName);

        return null;

    }

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




    public static int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
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

    private void BuildMaze()
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
                    pos.x = x * MAZE_SCALE_X;
                    pos.z = y * MAZE_SCALE_Y;

                    block = Instantiate(wallBlockPrefab);
                    block.transform.position = pos;
                }
            }
        }
    }

    public class PATH_NODE
    {
        List<PATH_NODE> coonectNodeList;
        Vector3         Position;
    }
}


