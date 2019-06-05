using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class CategoryListViewModel
    {
        public int HouseholdId { get; set; }
        public bool IsHouseholdOwner { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
    }
}