using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class MyHoseholdViewModel
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string Name { get; set; }
        public ICollection<MyHouseHoldMembersVieModel> Members { get; set; }

        public MyHoseholdViewModel()
        {
            Members = new HashSet<MyHouseHoldMembersVieModel>();
        }
    }
}