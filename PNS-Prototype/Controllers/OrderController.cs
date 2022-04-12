using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PNS_Prototype.Models;

namespace PNS_Prototype.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult List()
        {
            using (PNSDbEntities db = new PNSDbEntities())
            {
                return View(db.Orders.ToList());
            }
        }


        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Picking(int id)
        {
            using (PNSDbEntities db = new PNSDbEntities())
            {
                var pick = db.Fulfills.Where(x => x.OrderId == id).OrderBy(o => o.Supplied == o.Order).ToList();
                Session["OrderId"] = id;

                ViewBag.PackingSlipId = Session["OrderId"];

                if(Session["Scanned"] != null)
                {
                    ViewBag.Added = "Item " + Session["Scanned"] + " has been fulfilled!";
                }

                return View(pick);
            }
        }


        public ActionResult Preview()
        {
            using (PNSDbEntities db = new PNSDbEntities())
            {
                var orderIdSession = Convert.ToInt32(Session["OrderId"]);

                var result = db.Fulfills.Where(x => x.OrderId == orderIdSession).ToList();
 

                ViewBag.PackingSlipId = orderIdSession;

                return View(result);
            }
        }



        // For testing purposes, this block of code searches for entered
        // string in the text field and look for matches in the database 
        public ActionResult Scan(string scanned)
        {


            PNSDbEntities db = new PNSDbEntities();

            var orderIdSession = Convert.ToInt32(Session["OrderId"]);
  
            var scan = db.Fulfills.FirstOrDefault(x => x.ProCode == scanned && x.OrderId == orderIdSession);

            var orderid = orderIdSession;

            var url = "Picking/" + orderIdSession;

            ViewBag.Back = "https://localhost:44373/Order/" + url;

            if (scan == null)
            {

                return RedirectToAction("Error");
            }


            scan.Supplied++;
            ViewBag.Added = "Sucessfully scanned the item " + scan.ProCode + "!";


            if (scan.Supplied == scan.Order)
            {
                Session["Scanned"] = scan.ProCode;
                ViewData.Clear();

                db.SaveChanges();
                return RedirectToAction(url);
            }

            else if ((scan.Supplied > scan.Order))
            {
                ViewData.Clear();
                return RedirectToAction("Error");
            }

            else
            {
                db.SaveChanges();

                ModelState.Clear();
            }



            return View(scan);

        }

        // To view the specific row matching the id, then displays a view
        // where you should be able to manually add supply
        // However, for prototype purposes it doesn't make any changes
        // in the database.
        public ActionResult View(string id, Fulfill fulfill)
        {
            try
            {
                var orderIdSession = Convert.ToInt32(Session["OrderId"]);

                PNSDbEntities db = new PNSDbEntities();
                var scan = db.Fulfills.First(x => x.ProCode == id && x.OrderId == orderIdSession);
                //scan.Supplied = fulfill.Supplied;
                //db.SaveChanges();

                return View(scan);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error");
            }

        }

        // Returns a simple view displaying Error and
        // OK button to return to the previous page.
        public ActionResult Error()
        {

            return View();
        }

        //For button Save & Exit
        public ActionResult ExitPicking()
        {
            HttpContext.Session.Clear();
            ViewData.Clear();
            return RedirectToAction("List", "Order");
        }

    }
}