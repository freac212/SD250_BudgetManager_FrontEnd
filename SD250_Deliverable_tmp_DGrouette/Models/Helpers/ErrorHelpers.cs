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

        public static bool IsNotFound(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string Message = "Item not found")
        {
            if (statusCode == System.Net.HttpStatusCode.NotFound)
            {
                tempData["Message"] = Message;
                tempData["MessageColour"] = "danger";
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IdIsInvalid(int? id, System.Web.Mvc.TempDataDictionary tempData, string Message = "Improper Id")
        {
            if (id is null)
            {
                tempData["Message"] = Message;
                tempData["MessageColour"] = "danger";
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsInternalServerError(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string Message = "Internal Server Error, try again later")
        {
            if (statusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                tempData["Message"] = Message;
                tempData["MessageColour"] = "danger";
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUnAuthorized(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string Message = "Error: Not Authorized")
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                tempData["Message"] = Message;
                tempData["MessageColour"] = "danger";
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsBadRequest(HttpStatusCode statusCode, System.Web.Mvc.TempDataDictionary tempData, string Message = "Error: Bad Request")
        {
            if (statusCode == System.Net.HttpStatusCode.BadRequest)
            {
                tempData["Message"] = Message;
                tempData["MessageColour"] = "danger";
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void HandleResponseErrors(HttpResponseMessage response, TempDataDictionary tempData, ModelStateDictionary modelState, string ItemName = "Item")
        {
            if (!tempData.ContainsKey("MessageColour"))
                tempData["MessageColour"] = "danger";

            // Firstly, handling errors that wont have messages.
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                tempData["Message"] = $"{ItemName} not found";
                return;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                tempData["Message"] = "Internal Server Error: Try again later";
                return;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                tempData["Message"] = "Error: Not Authorized";
                return;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var respResult = response.Content.ReadAsStringAsync().Result;
                var erData = JsonConvert.DeserializeObject<ErrorDataModelState>(respResult);

                if (erData != null)
                {
                    if (erData.ModelState != null)
                    {
                        // Model State error, such as leaving a field empty upon creating/ editing something
                        foreach (var item in erData.ModelState)
                        {
                            foreach (var ItemValue in item.Value)
                            {
                                modelState.AddModelError(string.Empty, ItemValue);
                            }
                        }
                    }
                    else
                    {
                        // Single string bad request error;
                        tempData["Message"] = "Error: " + erData.Message;
                    }
                }
                return;
            }

            // Secondly, handling any kind of errors that should have a message
            // Types:
            //  >ModelState Errors
            //  >"Error" key errors
            var responseResult = response.Content.ReadAsStringAsync().Result;
            dynamic errorDataUnknown = JsonConvert.DeserializeObject<ExpandoObject>(responseResult);

            if (errorDataUnknown.Error != null)
            {
                tempData["Message"] = errorDataUnknown.Error;
                return;
            }

            if (errorDataUnknown.Message != null)
            {
                tempData["Message"] = errorDataUnknown.Message;
                return;
            }

            // Thirdly, handling errors that shouldn't technically exist
            tempData["Message"] = "Unknown Error: Contact an admin or something";
            return;
        }
    }
}