using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EnlightedApiConsumer.Data.Models
{
    [XmlRoot("floors")]
    public class FloorResults
    {
        [XmlElement(ElementName = "floor")]
        public List<Floor> floors;
    }
    public class Floor
    {

        //[XmlAttribute("id")]  [XmlElement("RefNo")]    
        //[XmlElement("id")]
        public int id { get; set; }

        //[XmlAttribute]
        //[XmlElement("name")]
        public int name { get; set; }

        //[XmlAttribute]
        //[XmlElement("building")]
        public int building { get; set; }

        //[XmlAttribute]
        //[XmlElement("campus")]
        public int campus { get; set; }

        //[XmlAttribute]
        //[XmlElement("company")]
        public int company { get; set; }

        //[XmlAttribute]
        //[XmlElement("description")]
        public string? description { get; set; }

        //[XmlAttribute]
        //[XmlElement("floorPlanUrl")]
        public string floorPlanUrl { get; set; }

        [XmlElement(ElementName = "parentFloorId")]
        public string ParentFloorId { get; set; }
    }
}
