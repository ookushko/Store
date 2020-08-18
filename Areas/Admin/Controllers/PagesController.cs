using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Объявляем список для представения (PageVM)
            List<PageVM> pageList;

            // Инициализируем список (БД)
            using (Db db = new Db())
            {   // Присваеваем объекты через подключение базы, в массив, затем сортируем, и добавляем отсартероваными в список
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            // Возвращаем список в представление
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Проверка моделей на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Объявляем переменную для краткого описания (ShortDesc)
                string shortDesc;

                // Инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                // Присваеваем заголовок модели (должен быть уникален)
                dto.Title = model.Title.ToUpper();

                // Проверить наличие краткого описания, если нет, то присваеваем
                if (string.IsNullOrWhiteSpace(model.ShortDesc))
                {
                    shortDesc = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    shortDesc = model.ShortDesc.Replace(" ", "-").ToLower();
                }

                // Убеждаемся что заголовок и краткое описание уникален
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.ShortDesc == model.ShortDesc))
                {
                    ModelState.AddModelError("", "That short description already exist");
                    return View(model);
                }

                // Присваеваем оставшиеся значения модели
                dto.ShortDesc = shortDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // Сохраняем в БД
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Передаём сообщение через TempData
            TempData["SM"] = "You added a new record"; // Successful message - сообщение об успешной операции

            // Переадресовываем пользователя на метод Index
            return RedirectToAction("Index");
        }
    }
}