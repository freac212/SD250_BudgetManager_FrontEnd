﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsVoid { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CreatorId { get; set; }
        public int BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public int HouseholdId { get; set; }
        public bool UserCanEdit { get; set; }
    }
}