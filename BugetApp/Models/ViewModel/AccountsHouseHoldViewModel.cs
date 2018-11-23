using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class AccountsHouseHoldViewModel
    {
        public string HouseholdName { get; set; }
        public List<AccountsViewModel> Accounts { get; set; }
        public AccountsHouseHoldViewModel()
        {
            Accounts = new List<AccountsViewModel>();
        }
    }
    public class AccountsViewModel
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
    }
}