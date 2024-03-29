﻿using Newtonsoft.Json;
using SD250_Deliverable_tmp_DGrouette.Controllers;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using SD250_Deliverable_tmp_DGrouette.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Models.Helpers
{
    public class HouseholdHelpers
    {
        public static bool IsUserCreator(int householdId, HttpRequestBase request, TempDataDictionary tempData)
        {
            var url = $"{ProjectConstants.APIURL}/api/household/isusercreator/{householdId}";

            var token = request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, tempData))
                return false;
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, tempData))
                return false;
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, tempData))
                return false;

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<IsCreatorViewModel>(responseResult);
                return data.IsCreator;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUserCreatorOrMember(int householdId, HttpRequestBase request, TempDataDictionary tempData)
        {
            var url = $"{ProjectConstants.APIURL}/api/household/isusercreatorormember/{householdId}";

            var token = request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, tempData))
                return false;
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, tempData))
                return false;
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, tempData))
                return false;

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<IsCreatorOrMemberViewModel>(responseResult);
                return data.IsCreatorOrMember;
            }
            else
            {
                return false;
            }
        }

        public static List<CategoryViewModel> GetCategories(int? householdId, HttpRequestBase Request)
        {
            if (householdId is null)
                return null;

            var url = $"{ProjectConstants.APIURL}/api/category/getallbyhousehold/{householdId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<CategoryViewModel>>(responseResult);
 
                return datas;
            }
            else
            {
                return null;
            }
        }

        public static List<BankAccountViewModel> GetBankAccounts(int? householdId, HttpRequestBase Request)
        {
            if (householdId is null)
                return null;

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/getallbyhousehold/{householdId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(responseResult);

                return datas;
            }
            else
            {
                return null;
            }
        }

        internal static string GetBankAccountName(int? bankAccountId, HttpRequestBase Request)
        {
            if (bankAccountId is null)
                return null;

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/getName/{bankAccountId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<string>(responseResult);

                return data;
            }
            else
            {
                return null;
            }
        }

        internal static string GetCategoryName(int? categoryId, HttpRequestBase Request)
        {
            if (categoryId is null)
                return null;

            var url = $"{ProjectConstants.APIURL}/api/category/getName/{categoryId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<string>(responseResult);

                return data;
            }
            else
            {
                return null;
            }
        }
    }
}