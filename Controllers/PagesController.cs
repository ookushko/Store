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
        public ActionResult Index()
        {
            return View();
        }
    }
}