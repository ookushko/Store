using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Account;
using Store_MVC.Models.ViewModels.Shop;
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
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match.");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                db.Users.Add(userDTO);

                db.SaveChanges();

                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2 /* Роль обычного юзера (Хардкод) */
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            TempData["SM"] = "You are now registered and can login.";

            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

        [Authorize]
        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session["cart"] = null;

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        
        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;

            UserProfileVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                model = new UserProfileVM(dto);
            }

            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && !model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match.");
                return View("UserProfile", model);
            }

            using (Db db = new Db())
            {
                string userName = User.Identity.Name;

                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} already exists.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited your profile.";

            if (!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }

        }

        // GET: /account/orders
        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                UserDTO user = db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                int userId = db.Users.FirstOrDefault(x => x.Username == User.Identity.Name).Id;

                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray()
                    .Select(x => new OrderVM(x)).ToList();

                foreach (var order in orders)
                {
                    Dictionary<string, int> productsAndQuantity = new Dictionary<string, int>();

                    decimal total = 0m;

                    List<OrderDetailsDTO> orderDetailsList = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    foreach (var orderDetails in orderDetailsList)
                    {
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);

                        decimal price = product.Price;

                        string productName = product.Name;

                        productsAndQuantity.Add(productName, orderDetails.Quantity);

                        total += orderDetails.Quantity * price;
                    }

                    ordersForUser.Add(new OrdersForUserVM
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQuantity = productsAndQuantity,
                        CreatedAt = order.CreatedAt
                    });
                }
            }
            return View(ordersForUser);
        }
    }
}