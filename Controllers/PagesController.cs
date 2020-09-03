using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Получаем/Устанавливаем ShortDesc 
            if (page == "")
            {
                page = "home";
            }

            // Объявляем модель и DTO
            PageVM model;
            PagesDTO dto;

            // Проверяем доступность текущей страницы 
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.ShortDesc.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Получаем DTO страницы
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.ShortDesc == page).FirstOrDefault();
            }

            // Устанавливаем Title // ViewBag - Динамический тип, не требует строгой типизации, сам определяет что в него передали
            ViewBag.PageTitle = dto.Title; 

            // Проверяем боковую панель
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // Заполняем модель данными
            model = new PageVM(dto);

            // Возвращаем представление с моделью
            return View(model);
        }

        
    }
}