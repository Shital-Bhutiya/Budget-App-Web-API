using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.Classes
{
    public class HouseHolds
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public HouseHolds()
        {
            HouseHoldInvites = new HashSet<HouseHoldInvites>();
        }
        public virtual ICollection<HouseHoldInvites> HouseHoldInvites { get; set; }

    }
}