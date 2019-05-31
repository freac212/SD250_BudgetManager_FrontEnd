using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Views
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}