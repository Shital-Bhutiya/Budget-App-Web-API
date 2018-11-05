using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.Classes
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int HouseholdId { get; set; }
        public virtual HouseHolds Household { get; set; }

        
    }
}