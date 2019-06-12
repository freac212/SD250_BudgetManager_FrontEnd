using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class HouseholdDetailsViewModel
    {
        public int HouseholdId { get; set; }
        public decimal NetSum { get; set; } // Sum of all accounts in this household
        public List<BankAccountTransactionsViewModel> BankAccountViewModels { get; set; }
    }

    public class BankAccountTransactionsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; } // Sum of all transactions in this account

        public List<IGrouping<string, TransactionViewModel>> TransactionViewModels { get; set; }
    }
}