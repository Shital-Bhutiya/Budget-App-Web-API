using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Date { get; set; }      

        public string Category { get; set; }

        public string Account  { get; set; }

        public bool IsVoided { get; set; }
    }
}