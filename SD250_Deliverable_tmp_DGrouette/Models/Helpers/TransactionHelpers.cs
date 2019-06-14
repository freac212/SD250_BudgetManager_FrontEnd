using Newtonsoft.Json;
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
    public class TransactionHelpers
    {
        public static bool IsUserCreator(int transactionId, HttpRequestBase request, TempDataDictionary tempData)
        {
            var url = $"{ProjectConstants.APIURL}/api/transaction/isusercreator/{transactionId}";

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

        public static void SetDropDownLists(CreateTransactionViewModel transactionViewModel, HttpRequestBase request)
        {
            var bankAccounts = HouseholdHelpers.GetBankAccounts(transactionViewModel.HouseholdId, request);
            var categories = HouseholdHelpers.GetCategories(transactionViewModel.HouseholdId, request);

            transactionViewModel.BankAccounts = bankAccounts.Select(p =>
                 new SelectListItem()
                 {
                     Text = p.Name,
                     Value = p.Id.ToString()
                 }).ToList();

            transactionViewModel.Categories = categories.Select(p =>
                new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
        }

        public static void SetCategoryDropDownList(CreateTransactionViewModel transactionViewModel, HttpRequestBase request)
        {
            var categories = HouseholdHelpers.GetCategories(transactionViewModel.HouseholdId, request);

            transactionViewModel.Categories = categories.Select(p =>
                new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
        }

        public static void SetCategoryDropDownList(EditTransactionViewModel transactionViewModel, HttpRequestBase request)
        {
            var categories = HouseholdHelpers.GetCategories(transactionViewModel.HouseholdId, request);

            transactionViewModel.Categories = categories.Select(p =>
                new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
        }

        public static bool BankAccntOrCategoriesIsNull(CreateTransactionViewModel viewModel, TempDataDictionary tempData)
        {
            if ((viewModel.BankAccounts is null || viewModel.Categories is null) || (viewModel.BankAccounts.Count <= 0 || viewModel.Categories.Count <= 0) &&!tempData.Keys.Contains("LoginMessage"))
            {
                tempData.Add("LoginMessage", "");
                tempData.Add("MessageColour", "danger");
            }

            if (viewModel.BankAccounts is null && viewModel.Categories is null || (viewModel.BankAccounts.Count <= 0 && viewModel.Categories.Count <= 0))
            {
                tempData["LoginMessage"] = "There's no bank accounts or categories on this household yet. Create them before making a transaction!";
                return true;
            }
            else if (viewModel.BankAccounts is null || viewModel.BankAccounts.Count <= 0)
            {
                tempData["LoginMessage"] = "There's no bank accounts on this household yet. Create one to make a transaction!";
                return true;
            }
            else if (viewModel.Categories is null || viewModel.Categories.Count <= 0)
            {
                tempData["LoginMessage"] = "There's no categories on this household yet. Create one to make a transaction!";
                return true;
            } else
            {
                return false;
            }
        }
    }
}