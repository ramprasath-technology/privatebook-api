using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateBookAPI.Data
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime Time { get; set; }
  
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
