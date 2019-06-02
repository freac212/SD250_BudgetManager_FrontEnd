using Newtonsoft.Json;
using SD220_Deliverable_1_DGrouette.Models.Filters;
using SD250_Deliverable_tmp_DGrouette.Models.Domain;
using SD250_Deliverable_tmp_DGrouette.Models.Helpers;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Controllers
{
    public class HouseholdController : Controller
    {
        private const string APIURL = "http://localhost:52445";
        private static readonly HttpClient httpClient = new HttpClient(); // Apparently this was reccomended! <- Will solve the fact that I'm

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
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var datas = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(responseResult);

                return View(datas);
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                if (errorData.ModelState != null)
                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            ModelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }
                else
                {
                    TempData.Add("LoginMessage", "Unknown Error, call an admin or something");
                    TempData.Add("MessageColour", "danger");
                }

                return View();
            }
        }

        // GET: CreateHousehold
        [Auth]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreateHousehold
        [Auth]
        [HttpPost]
        public ActionResult Create(CreateHouseholdViewModel createHouseholdViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/household/create";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", createHouseholdViewModel.Name),
                new KeyValuePair<string, string>("Description", createHouseholdViewModel.Description)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Created!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                if (errorData.ModelState != null)
                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            ModelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }
                else
                {
                    TempData.Add("LoginMessage", "Unknown Error, call an admin or something");
                    TempData.Add("MessageColour", "danger");
                }

                return View();
            }
        }


        // GET: Edit
        [Auth]
        [HttpGet]
        public ActionResult Edit(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            // get the household from the DB, I could pass the local data, but if someone else edits the household, I'd prefer the data is straight form the source.
            var url = $"{APIURL}/api/household/getbyid/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???


            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<EditHouseholdViewModel>(responseResult);

                return View(data);
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                if (errorData.ModelState != null)
                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            ModelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorData.Message);
                }

                return View();
            }
        }

        // POST: Edit
        [Auth]
        [HttpPost]
        public ActionResult Edit(EditHouseholdViewModel editHouseholdViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/household/edit/{editHouseholdViewModel.Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", editHouseholdViewModel.Name),
                new KeyValuePair<string, string>("Description", editHouseholdViewModel.Description)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Edited!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                if (errorData != null)
                {
                    foreach (var item in errorData.ModelState)
                    {
                        foreach (var ItemValue in item.Value)
                        {
                            ModelState.AddModelError(string.Empty, ItemValue);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorData.Message);
                }

                return View();
            }
        }

        // POST: Delete // Is a get here okay? I tried Delete but that didn't work all too well.
        [Auth]
        [HttpGet]
        public ActionResult Delete(int? Id)
        {
            if (ErrorHelpers.IdIsInvalid(Id, TempData))
                return RedirectToAction("Index", "Household");

            var url = $"{APIURL}/api/household/delete/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Handling lack of connection??? try catch?
            var response = httpClient.DeleteAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Deleted!");
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

        // GET: HouseholdUsers
        // > View households -> Created and households in
        [Auth]
        [HttpGet]
        public ActionResult Users(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{APIURL}/api/household/getallusers/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

                var datas = JsonConvert.DeserializeObject<List<UserViewModel>>(responseResult);
                var viewModel = new HouseholdUserViewModel
                {
                    UserViewModels = datas,
                    HouseholdId = (int)Id,
                    IsCreator = IsUserCreator((int)Id)
                };

                return View(viewModel);
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                foreach (var item in errorData.ModelState)
                {
                    foreach (var ItemValue in item.Value)
                    {
                        ModelState.AddModelError(string.Empty, ItemValue);
                    }
                }

                TempData.Add("LoginMessage", "Error in getting users");
                TempData.Add("MessageColour", "danger");

                return View();
            }
        }

        // GET: Invite
        [Auth]
        [HttpGet]
        public ActionResult Invite(int? HouseholdId)
        {
            if (HouseholdId is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }
            var viewModel = new InviteUserViewModel
            {
                HouseholdId = (int)HouseholdId
            };

            return View(viewModel);
        }

        // POST: Invite
        [Auth]
        [HttpPost]
        public ActionResult Invite(InviteUserViewModel inviteUserViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var url = $"{APIURL}/api/household/invite/{inviteUserViewModel.HouseholdId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            var callbackUrl = Url.Action("Join", "Household", new { Id = inviteUserViewModel.HouseholdId }, Request.Url.Scheme);

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", inviteUserViewModel.Email),
                new KeyValuePair<string, string>("CallbackUrl", callbackUrl),

            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            // Check ITE
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData.Add("LoginMessage", "Internal Server Error, try again later");
                TempData.Add("MessageColour", "danger");
                return View(inviteUserViewModel);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData.Add("LoginMessage", "User does not exist on the system");
                TempData.Add("MessageColour", "danger");
                return View(inviteUserViewModel);
            }

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "User Invited to the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                ModelState.AddModelError(string.Empty, errorData.Message);

                TempData.Add("LoginMessage", "Error in inviting users");
                TempData.Add("MessageColour", "danger");

                return View(inviteUserViewModel);
            }
        }


        // GET: Join
        [Auth]
        [HttpGet]
        public ActionResult Join(int? Id)
        {
            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{APIURL}/api/household/join/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader; // Set this on login I guess???

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("null", null),
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);

            // Handling lack of connection??? try catch?
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "You've joined the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                ModelState.AddModelError(string.Empty, errorData.Message);

                TempData.Add("LoginMessage", "Error in joining household");
                TempData.Add("MessageColour", "danger");

                return RedirectToAction("Index", "Household");
            }
        }

        // GET: Leave
        [Auth]
        [HttpGet]
        public ActionResult Leave(int? Id)
        {
            if (ErrorHelpers.IdIsInvalid(Id, TempData))
                return RedirectToAction("Index", "Household");

            var url = $"{APIURL}/api/household/leave/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "You've left the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                var errorData = JsonConvert.DeserializeObject<ErrorData>(responseResult);

                ModelState.AddModelError(string.Empty, errorData.Message);

                TempData.Add("LoginMessage", "Error in leaving household");
                TempData.Add("MessageColour", "danger");

                return RedirectToAction("Index", "Household");
            }
        }

        // GET: Invites
        [Auth]
        [HttpGet]
        public ActionResult Invites()
        {
            var url = $"{APIURL}/api/household/getallinvites";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
                return RedirectToAction("Index", "Household");

            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var datas = JsonConvert.DeserializeObject<List<InviteViewModel>>(responseResult);

                return View(datas);
            }
            else
            {
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

        private bool IsUserCreator(int householdId)
        {
            var url = $"{APIURL}/api/household/isusercreator/{householdId}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = httpClient.GetAsync(url).Result;

            if (ErrorHelpers.IsNotFound(response.StatusCode, TempData))
                return false;
            if (ErrorHelpers.IsInternalServerError(response.StatusCode, TempData))
                return false;
            if (ErrorHelpers.IsUnAuthorized(response.StatusCode, TempData))
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
    }
}