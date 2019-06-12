using Newtonsoft.Json;
using SD250_Deliverable_tmp_DGrouette.Models.Domain;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Models.Helpers
{
    public static class ErrorHelpers
    {

        public static bool IsNotFound(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string LoginMessage = "Item not found")
        {
            if (statusCode == System.Net.HttpStatusCode.NotFound)
            {
                tempData.Add("LoginMessage", LoginMessage);
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IdIsInvalid(int? id, System.Web.Mvc.TempDataDictionary tempData, string LoginMessage = "Improper Id")
        {
            if (id is null)
            {
                tempData.Add("LoginMessage", LoginMessage);
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsInternalServerError(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string LoginMessage = "Internal Server Error, try again later")
        {
            if (statusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                tempData.Add("LoginMessage", LoginMessage);
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUnAuthorized(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string LoginMessage = "Error: Not Authorized")
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                tempData.Add("LoginMessage", LoginMessage);
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsBadRequest(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string LoginMessage = "Error: Bad Request")
        {
            if (statusCode == System.Net.HttpStatusCode.BadRequest)
            {
                tempData.Add("LoginMessage", LoginMessage);
                tempData.Add("MessageColour", "danger");
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void HandleResponseErrors(HttpResponseMessage response, TempDataDictionary tempData, ModelStateDictionary modelState, string ItemName = "Item")
        {
            // Firstly, handling errors that wont have messages.
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                tempData.Add("LoginMessage", $"{ItemName} not found");
                tempData.Add("MessageColour", "danger");
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                tempData.Add("LoginMessage", "Internal Server Error: Try again later");
                tempData.Add("MessageColour", "danger");
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                tempData.Add("LoginMessage", "Error: Not Authorized");
                tempData.Add("MessageColour", "danger");
                return;
            }

            // Secondly, handling any kind of errors that should have a message
            // Types:
            //  >ModelState Errors
            //  >Messages
            //  >"Error" key errors
            var responseResult = response.Content.ReadAsStringAsync().Result;
            var errorData = JsonConvert.DeserializeObject<ErrorDataModelState>(responseResult);

            tempData.Add("LoginMessage", "Error");
            tempData.Add("MessageColour", "danger");

            if (errorData != null)
            {
                if (errorData.ModelState != null)
                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            modelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }
                else
                {
                    modelState.AddModelError(string.Empty, errorData.Message);

                    tempData["LoginMessage"] = "Error";
                    tempData["MessageColour"] = "danger";
                }
            }

            var errorDataSingleMessage = JsonConvert.DeserializeObject<ErrorDataSingleMessage>(responseResult);
            if (errorDataSingleMessage.Error != null)
            {
                tempData["LoginMessage"] = errorDataSingleMessage.Error;
                tempData["MessageColour"] = "danger";
                return;
            }

            dynamic errorDataBadRequest = JsonConvert.DeserializeObject<ExpandoObject>(responseResult);
            if (errorDataBadRequest.Message != null)
            {
                tempData["LoginMessage"] = errorDataBadRequest.Message;
                tempData["MessageColour"] = "danger";
                return;
            }

            // Thirdly, handling errors that shouldn't technically exist
            tempData["LoginMessage"] = "Unknown Error: Contact an admin or something";
            tempData["MessageColour"] = "danger";
            return;

        }
    }
}