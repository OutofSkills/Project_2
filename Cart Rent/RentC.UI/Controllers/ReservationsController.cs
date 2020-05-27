using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using RentC.UI.Models;
using WebGrease.Css.Extensions;

namespace RentC.UI.Controllers
{
    public class ReservationsController : Controller
    {
        private RentC_Entities db = new RentC_Entities();

        // GET: Reservations
        public ActionResult ListRents()
        {
            UpdateRentStatus();
            var reservations = db.Reservations.Include(r => r.Cars).Include(r => r.Customers).Include(r => r.ReservationStatuses);
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservations = db.Reservations.Find(id);
            if (reservations == null)
            {
                return HttpNotFound();
            }
            return View(reservations);
        }

        // GET: Reservations/Create
        public ActionResult Create()
        {
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate");
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "CouponCode");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerID");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CarID,CustomerID,StartDate,EndDate,Location,CouponCode")] Reservations reservations)
        {
            if (ModelState.IsValid)
            {
                if (ValidateCar(reservations))
                {
                    if(ValidateCustomer(reservations.CustomerID))
                    {
                        if (ValidateLocation(reservations.Location, reservations.CarID))
                        {
                            if (reservations.EndDate > reservations.StartDate)
                            {
                                db.Reservations.Add(new Reservations
                                {
                                    CarID = reservations.CarID,
                                    CustomerID = reservations.CustomerID,
                                    StartDate = reservations.StartDate,
                                    EndDate = reservations.EndDate,
                                    Location = reservations.Location,
                                    CouponCode = reservations.CouponCode,
                                    ReservStatsID = 1
                                });

                                db.SaveChanges();
                                Session["ReservationID"] = db.Reservations.Max(d => d.ReservationID);

                                return RedirectToAction("GetRentID", "Reservations");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid time interval");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "This car is not available in your location");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your ID is not valid");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Car is unavailable");
                }
            }

            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservations.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservations.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservations.CustomerID);

            return View(reservations);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit()
        {
            ViewBag.ReservationID = new SelectList(db.Reservations, "ReservationID", "ReservationID");
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate");
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "CouponCode");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerID");

            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID, CarID,CustomerID,StartDate,EndDate,Location,CouponCode")] Reservations reservations)
        {
            if (ModelState.IsValid)
            {
                if (db.Reservations.Find(reservations.ReservationID) != null)
                {
                    if (ValidateCar(reservations))
                    {
                        if (ValidateCustomer(reservations.CustomerID) && ValidateCustomerToUpdate(reservations.ReservationID, reservations.CustomerID)) ////
                        {
                            if (ValidateLocation(reservations.Location, reservations.CarID))
                            {
                                if (reservations.EndDate > reservations.StartDate)
                                {
                                    var details = db.Reservations.Where(d => d.ReservationID == reservations.ReservationID).First();

                                    details.CarID = reservations.CarID;
                                    details.CustomerID = reservations.CustomerID;
                                    details.StartDate = reservations.StartDate;
                                    details.EndDate = reservations.EndDate;
                                    details.Location = reservations.Location;
                                    details.CouponCode = reservations.CouponCode;
                                    details.ReservStatsID = 1;


                                    db.SaveChanges();
                                    return RedirectToAction("", "Home");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid time interval");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "This car is not available in your location");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Your ID is not valid or there's no existing reservation with your ID");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Car is unavailable");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your reservation ID is not valid");
                }
            }

            ViewBag.ReservationID = new SelectList(db.Reservations, "ReservationID", "ReservationID");
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservations.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservations.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservations.CustomerID);

            return View(reservations);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservations = db.Reservations.Find(id);
            if (reservations == null)
            {
                return HttpNotFound();
            }
            return View(reservations);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservations reservations = db.Reservations.Find(id);
            db.Reservations.Remove(reservations);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetRentID()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValidateCar(Reservations reservations)
        {
            string sDate = reservations.StartDate.ToString("yyyy-MM-dd");
            string eDate = reservations.EndDate.ToString("yyyy-MM-dd");

            var carList = db.Database.SqlQuery<int>("SELECT DISTINCT ReservationID FROM Reservations WHERE CarID = @carID" +
                " and ReservationID IN (SELECT ReservationID FROM Reservations WHERE NOT ((StartDate > @endDate) OR (EndDate < @startDate)))"
                , new SqlParameter("carID", reservations.CarID), new SqlParameter("endDate", eDate), new SqlParameter("startDate", eDate)).ToList();
         
            if (carList.FirstOrDefault() > 0)
            {
                return false;
            }
            return true;
        }

        private bool ValidateCustomer(int id)
        {
            if(db.Customers.Find(id) == null)
            {
                return false;
            }

            return true;
        }

        //Used to verify if there's a resservation with the given reservation ID and customer's ID
        //to be sure that the user won't change other reservations by mistake
        private bool ValidateCustomerToUpdate(int rentID, int customerID)  
        {
            var validateCustomerID = db.Reservations.Where(id => id.ReservationID == rentID && id.CustomerID == customerID);
            if (validateCustomerID.FirstOrDefault() != null)
                return true;
            return false;
        }

        private bool ValidateLocation(string location, int carID)
        {
            var carAtLocation = db.Database.SqlQuery<int>("Select CarID FROM Cars WHERE Location = @location", new SqlParameter("location", location)).ToList<int>(); ;
            if(carAtLocation.Contains(carID))
            {
                return true;
            }
                return false;
        }

        private void UpdateRentStatus()
        {
            var rent = db.Reservations.Where(r => r.EndDate < DateTime.Now).ToList(); 
            rent.ForEach(r=>r.ReservStatsID = 2);
            db.SaveChanges();
        }
    }
}
