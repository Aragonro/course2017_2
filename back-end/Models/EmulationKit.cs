using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class EmulationKit
    {
        public int EmulationKitId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int VideoId { get; set; }
        public int Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int Like { get; set; }
        public int Dislike { get; set; }
        public User User { get; set; }
        public Video Video { get; set; }
        public ICollection<EmulationKitUpdate> EmulationKitUpdate { get; set; }
    }
}