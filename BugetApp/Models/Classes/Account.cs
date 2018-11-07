using System.Collections.Generic;

namespace BugetApp.Models.Classes
{
    public class Account
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Balance { get; set; }

        public int HouseholdId { get; set; }
        public virtual HouseHolds Household { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public Account()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}