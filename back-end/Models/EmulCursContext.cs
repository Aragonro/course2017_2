using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmulCurs.Models
{
    public class EmulCursContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public EmulCursContext() : base("name=EmulCursContext")
        {
        }

        public System.Data.Entity.DbSet<EmulCurs.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<EmulCurs.Models.EmulationKit> EmulationKits { get; set; }

        public System.Data.Entity.DbSet<EmulCurs.Models.Video> Videos { get; set; }

        public System.Data.Entity.DbSet<EmulCurs.Models.EmulationKitUpdate> EmulationKitUpdates { get; set; }

        public System.Data.Entity.DbSet<EmulCurs.Models.Device> Devices { get; set; }
    }
}
