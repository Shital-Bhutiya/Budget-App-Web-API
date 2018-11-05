namespace BugetApp.Models.Classes
{
    public class Account
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int HouseholdId { get; set; }
        public virtual HouseHolds Household { get; set; }

        public int Balance { get; set; }
    }
}