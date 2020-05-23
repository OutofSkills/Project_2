using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RentC.UI.Models;

namespace RentC.UI.Controllers
{
    public class ReservationsController : Controller
    {
        private DatabaseEntity db = new DatabaseEntity();

        // GET: Reservations
        public ActionResult Index()
        {
            var reservations = db.Reservations.Include(r => r.Cars).Include(r => r.Customers).Include(r => r.ReservationStatuses.Name);
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
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationID,CarID,CustomerID,StartDate,EndDate,Location,CouponCode")] Reservations reservations)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservations.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservations.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservations.CustomerID);
            return View(reservations);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservations.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservations.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservations.CustomerID);
            ViewBag.ReservStatsID = new SelectList(db.ReservationStatuses, "ReservStatsID", "Name", reservations.ReservStatsID);
            return View(reservations);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,CarID,CustomerID,ReservStatsID,StartDate,EndDate,Location,CouponCode,CartPlate")] Reservations reservations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservations.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservations.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservations.CustomerID);
            ViewBag.ReservStatsID = new SelectList(db.ReservationStatuses, "ReservStatsID", "Name", reservations.ReservStatsID);
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
