using Newtonsoft.Json;
using SD250_Deliverable_tmp_DGrouette.Models.Helpers;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using SD250_Deliverable_tmp_DGrouette.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.Expando;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Controllers
{
    public class BankAccountController : Controller
    {
        // Only household owner can create, edit, delete BankAccounts
        // Other members of the household can view the bank accounts

        // GET: HouseholdBankAccounts
        public ActionResult HouseholdBankAccounts(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/getallbyhousehold/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(responseResult);
                var viewModel = new BankAccountListViewModel
                {
                    BankAccounts = datas,
                    HouseholdId = (int)Id,
                    IsHouseholdOwner = HouseholdController.IsUserCreator((int)Id, Request, TempData)
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
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var viewModel = new CreateBankAccountViewModel()
            {
                HouseholdId = (int)Id
            };

            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBankAccountViewModel bankAccountViewModel)
        {
            if (!ModelState.IsValid)
                return View(bankAccountViewModel);

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/create/{bankAccountViewModel.HouseholdId}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", bankAccountViewModel.Name),
                new KeyValuePair<string, string>("Description", bankAccountViewModel.Description)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["LoginMessage"] = "Bank Account Created!";
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = bankAccountViewModel.HouseholdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(bankAccountViewModel);
            }
        }

        // GET: Edit
        [HttpGet]
        public ActionResult Edit(int? Id)
        {
            // Bank Account Id
            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/getbyid/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<CreateBankAccountViewModel>(responseResult);

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
        public ActionResult Edit(CreateBankAccountViewModel bankAccountViewModel)
        {
            if (!ModelState.IsValid)
                return View(bankAccountViewModel);

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/edit/{bankAccountViewModel.Id}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", bankAccountViewModel.Name),
                new KeyValuePair<string, string>("Description", bankAccountViewModel.Description)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", $"Account '{bankAccountViewModel.Name}' Edited!");
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = bankAccountViewModel.HouseholdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(bankAccountViewModel);
            }
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? Id, int? householdId)
        {
            // Id is Bank Account Id
            if (Id is null || householdId is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/delete/{Id}";

            var response = HttpClientContext.httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["LoginMessage"] = "Account Deleted!";
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = householdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = householdId });
            }
        }

        // POST: UpdateBalance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBalance(int? Id, int? householdId)
        {
            // Id is Bank Account Id
            if (Id is null || householdId is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/bankaccount/updatebalance/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(responseResult);

                TempData["LoginMessage"] = $"Account '{data.Name}' balance has been updated!";
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = householdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("HouseholdBankAccounts", "BankAccount", new { Id = householdId });
            }
        }
    }
}