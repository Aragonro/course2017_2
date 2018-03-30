using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class Device
    {
        public int DeviceId { get; set; }

        public string Prof { get; set; }

        public int Status { get; set; }

        public int UserId { get; set; }
        public int EmulationKitId { get; set; }

    }
}