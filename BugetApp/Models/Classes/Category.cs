using System.Collections.Generic;

namespace BugetApp.Models.Classes
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int HouseholdId { get; set; }
        public virtual HouseHolds Household { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public Category()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}