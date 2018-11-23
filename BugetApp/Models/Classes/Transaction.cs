using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.Classes
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public string EnteredById { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }

        public bool IsVoided { get; set; }
    }
}