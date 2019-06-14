using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class EditTransactionViewModel
    {
        [Required]
        public string Id { get; set; } // Only required for editing
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please choose a Category")]
        public int CategoryId { get; set; }
        [Required]
        public int HouseholdId { get; set; }

        public List<SelectListItem> BankAccounts { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}