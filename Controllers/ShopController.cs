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
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Where(x => x.ShortDesc == name).FirstOrDefault();
                int categoryId = dto.Id;

                productVMList = db.Products.ToArray().Where(x => x.CategoryId == categoryId).Select(x => new ProductVM(x)).ToList();

                var productCategory = db.Products.Where(x => x.CategoryId == categoryId).FirstOrDefault();

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

            return View(productVMList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductDTO dto;
            ProductVM model;

            int id = 0;
            using (Db db = new Db())
            {
                if (!db.Products.Any(x => x.ShortDesc.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                dto = db.Products.Where(x => x.ShortDesc == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductVM(dto);
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            return View("ProductDetails", model);
        }
    }
}