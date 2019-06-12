using SD250_Deliverable_tmp_DGrouette.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Bindings
{
    public class HouseholdBindingModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public bool IsCreator { get; set; }
        public bool IsMember { get; set; }
        public List<HouseholdUserViewModel> HouseholdUsers { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<BankAccountViewModel> BankAccounts { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
    }
}