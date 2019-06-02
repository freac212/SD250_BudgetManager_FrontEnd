using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SD250_Deliverable_tmp_DGrouette.Models.Helpers
{
    public static class ErrorHelpers
    {

        public static bool IsNotFound(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData)
        {
            if (statusCode == System.Net.HttpStatusCode.NotFound)
            {
                tempData.Add("LoginMessage", "Item not found");
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IdIsInvalid(int? id, System.Web.Mvc.TempDataDictionary tempData)
        {
            if (id is null)
            {
                tempData.Add("LoginMessage", "Improper Id");
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsInternalServerError(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData)
        {
            if (statusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                tempData.Add("LoginMessage", "Internal Server Error, try again later");
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUnAuthorized(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData)
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                tempData.Add("LoginMessage", "Error: Not Authorized");
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}