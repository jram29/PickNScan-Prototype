using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PNS_Prototype.Controllers
{
    public class HomeController : Controller
    {

        

        public ActionResult Index()
        {
            return View(); // Simple navigation that return using html which
                           // has links to direct user to other controllers.
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}