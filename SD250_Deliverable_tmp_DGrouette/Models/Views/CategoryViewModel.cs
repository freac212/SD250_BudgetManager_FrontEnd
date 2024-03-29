﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int HouseholdId { get; set; }
    }
}