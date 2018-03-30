using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class Video
    {
        public int VideoId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<EmulationKit> EmulationKit { get; set; }
    }
}