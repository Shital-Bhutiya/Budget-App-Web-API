using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class AccountsHouseHoldViewModel
    {
        public string HouseholdName { get; set; }
        public AccountsViewModel Accounts { get; set; }
        //public int Balance { get; set; }
       public AccountsHouseHoldViewModel()
        {
            Accounts = new AccountsViewModel();
        }
    }
    public class AccountsViewModel
    {
        public string Owner { get; set; }
        public List<string> JoinedUsers { get; set; }
        public AccountsViewModel()
        {
            JoinedUsers = new List<string>();
        }
    }
}