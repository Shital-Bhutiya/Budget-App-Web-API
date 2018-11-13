using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class HouseHoldViewModel
    {
        public string Name { get; set; }
        public string Creator { get; set; }
        public List<string> Members { get; set; }
        public HouseHoldViewModel()
        {
            Members = new List<string>();
        }
    }
}