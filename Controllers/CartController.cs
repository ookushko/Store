using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Store_MVC.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(cart);
        }

        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();

            int quantity = 0;

            decimal price = 0;

            if (Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    quantity += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = quantity;
                model.Price = price;
            }
            else
            {
                model.Quantity = 0;
                model.Price = 0m;
            }

            return PartialView("_CartPartial", model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                ProductDTO product = db.Products.Find(id);

                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                if (productInCart == null)
                {
                    cart.Add(new CartVM() 
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
            }
            int qty = 0; // qty - quantity
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            Session["cart"] = cart;

            return PartialView("_AddToCartPartial", model);
        }

        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                model.Quantity++;

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Cart/DecrementProduct
        public JsonResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemoveProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }

        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }

        // POST: /cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            string userName = User.Identity.Name;

            List<string> mailOrderList = new List<string>() { "\nOrdered goods:\n" };

            int orderId = 0;

            using (Db db = new Db())
            {
                OrderDTO orderDTO = new OrderDTO();

                int userId = db.Users.FirstOrDefault(x => x.Username == userName).Id;
                
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO);
                db.SaveChanges();

                orderId = orderDTO.OrderId;

                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();

                    mailOrderList.Add($"Product name: {item.ProductName}, Price: {item.Quantity}x{item.Price} = {item.Total}$");
                }
            }

            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("6837973d3e1464", "2135485833438a"),
                EnableSsl = true
            };

            string msg = $"You have a new order. Order number: {orderId}\n" +
                $"{string.Join("\n", mailOrderList)}";

            client.Send("shopmvc@example.com", "admin@example.com", "New Order", msg);
            Session["cart"] = null;
        }
    }
}