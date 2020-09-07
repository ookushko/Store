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
            TempData["SM"] = "You added a new Page"; // Successful message - сообщение об успешной операции

            // Переадресовываем пользователя на метод Index
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                // Проверяем доступна ли страница
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Инициализируем модель данными
                model = new PageVM(dto);
            }

            // Возвращаем представление с моделью
            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Проверить на валидность модель
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Получить Id страницы
                int id = model.Id;

                // Объявим переменную для ShortDesc
                string shortDesc = null;

                // Получаем страницу (по Id)
                PagesDTO dto = db.Pages.Find(id);

                // Присваеваем название из полученой модели в DTO
                dto.Title = model.Title;

                // Проверяем краткий заголовок и присваеваем его, если необходимо
                if (string.IsNullOrWhiteSpace(model.ShortDesc))
                {
                    shortDesc = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    shortDesc = model.ShortDesc.Replace(" ", "-").ToLower();
                }

                // Проверить Title & ShortDesc на уникальность 
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.ShortDesc == shortDesc))
                {
                    ModelState.AddModelError("", "That short description alredy exist.");
                    return View(model);
                }

                // Присвоить остальные значения в класс DTO
                dto.ShortDesc = shortDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Сохраняем изменения в БД
                db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "You have edited the page.";

            // Вернуть пользователя обратно
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Объявим модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Получить страницу
                PagesDTO dto = db.Pages.Find(id);

                // Подтвердаем доступность страницы
                if (dto == null)
                {
                    return Content("The pages does  not exist.");
                }

                // Присваеваем модели информацию из базы
                model = new PageVM(dto);
            }
            // Возвращаем модель в представление
            return View(model);
        }

        // Метод удаления записей (5)
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                // Удаляем страницу
                db.Pages.Remove(dto);

                // Сохраняем изменения в базе
                db.SaveChanges();
            }

            // Добавляем сообщение об успешном удалении
            TempData["SM"] = "You have deleted a page.";

            // Возвращаем пользователя
            return RedirectToAction("Index");
        }

        // Метод сортировки
        // GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {

            using (Db db = new Db())
            {
                // Реализуем начальный счётчик
                int count = 0;

                // Инициализируем модель данных
                PagesDTO dto;

                // Устанавливаем сортирвку для каждой страницы
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Объявляем модель
            SidebarVM model;

            using (Db db = new Db())
            {
                // Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1); // Жесткие значения в коде не желательно добавлять, ИСПРАВИТЬ!

                // Заполняем модель данными
                model = new SidebarVM(dto);
            }

            // Вернуть представление с моделью
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                // Получить данные из DTO 
                SidebarDTO dto = db.Sidebars.Find(1); // Жесткие значения в коде не желательно добавлять, ИСПРАВИТЬ!

                // Присваеваем данные в тело (в св-во Body)
                dto.Body = model.Body; // Из представления сохраняем в модель которая работает с БД

                // Сохраняем в БД
                db.SaveChanges();
            }
            // Присваиваем сообщение в TempData
            TempData["SM"] = "You have edited the sidebar.";

            // Переадресация
            return RedirectToAction("EditSidebar");
        }
    }
}