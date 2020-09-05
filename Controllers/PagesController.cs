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
        public ActionResult Index(string page = "home") // Получаем/Устанавливаем ShortDesc
        {
            // Проверяем доступность текущей страницы 
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.ShortDesc.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Объявляем DTO
            PagesDTO dto;

            // Получаем DTO страницы
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.ShortDesc == page).FirstOrDefault();
            }

            // Устанавливаем Title // ViewBag - Динамический тип, не требует строгой типизации, сам определяет что в него передали
            ViewBag.PageTitle = dto.Title;

            // Проверяем боковую панель
            ViewBag.Sidebar = dto.HasSidebar ? "Yes" : "No";

            // Объявляем и заполняем модель данными
            PageVM model = new PageVM(dto);

            // Возвращаем представление с моделью
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Инициализируем лист PageVM
            List<PageVM> pageVMList;

            // Получаем все страницы, кроме HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting)
                    .Where(x => x.ShortDesc != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            // Возвращаем частичное представление и лист данныхв
            return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Объявляем модель данных
            SidebarVM model;

            // Инициализируем модель
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); // В последствии переделать

                model = new SidebarVM(dto);
            }

            // Возвращаем модель в частичное представление
            return PartialView("_SidebarPartial", model);
        }
    }
}