using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // Проверяем соответствие пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match.");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                // Проверяем уникально ли имя
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                // Создаём экземпляр класса UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                // Добавляем данные в модель
                db.Users.Add(userDTO);

                // Сохраняем данные
                db.SaveChanges();

                // Добавляем роль пользователю
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2 /* Роль обычного юзера */
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // Записываем сообщение в TempData
            TempData["SM"] = "You are now registered and can login.";

            // Переадресовываем пользователя
            return RedirectToAction("Login");
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

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверяем пользователя на валидность
            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserNavPartial()
        {
            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем модель
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // Получаем пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                // Заполняем модель данными из контекста dto
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            // Возвращаем частичное представление представление
            return PartialView(model);
        }

    }
}