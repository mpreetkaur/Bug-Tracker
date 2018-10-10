using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models.Classes
{
    public class TicketPriority
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}