using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class EmulationKitUpdate
    {
        public int EmulationKitUpdateId { get; set; }

        public int EmulationKitId { get; set; }
        public int TemperatureUpdate { get; set; }

        public int PressureUpdate { get; set; }

        public int HumidityUpdate { get; set; }

        public EmulationKit EmulationKit { get; set; }
    }
}