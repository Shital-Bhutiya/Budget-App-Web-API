using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugetApp.Models.ViewModel
{
    public class AccountsEditingViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Balance { get; set; }
       
    }
}