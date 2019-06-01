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

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            // Check ITE
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Internal Server Error");
            }

            if (response.IsSuccessStatusCode)
            {
                // Should 'login' the user here
                if (!LogUserIn(registerUserViewModel.Email, registerUserViewModel.Password))
                {
                    ModelState.AddModelError("", "There was an error logging you in..");
                    return View();
                }
                else
                {
                    TempData.Add("LoginMessage", "Account Created!");
                    return RedirectToAction("Index", "Home");
                }
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

        // GET: Login
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult LogIn(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            if (LogUserIn(loginViewModel.Email, loginViewModel.Password))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                return View();
            }
        }

        // GET: Login
        [HttpGet]
        [Auth]
        public ActionResult LogOut()
        {
            // Bassically removing the cookie, in the goofiest way possible.
            if (Request.Cookies["UserAuthCookie"] != null)
            {
                HttpCookie myCookie = new HttpCookie("UserAuthCookie");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            else
            {
                TempData.Add("LoginMessage", "Error logging out..");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }





        // GET: RecoverPassword
        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        // POST: RecoverPassword
        [HttpPost]
        public ActionResult RecoverPassword(RecoverPasswordViewModel passwordViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/Account/SendResetPassword";
            //var callbackUrl = Url.Content("~/Account/ResetPassword/");
            var callbackUrl = Url.Action("ResetPassword", "Account", null, Request.Url.Scheme);

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", passwordViewModel.Email),
                new KeyValuePair<string, string>("Url", callbackUrl)
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
                return RedirectToAction("Index", "Home");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", $"If that account exists, an email has been sent!");
                //TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Home");
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




        //GET ResetPassword
        [HttpGet]
        public ActionResult ResetPassword(string token, string email)
        {
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(email))
            {
                TempData.Add("LoginMessage", "Error");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var passwordViewModel = new ResetPasswordViewModel()
                {
                    Token = token,
                    Email = email
                };

                return View(passwordViewModel);
            }
        }

        //POST ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/Account/ResetPassword";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("NewPassword", resetPasswordViewModel.Password),
                new KeyValuePair<string, string>("ConfirmPassword", resetPasswordViewModel.ConfirmPassword),
                new KeyValuePair<string, string>("Token", resetPasswordViewModel.Token),
                new KeyValuePair<string, string>("Email", resetPasswordViewModel.Email)
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
                return RedirectToAction("Index", "Home");
            }

            if (response.IsSuccessStatusCode)
            {
                // Log user in
                if (!LogUserIn(resetPasswordViewModel.Email, resetPasswordViewModel.Password))
                {
                    ModelState.AddModelError("", "There was an error logging you in..");
                    return View();
                }
                else
                {
                    TempData.Add("LoginMessage", "Password Changed!");
                    return RedirectToAction("Index", "Home");
                }
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


        //GET ChangePassword
        [HttpGet]
        [Auth]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //POST ChangePassword
        [HttpPost]
        [Auth]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/Account/ChangePassword";

            var token = Request.Cookies["UserAuthCookie"].Value;

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OldPassword", changePasswordViewModel.OldPassword),
                new KeyValuePair<string, string>("NewPassword", changePasswordViewModel.NewPassword),
                new KeyValuePair<string, string>("ConfirmPassword", changePasswordViewModel.ConfirmPassword),
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            // Check ITE
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData.Add("LoginMessage", "Internal Server Error");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Home");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Password Changed!");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            ModelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }

                return View();
            }
        }



        private bool LogUserIn(string email, string password)
        {
            var url = $"{APIURL}/token";
            var grantType = "password";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", grantType)
            };

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.IsSuccessStatusCode)
            {

                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<LoginData>(data);

                var cookie = new HttpCookie("UserAuthCookie", // Does naming convention matter?
                        result.AccessToken);
                var cookieUser = new HttpCookie("UserCookie", // Does naming convention matter?
                        email);

                Response.Cookies.Add(cookie);
                Response.Cookies.Add(cookieUser);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
