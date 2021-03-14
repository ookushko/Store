using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Store_MVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using (Db db = new Db())
            {   
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string shortDesc;

                PagesDTO dto = new PagesDTO();

                dto.Title = model.Title.ToUpper();

                if (string.IsNullOrWhiteSpace(model.ShortDesc))
                {
                    shortDesc = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    shortDesc = model.ShortDesc.Replace(" ", "-").ToLower();
                }

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

                dto.ShortDesc = shortDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "You added a new Page"; // Successful message

            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                model = new PageVM(dto);
            }

            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;

                string shortDesc = null;

                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.ShortDesc))
                {
                    shortDesc = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    shortDesc = model.ShortDesc.Replace(" ", "-").ToLower();
                }

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

                dto.ShortDesc = shortDesc;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the page.";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The pages does  not exist.");
                }

                model = new PageVM(dto);
            }
            return View(model);
        }

        // Record deletion method
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                db.Pages.Remove(dto);

                db.SaveChanges();
            }

            TempData["SM"] = "You have deleted a page.";

            return RedirectToAction("Index");
        }

        // Sorting method
        // GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {

            using (Db db = new Db())
            {
                int count = 0;

                PagesDTO dto;

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
            SidebarVM model;

            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); // Хардкод, ИСПРАВИТЬ!

                model = new SidebarVM(dto);
            }

            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1); // Хардкод, ИСПРАВИТЬ!

                dto.Body = model.Body;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the sidebar.";

            return RedirectToAction("EditSidebar");
        }
    }
}