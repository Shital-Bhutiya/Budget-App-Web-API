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
            JoinedUsers = new HashSet<ApplicationUser>();
            Categories = new HashSet<Category>();
            Accounts = new HashSet<Account>();
        }
        public virtual ICollection<HouseHoldInvites> HouseHoldInvites { get; set; }
        public virtual ICollection<ApplicationUser> JoinedUsers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}