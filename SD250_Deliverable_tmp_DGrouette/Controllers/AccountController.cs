using Newtonsoft.Json;
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
    public class AccountController : Controller
    {
        // Save api url here orrrr?
        private const string APIURL = "http://localhost:52445";
        private static readonly HttpClient httpClient = new HttpClient(); // Apparently this was reccomended!

        // Tasks: 
        // > Register User
        // > Login w/ token auth
        // > Change Password
        // > Recover Lost Password

        // GET: RegisterUser
        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }

        // POST: RegisterUser
        [HttpPost]
        public ActionResult RegisterUser(RegisterUserViewModel registerUserViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/Account/Register";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", registerUserViewModel.Email),
                new KeyValuePair<string, string>("Password", registerUserViewModel.Password),
                new KeyValuePair<string, string>("ConfirmPassword", registerUserViewModel.ConfirmPassword)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PutAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Account Created!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Error messages from the API, where do those come from?
                ModelState.AddModelError("", response.Content.ReadAsStringAsync().Result);
                return View(ModelState);
            }
        }

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            var url = $"{APIURL}/token";
            var grantType = "password";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", loginViewModel.Email),
                new KeyValuePair<string, string>("password", loginViewModel.Password),
                new KeyValuePair<string, string>("grant_type", grantType)
            };

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            var data = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<LoginData>(data);

            var cookie = new HttpCookie("UserAuthCookie", // Does naming convention matter?
                result.AccessToken);

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }
    }
}
