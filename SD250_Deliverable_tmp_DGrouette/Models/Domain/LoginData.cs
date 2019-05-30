using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Domain
{
    public class LoginData
    {
        // Since the prop form the api's name is "access_token", it's being renamed to make it easier to work with here.
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}