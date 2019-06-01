﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD220_Deliverable_1_DGrouette.Models.Views
{
    public class CategoryViewModel
    {
        public int Id { get; internal set; }
        public DateTime DateCreated { get; internal set; }
        public DateTime? DateUpdated { get; internal set; }
        public string Description { get; internal set; }
        public string Name { get; internal set; }
        public int HouseholdId { get; set; }
    }
}