using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            // Инициализируем модель 
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // Метод выводит список товаров по категориям
        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            // Объявляем список типа List
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // Получаем id категории
                CategoryDTO dto = db.Categories.Where(x => x.ShortDesc == name).FirstOrDefault();

                int catId = dto.Id;

                // Инициализируем наш список данными
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                //Получаем имя категории
                var productCategory = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                // Проверяем на null
                if (productCategory == null)
                {
                    var catName = db.Categories.Where(x => x.ShortDesc == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCategory.CategoryName;
                }
            }

            //Возвращаем представление с моделью
            return View(productVMList);
        }
    }
}