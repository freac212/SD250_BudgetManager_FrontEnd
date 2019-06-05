using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace SD250_Deliverable_tmp_DGrouette.Models.Domain
{
    public class ErrorDataSingleMessage
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}