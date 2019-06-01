using Newtonsoft.Json;
using SD220_Deliverable_1_DGrouette.Models.Filters;
using SD250_Deliverable_tmp_DGrouette.Models.Domain;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Controllers
{
    public class HouseholdController : Controller
    {
        private const string APIURL = "http://localhost:52445";
        private static readonly HttpClient httpClient = new HttpClient(); // Apparently this was reccomended! <- Will solve the fact that I'm
        // using it more than once eventually!

        /*
         *Household Management
         *  Create household
         *  Edit household
         *  View households -> Created and households in
         *  View users in household
         *  Invite users to join household
         *  Join household 
         *  Leave household
         */

        // GET: Household
        // > View households -> Created and households in
        [Auth]
        [HttpGet]
        public ActionResult Index()
        {
            var url = $"{APIURL}/api/household/getall";

            var token = Request.Cookies["UserAuthCookie"].Value;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            // Check ITE
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData.Add("LoginMessage", "Internal Server Error, try again later");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var datas = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(responseResult);

                return View(datas);
            }
            else
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                foreach (var item in errorData.ModelState)
                {
                    foreach (var ItemValue in item.Value)
                    {
                        ModelState.AddModelError(string.Empty, ItemValue);
                    }
                }

                TempData.Add("LoginMessage", "Error in getting households");
                TempData.Add("MessageColour", "danger");

                return View();
            }
        }

        // GET: CreateHousehold
        [Auth]
        [HttpGet]
        public ActionResult CreateHousehold()
        {
            return View();
        }

        // POST: CreateHousehold
        [Auth]
        [HttpPost]
        public ActionResult CreateHousehold(CreateHouseholdViewModel createHouseholdViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/household/create";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", createHouseholdViewModel.Name),
                new KeyValuePair<string, string>("Description", createHouseholdViewModel.Description)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            // Check ITE
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData.Add("LoginMessage", "Internal Server Error");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Created!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                foreach (var item in errorData.ModelState)
                {
                    foreach (var ItemValue in item.Value)
                    {
                        ModelState.AddModelError(string.Empty, ItemValue);
                    }
                }

                return View();
            }
        }
    }
}