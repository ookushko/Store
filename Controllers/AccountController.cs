using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Подтвердить что пользователь не авторизован
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // Возвращаем представление
            return View();
        }

    }
}