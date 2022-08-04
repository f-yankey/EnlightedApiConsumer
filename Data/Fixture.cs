using System;
using System.Collections.Generic;

#nullable disable

namespace EnlightedApiConsumer
{
    public partial class Fixture
    {
        public int FixtureId { get; set; }
        public int FloorId { get; set; }
        public string Name { get; set; }
        public int? Xaxis { get; set; }
        public int? Yaxis { get; set; }
        public int? GroupId { get; set; }
        public string MacAddress { get; set; }
        public string ClassName { get; set; }
    }
}
