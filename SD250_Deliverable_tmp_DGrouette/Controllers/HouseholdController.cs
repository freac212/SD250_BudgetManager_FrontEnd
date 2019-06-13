using Newtonsoft.Json;
using SD220_Deliverable_1_DGrouette.Models.Filters;
using SD250_Deliverable_tmp_DGrouette.Models.Bindings;
using SD250_Deliverable_tmp_DGrouette.Models.Domain;
using SD250_Deliverable_tmp_DGrouette.Models.Helpers;
using SD250_Deliverable_tmp_DGrouette.Models.Views;
using SD250_Deliverable_tmp_DGrouette.Models.Wrappers;
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
        // Rule of cleaning this up -> Go through each method, make sure they're only returning/ create/ whatever/ what they should be, look very carefully!
        // Also, clean up the viewModels, the Create views are being used in edits, others being used elsewhere so, clean that up.
        // Run through each views to make sure they look right, I know some buttons are pretty big so adjust those.
        // JQuery Arrow images
        // Does the Catagory, transaction, and account management rely on households? Or are they independant lists? (Because if they were I'd add lists to the side bar and have to rewrite a few queries)

        // GET: Household
        // > View households -> Created and households in
        [Auth]
        [HttpGet]
        public ActionResult Index()
        {
            var url = $"{ProjectConstants.APIURL}/api/household/getall";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            //HttpClientContext.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}"); // Old way

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(responseResult);

                return View(datas);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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
                return View(createHouseholdViewModel);

            var url = $"{ProjectConstants.APIURL}/api/household/create";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", createHouseholdViewModel.Name),
                new KeyValuePair<string, string>("Description", createHouseholdViewModel.Description)
            };

            // x-www-form-encoded tag, just like in post man, so that the data is sent on the body.
            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Created!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(createHouseholdViewModel);
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

            var url = $"{ProjectConstants.APIURL}/api/household/getbyid/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<EditHouseholdViewModel>(responseResult);

                return View(data);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }
        }

        // POST: Edit
        [Auth]
        [HttpPost]
        public ActionResult Edit(EditHouseholdViewModel editHouseholdViewModel)
        {
            if (!ModelState.IsValid)
                return View(editHouseholdViewModel);

            var url = $"{ProjectConstants.APIURL}/api/household/edit/{editHouseholdViewModel.Id}";

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", editHouseholdViewModel.Name),
                new KeyValuePair<string, string>("Description", editHouseholdViewModel.Description)
            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", $"Household '{editHouseholdViewModel.Name}' Edited!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View(editHouseholdViewModel);
            }
        }

        // GET: Delete // Is a get here okay? I tried Delete but that didn't work all too well.
        [Auth]
        [HttpGet]
        public ActionResult Delete(int? Id)
        {
            if (ErrorHelpers.IdIsInvalid(Id, TempData))
                return RedirectToAction("Index", "Household");

            var url = $"{ProjectConstants.APIURL}/api/household/delete/{Id}";

            var response = HttpClientContext.httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "Household Deleted!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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

            var url = $"{ProjectConstants.APIURL}/api/household/getallusers/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;
            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

                var datas = JsonConvert.DeserializeObject<List<UserViewModel>>(responseResult);
                var viewModel = new HouseholdUserViewModel
                {
                    UserViewModels = datas,
                    HouseholdId = (int)Id,
                    IsCreator = IsUserCreator((int)Id, Request, TempData)
                };

                return View(viewModel);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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
                return View(inviteUserViewModel);

            var url = $"{ProjectConstants.APIURL}/api/household/invite/{inviteUserViewModel.HouseholdId}";

            var callbackUrl = Url.Action("Join", "Household", new { Id = inviteUserViewModel.HouseholdId }, Request.Url.Scheme);

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", inviteUserViewModel.Email),
                new KeyValuePair<string, string>("CallbackUrl", callbackUrl),

            };

            var encodedParameters = new FormUrlEncodedContent(parameters);
            var response = HttpClientContext.httpClient.PostAsync(url, encodedParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "User Invited to the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
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

            var url = $"{ProjectConstants.APIURL}/api/household/join/{Id}";

            var response = HttpClientContext.httpClient.PostAsync(url, null).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "You've joined the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("LogIn", "Account"); // Could I somehow have the user login, then have it redirect to join?? ++Q
            }
        }

        // GET: Leave
        [Auth]
        [HttpGet]
        public ActionResult Leave(int? Id)
        {
            if (ErrorHelpers.IdIsInvalid(Id, TempData))
                return RedirectToAction("Index", "Household");

            var url = $"{ProjectConstants.APIURL}/api/household/leave/{Id}";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData.Add("LoginMessage", "You've left the household!");
                return RedirectToAction("Index", "Household");
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return RedirectToAction("Index", "Household");
            }
        }

        // GET: Invites
        [Auth]
        [HttpGet]
        public ActionResult Invites()
        {
            var url = $"{ProjectConstants.APIURL}/api/household/getallinvites";

            var response = HttpClientContext.httpClient.GetAsync(url).Result;
            var responseResult = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var datas = JsonConvert.DeserializeObject<List<InviteViewModel>>(responseResult);

                return View(datas);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }
        }

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

        [Auth]
        [HttpGet]
        public ActionResult Details(int? Id)
        {
            // HouseholdId
            // Last 10 Transactions of those accounts/ grouped by category. -> Do that on that backend

            if (Id is null)
            {
                TempData.Add("LoginMessage", "Improper Id");
                TempData.Add("MessageColour", "danger");
                return RedirectToAction("Index", "Household");
            }

            var url = $"{ProjectConstants.APIURL}/api/household/getbyid/{Id}";

            var token = Request.Cookies["UserAuthCookie"].Value;
            var authHeader = new AuthenticationHeaderValue("Bearer", token);
            HttpClientContext.httpClient.DefaultRequestHeaders.Authorization = authHeader;

            // Handling lack of connection??? try catch?
            var response = HttpClientContext.httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;

                var datas = JsonConvert.DeserializeObject<HouseholdBindingModel>(responseResult);
                // Organize data into HouseholdDetailsViewModel
                var householdDetails = new HouseholdDetailsViewModel()
                {
                    NetSum = datas.BankAccounts.Sum(acnt => acnt.Balance),
                    HouseholdId = (int)Id,
                    BankAccountViewModels = datas.BankAccounts.Select(p => new BankAccountTransactionsViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Balance = p.Balance,
                        TransactionViewModels = datas.Transactions.Where(trnsctn => trnsctn.BankAccountId == p.Id).Select(x => new TransactionViewModel()
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Description = x.Description,
                            Date = x.Date,
                            DateCreated = x.DateCreated,
                            DateUpdated = x.DateUpdated,
                            IsVoid = x.IsVoid,
                            Amount = x.Amount,
                            CreatorId = x.CreatorId,
                            CategoryId = x.CategoryId,
                            CategoryName = HouseholdHelpers.GetCategoryName(x.CategoryId, Request)
                        }).GroupBy(transaction => transaction.CategoryName).ToList()
                    }).ToList()
                };

                return View(householdDetails);
            }
            else
            {
                ErrorHelpers.HandleResponseErrors(response, TempData, ModelState);
                return View();
            }

        }
    }
}