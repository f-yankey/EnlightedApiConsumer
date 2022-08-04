using System;
using System.Collections.Generic;

#nullable disable

namespace EnlightedApiConsumer
{
    public partial class Floor
    {
        public int FloorId { get; set; }
        public int? Name { get; set; }
        public int? Building { get; set; }
        public int? Campus { get; set; }
        public int? Company { get; set; }
        public string Description { get; set; }
        public string FloorPlanUrl { get; set; }
        public string ParentFloorId { get; set; }
    }
}
