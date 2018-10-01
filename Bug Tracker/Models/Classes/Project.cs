using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models.Classes
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set;}

        public ICollection<ApplicationUser> Users { get; set; }
        
        public Project()
        {
            Users = new HashSet<ApplicationUser>();
        }
    }
}