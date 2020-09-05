using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        // Метод для просмотра детальной информации о продукте
        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Объявляем модели DTO и VM
            ProductDTO dto;
            ProductVM model;

            // Инициализируем id продукта
            int id = 0;
            using (Db db = new Db())
            {
                // Проверяем доступность
                if (!db.Products.Any(x => x.ShortDesc.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Инициализируем модель DTO данными
                dto = db.Products.Where(x => x.ShortDesc == name).FirstOrDefault();

                // Получаем id
                id = dto.Id;

                // Инициализируем модель VM данными
                model = new ProductVM(dto);
            }

            // Получаем изображение из гелереи
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Возвращаем модель в представление
            return View("ProductDetails", model);
        }
    }
}