using BugetApp.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class TransactionEditingViewModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}