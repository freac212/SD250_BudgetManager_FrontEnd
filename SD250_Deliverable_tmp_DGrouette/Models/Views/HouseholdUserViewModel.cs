using SD250_Deliverable_tmp_DGrouette.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class HouseholdUserViewModel
    {
        public List<UserViewModel> UserViewModels { get; set; }
        public bool IsCreator { get; set; }
        public int HouseholdId { get; set; }
    }
}