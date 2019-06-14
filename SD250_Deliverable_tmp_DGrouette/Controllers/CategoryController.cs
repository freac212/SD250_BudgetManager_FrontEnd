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
    public class CategoryController : Controller
    {
        // Only household owner can create, edit, delete categories.
        // Other members of the household can view the categories

        // GET: Category
        public ActionResult HouseholdCategories(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/category/getallbyhousehold/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<CategoryViewModel>>(responseResult);
                var viewModel = new CategoryListViewModel
                {
                    Categories = datas,
                    HouseholdId = (int)Id,
                    IsHouseholdOwner = HouseholdHelpers.IsUserCreator((int)Id, Request, TempData)
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

            var viewModel = new CreateCategoryViewModel()
            {
                HouseholdId = (int)Id
            };

            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Place these on the other actions
        public ActionResult Create(CreateCategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
                return View(categoryViewModel);

            var url = $"{ProjectConstants.APIURL}/api/category/create/{categoryViewModel.HouseholdId}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", categoryViewModel.Name),
                new KeyValuePair<string, string>("Description", categoryViewModel.Description)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("Message", "Category Created!");
                return RedirectToAction("HouseholdCategories", "Category", new {Id = categoryViewModel.HouseholdId});
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(categoryViewModel);
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

            var url = $"{ProjectConstants.APIURL}/api/category/getbyid/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<CreateCategoryViewModel>(responseResult);

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
        public ActionResult Edit(CreateCategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
                return View(categoryViewModel);

            var url = $"{ProjectConstants.APIURL}/api/category/edit/{categoryViewModel.Id}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", categoryViewModel.Name),
                new KeyValuePair<string, string>("Description", categoryViewModel.Description)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("Message", $"Category '{categoryViewModel.Name}' Edited!");
                return RedirectToAction("HouseholdCategories", "Category", new { Id = categoryViewModel.HouseholdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(categoryViewModel);
            }
        }


        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? Id, int? householdId)
        {
            // Id is Category Id
            if (Id is null || householdId is null)
            {
                TempData.Add("Message", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/category/delete/{Id}";

            var response = HttpClientContext.httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Category Deleted!";
                return RedirectToAction("HouseholdCategories", "Category", new { Id = householdId });
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("HouseholdCategories", "Category", new { Id = householdId });
            }
        }
    }
}