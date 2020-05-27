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
    public class CustomersController : Controller
    {
        private RentC_Entities db = new RentC_Entities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,Name,BirthDate,Location")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                if (ValidateID(customers.CustomerID))
                {
                    db.Customers.Add(customers);
                    db.SaveChanges();
                    return RedirectToAction("", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This ID already exists");
                }
            }

            return View(customers);
        }

        // GET:
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,Name,BirthDate,Location")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                if (!ValidateID(customers.CustomerID))
                {
                    var details = db.Customers.Where(c => c.CustomerID == customers.CustomerID).ToList();

                    details.ForEach(c =>
                    {
                        c.Name = customers.Name;
                        c.BirthDate = customers.BirthDate;
                        c.Location = customers.Location;
                    });

                    db.SaveChanges();
                    return RedirectToAction("", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This ID doesn't exist");
                }
            }

            return View(customers);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customers customers = db.Customers.Find(id);
            db.Customers.Remove(customers);
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

        private bool ValidateID(int id)
        {
            if(db.Customers.Find(id) == null)
            {
                return true;
            }
            return false;
        }
        
    }
}
