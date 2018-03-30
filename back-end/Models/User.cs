using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Login { get; set; }


        public string Email { get; set; }

        public ICollection<EmulationKit> EmulationKit { get; set; }
    }
}
