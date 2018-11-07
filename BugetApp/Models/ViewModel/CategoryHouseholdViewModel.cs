using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class CategoryHouseholdViewModel
    {
        public string HouseholdName { get; set; }
        public List<IndividualCategoryViewModel> IndividualCategories { get; set; }
        public List<AllCategoryViewModel> AllCategory { get; set; }
        public CategoryHouseholdViewModel()
        {
            IndividualCategories = new List<IndividualCategoryViewModel>();
            AllCategory = new List<AllCategoryViewModel>();
        }
    }
    public class IndividualCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AllCategoryViewModel : IndividualCategoryViewModel
    {
        public string HouseHoldName { get; set; }
    }
}