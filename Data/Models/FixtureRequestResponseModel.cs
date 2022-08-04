using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EnlightedApiConsumer.Data.Models
{
    [XmlRoot("fixtures")]
    public class FixtureResults
    {
        [XmlElement(ElementName = "fixture")]
        public List<Fixture> fixtures;
    }

    public class Fixture
    {
        public int id { get; set; }
        public string name { get; set; }
        public int xaxis { get; set; }
        public int yaxis { get; set; }
        public int groupId { get; set; }
        public string macAddress { get; set; }

        [XmlElement(ElementName = "class")]
        public string ClassName { get; set; }

    }
}
