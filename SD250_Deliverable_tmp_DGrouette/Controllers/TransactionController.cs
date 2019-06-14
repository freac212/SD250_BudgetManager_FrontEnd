using Newtonsoft.Json;
using SD220_Deliverable_1_DGrouette.Models.Filters;
using SD250_Deliverable_tmp_DGrouette.Models.Helpers;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using SD250_Deliverable_tmp_DGrouette.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Controllers
{
    [Auth]
    public class TransactionController : Controller
    {
        // Only household owner can create, edit, delete all transaction.
        // Other members of the household can only create, edit, delete, their own transactionss

        // GET: HouseholdTransactions
        [HttpGet]
        public ActionResult HouseholdTransactions(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/transaction/getallbyhousehold/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<TransactionViewModel>>(responseResult);

                foreach (var item in datas)
                {
                    item.CategoryName = HouseholdHelpers.GetCategoryName(item.CategoryId, Request);
                    item.BankAccountName = HouseholdHelpers.GetBankAccountName(item.BankAccountId, Request);
                    item.UserCanEdit = TransactionHelpers.IsUserCreator(item.Id, Request, TempData);
                }

                var viewModel = new TransactionListViewModel
                {
                    Transactions = datas,
                    HouseholdId = (int)Id,
                    IsHouseholdOwnerOrMember = HouseholdHelpers.IsUserCreatorOrMember((int)Id, Request, TempData)
                };
                return View(viewModel);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }
        }


        // GET: Create
        [HttpGet]
        public ActionResult Create(int? Id)
        {
            // Household Id
            if (Id is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var viewModel = new CreateTransactionViewModel()
            {
                HouseholdId = (int)Id,
                Date = DateTime.Now
            };

            TransactionHelpers.SetDropDownLists(viewModel, Request);

            if (TransactionHelpers.BankAccntOrCategoriesIsNull(viewModel, TempData))
                return RedirectToAction("Index", "Household");

            return View(viewModel);
        }


        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateTransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid || transactionViewModel.BankAccountId is null || transactionViewModel.CategoryId is null)
            {
                TransactionHelpers.SetDropDownLists(transactionViewModel, Request);

                if (TransactionHelpers.BankAccntOrCategoriesIsNull(transactionViewModel, TempData))
                    return RedirectToAction("Index", "Household");

                return View(transactionViewModel);
            }


            var url = $"{ProjectConstants.APIURL}/api/transaction/create/{transactionViewModel.BankAccountId}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Title", transactionViewModel.Title),
                new KeyValuePair<string, string>("Description", transactionViewModel.Description),
                new KeyValuePair<string, string>("Amount", transactionViewModel.Amount.ToString()),
                new KeyValuePair<string, string>("Date", transactionViewModel.Date.ToString()),
                new KeyValuePair<string, string>("CategoryId", transactionViewModel.CategoryId.ToString()),
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("Message", "Transaction Created!");
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = transactionViewModel.HouseholdId });
            }
            else
            {
                TransactionHelpers.SetDropDownLists(transactionViewModel, Request);
                if (TransactionHelpers.BankAccntOrCategoriesIsNull(transactionViewModel, TempData))
                    return RedirectToAction("Index", "Household");

                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);

                return View(transactionViewModel);
            }
        }

        // GET: Edit
        [HttpGet]
        public ActionResult Edit(int? Id)
        {
            // Category Id
            if (Id is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/transaction/getbyid/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<EditTransactionViewModel>(responseResult);

                TransactionHelpers.SetCategoryDropDownList(data, Request);

                if (data.Categories is null)
                {
                    TempData.Add("Message", "Error: Categories missing");
                    TempData.Add("MessageColour", "danger");
                    return RedirectToAction("Index", "Household");
                }

                return View(data);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }
        }


        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTransactionViewModel transactionViewModel)
        {
            if (!ModelState.IsValid)
            {
                TransactionHelpers.SetCategoryDropDownList(transactionViewModel, Request);

                if (transactionViewModel.Categories is null)
                {
                    TempData.Add("Message", "Error: Categories missing");
                    TempData.Add("MessageColour", "danger");
                    return RedirectToAction("Index", "Household");
                }

                return View(transactionViewModel);
            }

            var url = $"{ProjectConstants.APIURL}/api/transaction/edit/{transactionViewModel.Id}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Title", transactionViewModel.Title),
                new KeyValuePair<string, string>("Description", transactionViewModel.Description),
                new KeyValuePair<string, string>("Amount", transactionViewModel.Amount.ToString()),
                new KeyValuePair<string, string>("Date", transactionViewModel.Date.ToString()),
                new KeyValuePair<string, string>("CategoryId", transactionViewModel.CategoryId.ToString()),
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("Message", $"Transaction '{transactionViewModel.Title}' Edited!");
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = transactionViewModel.HouseholdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);

                TransactionHelpers.SetCategoryDropDownList(transactionViewModel, Request);

                if (transactionViewModel.Categories is null)
                {
                    TempData.Add("Message", "Error: Categories missing");
                    TempData.Add("MessageColour", "danger");
                    return RedirectToAction("Index", "Household");
                }

                return View(transactionViewModel);
            }
        }


        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? Id, int? householdId)
        {
            // Id is Transaction Id
            if (Id is null || householdId is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/transaction/delete/{Id}";

            var response = HttpClientContext.httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Transaction Deleted!";
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = householdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = householdId });
            }
        }


        // POST: SwitchVoid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchVoid(int? Id, int? householdId)
        {
            // Id is Transaction Id
            if (Id is null || householdId is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/transaction/switchvoid/{Id}";

            var response = HttpClientContext.httpClient.PostAsync(url, null).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = $"Transaction's void state has been changed successfully!";
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = householdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("HouseholdTransactions", "Transaction", new { Id = householdId });
            }
        }
    }
}