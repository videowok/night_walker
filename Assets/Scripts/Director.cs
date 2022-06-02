using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


public class Director : MonoBehaviour
{
    private MapXML.Map map;


    // Start is called before the first frame update
    void Start()
    {
        map = LoadMapData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    MapXML.Map LoadMapData()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "arena_map.xml");

        if (File.Exists(fileName))
        {
            string fileText = File.ReadAllText(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(MapXML.Map));
            using (StringReader reader = new StringReader(fileText))
            {
                //MapXML.Map map = serializer.Deserialize(reader) as MapXML.Map;
                MapXML.Map map = (MapXML.Map)serializer.Deserialize(reader);

                foreach (MapXML.MapItem item in map.MapItems)
                {
                    Debug.Log("Map type: " + item.Type + ". Position: " + item.Position.x + ", " + item.Position.z);
                }

                return map;
            }
        }

        Debug.Log("No map xml file: " + fileName);

        return null;

    }
}

public class PlayerData
{
    public string Name;
    public Vector3 Position;
}
