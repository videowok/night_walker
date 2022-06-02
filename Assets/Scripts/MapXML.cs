using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace MapXML
{
    [XmlRoot(ElementName = "item")]
    public class MapItem
    {
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "position")]
        public Vector3 Position { get; set; }
    }

    [XmlRoot(ElementName = "map")]
    public class Map
    {
        [XmlElement(ElementName = "item")]
        public List<MapItem> MapItems { get; set; }
    }
}
