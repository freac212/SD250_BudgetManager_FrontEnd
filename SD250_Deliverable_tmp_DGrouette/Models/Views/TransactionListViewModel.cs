using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class TransactionListViewModel
    {
        public int HouseholdId { get; set; }
        public bool IsHouseholdOwnerOrMember { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
    }
}