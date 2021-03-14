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
        public ActionResult Index(string page = "home")
        {
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.ShortDesc.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            PagesDTO dto;

            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.ShortDesc == page).FirstOrDefault();
            }

            ViewBag.PageTitle = dto.Title;

            ViewBag.Sidebar = dto.HasSidebar ? "Yes" : "No";

            PageVM model = new PageVM(dto);

            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageVMList;

            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting)
                    .Where(x => x.ShortDesc != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }

            return PartialView("_SidebarPartial", model);
        }
    }
}