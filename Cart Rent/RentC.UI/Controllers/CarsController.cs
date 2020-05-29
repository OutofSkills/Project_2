using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RentC.UI.Models;

namespace RentC.UI.Controllers
{
    public class CarsController : Controller
    {
        private RentC_Entities db = new RentC_Entities();

        public ActionResult SubmitCondition()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitCondition([Bind(Include = "StartDate,EndDate,Location")] Reservations reservation)
        {
            if (reservation.StartDate <= reservation.EndDate)
            {

                if (reservation.StartDate != default)
                    TempData["StartDate"] = reservation.StartDate;
                else
                    TempData["StartDate"] = null;

                if (reservation.EndDate != default)
                    TempData["EndDate"] = reservation.EndDate;
                else
                    TempData["EndDate"] = null;

                if (reservation.Location != default)
                    TempData["Location"] = reservation.Location;
                else
                    TempData["Location"] = null;
            }

             return Redirect("/Cars/DisplayCars");
                
        }

  
        public ActionResult DisplayCars()
        {
            List<Cars> carList = new List<Cars>();

            var Location = TempData["Location"];
            var StartDate = TempData["StartDate"];
            var EndDate = TempData["EndDate"];
            
            if (ModelState.IsValid)
            {
                if (StartDate != null && EndDate != null && Location != null)
                {
                    carList = db.Database.SqlQuery<Cars>("Select * from Cars WHERE Location = @location AND CarID NOT IN" +
                         "(Select CarID FROM Reservations WHERE NOT (StartDate > @endDate) OR (EndDate < @startDate))",
                         new SqlParameter("location", Location), new SqlParameter("endDate", EndDate), new SqlParameter("startDate", StartDate)).ToList<Cars>();

                    Session["Details"] = "List of Cars in " + Location + " during " + Convert.ToDateTime(StartDate).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(EndDate).ToString("dd/MM/yyyy");
                }
                else if (StartDate == null && EndDate == null && Location != null)
                {
                    carList = db.Database.SqlQuery<Cars>("Select * from Cars WHERE Location = @location",
                         new SqlParameter("location", Location)).ToList<Cars>();

                    Session["Details"] = "List of Cars in " + Location;
                }
                else if (StartDate != null && EndDate != null && Location == null)
                {
                    carList = db.Database.SqlQuery<Cars>("Select * from Cars WHERE CarID NOT IN" +
                        "(Select CarID FROM Reservations WHERE NOT (StartDate > @endDate) OR (EndDate < @startDate))",
                        new SqlParameter("endDate", EndDate), new SqlParameter("startDate", StartDate)).ToList<Cars>();

                    Session["Details"] = "List of available Cars during " + Convert.ToDateTime(StartDate).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(EndDate).ToString("dd/MM/yyyy");
                }
                else
                {
                    carList = db.Database.SqlQuery<Cars>("Select * FROM Cars").ToList<Cars>();

                    Session["Details"] = "No Criteria Cars List";
                }
            }

            if (carList == null)
            {
                ModelState.AddModelError("", "No available cars");
            }
            return View(carList);
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cars cars = db.Cars.Find(id);
            if (cars == null)
            {
                return HttpNotFound();
            }
            return View(cars);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
