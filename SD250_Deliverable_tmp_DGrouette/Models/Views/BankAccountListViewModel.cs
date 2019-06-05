using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class BankAccountListViewModel
    {
        public int HouseholdId { get; set; }
        public bool IsHouseholdOwner { get; set; }
        public List<BankAccountViewModel> BankAccounts { get; set; }
    }
}