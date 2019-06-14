using Newtonsoft.Json;
using SD220_Deliverable_1_DGrouette.Models.Filters;
using SD250_Deliverable_tmp_DGrouette.Models.Domain;
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
    public class AccountController : Controller
    {
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

            var url = $"{ProjectConstants.APIURL}/api/Account/Register";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", registerUserViewModel.Email),
                new KeyValuePair<string, string>("Password", registerUserViewModel.Password),
                new KeyValuePair<string, string>("ConfirmPassword", registerUserViewModel.ConfirmPassword)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                if (!LogUserIn(registerUserViewModel.Email, registerUserViewModel.Password))
                {
                    ModelState.AddModelError("", "There was an error logging you in..");
                    return View();
                }
                else
                {
                    TempData.Add("LoginMessage", "Account Created!");
                    return RedirectToAction("Index", "Household");
                }
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                return View();
            }
        }


        // GET: LogOut
        [HttpGet]
        [Auth]
        public ActionResult LogOut()
        {
            // Bassically removing the cookie, in the goofiest way possible.
            if (Request.Cookies["UserAuthCookie"] != null)
            {
                HttpCookie myCookie = new HttpCookie("UserAuthCookie")
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(myCookie);
            }
            else
            {
                TempData.Add("LoginMessage", "Error logging out..");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            return RedirectToAction("Index", "Household");
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
                return View(); // Not sending the viewModel because passwords

            var url = $"{ProjectConstants.APIURL}/api/Account/SendResetPassword";
            var callbackUrl = Url.Action("ResetPassword", "Account", null, Request.Url.Scheme);

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", passwordViewModel.Email),
                new KeyValuePair<string, string>("Url", callbackUrl)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", $"If that account exists, an email has been sent!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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
                return RedirectToAction("Index", "Household");
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

            var url = $"{ProjectConstants.APIURL}/api/Account/ResetPassword";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("NewPassword", resetPasswordViewModel.Password),
                new KeyValuePair<string, string>("ConfirmPassword", resetPasswordViewModel.ConfirmPassword),
                new KeyValuePair<string, string>("Token", resetPasswordViewModel.Token),
                new KeyValuePair<string, string>("Email", resetPasswordViewModel.Email)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

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
                    return RedirectToAction("Index", "Household");
                }
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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

            var url = $"{ProjectConstants.APIURL}/api/Account/ChangePassword";

            var token = Request.Cookies["UserAuthCookie"].Value;

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("OldPassword", changePasswordViewModel.OldPassword),
                new KeyValuePair<string, string>("NewPassword", changePasswordViewModel.NewPassword),
                new KeyValuePair<string, string>("ConfirmPassword", changePasswordViewModel.ConfirmPassword),
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Password Changed!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }
        }


        private bool LogUserIn(string email, string password)
        {
            var url = $"{ProjectConstants.APIURL}/token";
            var grantType = "password";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", grantType)
            };

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = HttpClientContext.httpClient.PostAsync(url, encodedValues).Result;

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

                var token = Request.Cookies["UserAuthCookie"].Value;
                var authHeader = new AuthenticationHeaderValue("Bearer", token);
                HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
