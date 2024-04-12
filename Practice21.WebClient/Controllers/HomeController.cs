using Microsoft.AspNetCore.Mvc;
using Practice21.WebClient.Models;
using System.Diagnostics;
using Practice21.WPFClient;
using Practice21.MinimalAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Practice21.MinimalAPI.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;

namespace Practice21.WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public bool IsUserLoggedIn { get; set; } = false;
        PhoneBookModel? model;

        public HomeController(ILogger<HomeController> logger, PhoneBookModel model)
        {
            this.model = model;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (model != null)
            {
                return View();
            }
            else
                return Error();
        }

        #region WorkWithUsers
        [HttpGet]
        public IActionResult Login()
        {
            if (model != null)
            {
                return View("Login");
            }
            else
                return Error();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (model != null)
            {
                if (user != null)
                {
                    if (model.LoginUser(user.Login!, user.Password!))
                    {
                        user = model.CurrentUser;
                        _logger.LogInformation("User {0} logged in", user.Login);
                        return Redirect("Index");
                    }
                    else
                        return View("AccessDenied");
                }
                else
                    return Error();
            }
            else
                return Error();
        }

        public IActionResult Logout()
        {
            if (model != null)
            {
                if (model.LogoutUser())
                {
                    _logger.LogInformation("User logged out");
                    return Redirect("Index");
                }
                else
                    return Error();
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult EditUsers()
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                    return View("EditUsers");
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            if (model != null)
            {

                if (model.CurrentUser.RoleId == 1)
                {
                    User user = new User();
                    return View("UserInfo", user);
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                {
                    if (model.AddUser(user))
                    {
                        _logger.LogInformation($"Added user {user}");
                        return Redirect("EditUsers");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                    return View("UserInfo", model.Users.Where(u => u.Id == id).First());
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpPost]
        public IActionResult UpdateUser(User user)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                {
                    if (model.UpdateUser(user))
                    {
                        _logger.LogInformation($"Updated user {user}");
                        return Redirect("EditUsers");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                {
                    if (model.DeleteUser(model.Users.Where(e => e.Id == id).First()))
                    {
                        _logger.LogInformation($"Deleted user {id}");
                        return View("EditUsers");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }
        #endregion

        #region WorkWithEntries
        [HttpGet]
        public IActionResult AddEntry()
        {
            if (model != null)
            {

                if (model.CurrentUser.RoleId > 0)
                {
                    PhoneBookEntry phoneBookEntry = new PhoneBookEntry();
                    return View("AddEntry", phoneBookEntry);
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpPost]
        public IActionResult CreateEntry(PhoneBookEntry entry)
        {
            if (model != null)
            {
                string phoneNumberString = Request.Form["PhoneNumber"]!;
                _logger.LogError($"phoneNumberString = {phoneNumberString}");
                if (model.CurrentUser.RoleId == 1 || (model.CurrentUser.RoleId == 2 && entry.Id == 0))
                {
                    if (model.AddEntry(entry))
                    {
                        _logger.LogInformation($"Added entry {entry}");
                        return Redirect("Index");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult EditEntry(int id)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                    return View("AddEntry", model.PhoneBookEntries.Where(e => e.Id == id).First());
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpPost]
        public IActionResult UpdateEntry(PhoneBookEntry entry)
        {
            if (model != null)
            {
                entry.PhoneNumber = Convert.ToInt64(Regex.Replace(Request.Form["PhoneNumber"]!, "[^0-9]", ""));
                if (model.CurrentUser.RoleId == 1)
                {
                    if (model.UpdateEntry(entry))
                    {
                        _logger.LogInformation($"Updated entry {entry}");
                        return Redirect("Index");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        [HttpGet]
        public IActionResult DeleteEntry(int id)
        {
            if (model != null)
            {
                if (model.CurrentUser.RoleId == 1)
                {
                    if (model.DeleteEntry(model.PhoneBookEntries.Where(e => e.Id == id).First()))
                    {
                        _logger.LogInformation($"Deleted entry {id}");
                        return View("Index");
                    }
                    else
                        return Error();
                }
                else
                    return View("AccessDenied");
            }
            else
                return Error();
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
