using Store_MVC.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CartPartial()
        {
            // Объявляем модель CartVM
            CartVM model = new CartVM();

            // Объявляем переменную количества
            int quantety = 0;

            // Объявляем переменную цены
            decimal price = 0;

            // Проверяем сессию корзины
            if (Session["cart"] != null)
            {
                // Получаем общее количество товаров и цену
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    quantety += item.Quantity;
                    price += item.Quantity * item.Price;
                }
            }
            else
            {
                // Или устанавливаем количество и цену = 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CartPartial", model);
        }
    }
}