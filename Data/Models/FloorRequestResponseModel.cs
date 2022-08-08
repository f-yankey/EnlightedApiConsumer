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

        [XmlElement(ElementName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "name")]
        public int Name { get; set; }

        [XmlElement(ElementName = "building")]
        public int Building { get; set; }

        [XmlElement(ElementName = "campus")]
        public int Campus { get; set; }

        [XmlElement(ElementName = "company")]
        public int Company { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "floorPlanUrl")]
        public string FloorPlanUrl { get; set; }

        [XmlElement(ElementName = "parentFloorId")]
        public string ParentFloorId { get; set; }
    }
}
