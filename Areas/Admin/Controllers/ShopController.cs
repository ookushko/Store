using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Store_MVC.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            // Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            // Возвращаем List в представление
            return View(categoryVMList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string categoryName)
        {
            // Строковая переменная ID 
            string id;

            using (Db db = new Db())
            {
                // Проверить на уникальность
                if (db.Categories.Any(x => x.Name == categoryName)) { return "titletaken"; }

                // Иницализировать модель DTO
                CategoryDTO dto = new CategoryDTO();

                // Заполняем данными модель
                dto.Name = categoryName;
                dto.ShortDesc = categoryName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // Сохранить
                db.Categories.Add(dto);
                db.SaveChanges();

                // Получаем ID для возврата в представление 
                id = dto.Id.ToString();
            }

            // Вернуть ID в представление
            return id;
        }

        // Метод сортировки
        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {

            using (Db db = new Db())
            {
                // Реализуем начальный счётчик
                int count = 0;

                // Инициализируем модель данных
                CategoryDTO dto;

                // Устанавливаем сортирвку для каждой страницы
                foreach (var categoryId in id)
                {
                    dto = db.Categories.Find(categoryId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // Метод удаления записей
        // GET: Admin/Pages/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Получаем модель категории
                CategoryDTO dto = db.Categories.Find(id);

                // Удаляем категорию
                db.Categories.Remove(dto);

                // Сохраняем изменения в базе
                db.SaveChanges();
            }

            // Добавляем сообщение об успешном удалении
            TempData["SM"] = "You have deleted a category.";

            // Возвращаем пользователя
            return RedirectToAction("Categories");
        }

        // POST: Admin/Pages/DeleteCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // Проверяем уникальность имени
                if (db.Categories.Any(x => x.Name == newCatName)) { return "titletaken"; }

                // Получаем модель DTO
                CategoryDTO dto = db.Categories.Find(id);

                // Редактируем модель DTO
                dto.Name = newCatName;
                dto.ShortDesc = newCatName.Replace(" ", "-").ToLower();

                // Сохраняем изменения
                db.SaveChanges();
            }

            // Возвращаем результат
            return "";
        }

        // Добавление товаров
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Объявить модель данных
            ProductVM model = new ProductVM();

            // Добавляем список категорий
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Возвращаем модель в представление
            return View(model);
        }

        // Добавление товаров
        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid) // Если не валидная модель то берем из БД и возвращаем представление
            { 
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Проверяем имя продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "This product name is taken.");
                    return View(model);
                }
            }

            // Объявляем переменную ProductID
            int id;

            // Сохраняем в БД модель на основе ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.ShortDesc = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO categoryDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = categoryDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "You have added a product.";

            #region Upload img

            // Создаем необходимые ссылки на дериктории
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\UPloads"));

            var pathStr1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathStr2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathStr3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathStr4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathStr5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Проверка наличия директории, если нет дериктории, то создаем
            if (!Directory.Exists(pathStr1)) { Directory.CreateDirectory(pathStr1); }

            if (!Directory.Exists(pathStr2)) { Directory.CreateDirectory(pathStr2); }

            if (!Directory.Exists(pathStr3)) { Directory.CreateDirectory(pathStr3); }

            if (!Directory.Exists(pathStr4)) { Directory.CreateDirectory(pathStr4); }

            if (!Directory.Exists(pathStr5)) { Directory.CreateDirectory(pathStr5); }

            // Проверяем, был ли загружен файл
            if (file != null && file.ContentLength > 0)
            {
                // Получаем рамширение файла
                string ext = file.ContentType.ToLower();

                // Проверяем расширение файла 
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }
                }


                // Объявляем переменную с именем изображения
                string imageName = file.FileName;

                // Сохраняем имя изображения в модель DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Назначаем путь к оригинальному и уменьшеному изображению 
                var path = string.Format($"{pathStr2}\\{imageName}");
                var path2 = string.Format($"{pathStr3}\\{imageName}"); // Уменьшенное изображение

                // Сохраняем оригинальное изображение
                file.SaveAs(path);

                // Создаем и сохраняем уменьшеную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            #endregion

            // Переадресация
            return RedirectToAction("AddProduct");
        }
    }
}