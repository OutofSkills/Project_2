using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RentC.UI.Models;

namespace RentC.UI.Controllers
{
    public class CustomersController : Controller
    {
        private RentC_Entities db = new RentC_Entities();

        // GET: Customers
        [Authorize(Roles ="Administrator, Manager, Salesperson")]
        public ActionResult CustomerList(string sortOrder)
        {
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.LocationSortParm = sortOrder == "Location" ? "location_desc" : "Location";

            var customers = from s in db.Customers
                               select s;

            switch (sortOrder)
            {
                case "id_desc":
                    customers = customers.OrderByDescending(s=>s.CustomerID);
                    break;
                case "Name":
                    customers = customers.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    customers = customers.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    customers = customers.OrderBy(s => s.BirthDate);
                    break;
                case "date_desc":
                    customers = customers.OrderByDescending(s => s.BirthDate);
                    break;
                case "location_desc":
                    customers = customers.OrderByDescending(s => s.Location);
                    break;
                case "Location":
                    customers = customers.OrderBy(s => s.Location);
                    break;
                default:
                    customers = customers.OrderBy(s => s.CustomerID);
                    break;
            }
            return View(customers.ToList());
        }

        // GET: Customers/Details
        [Authorize(Roles = "Administrator, Manager, Salesperson")]
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
        [Authorize(Roles = "Administrator, Manager, Salesperson")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,Name,BirthDate,Location")] Customers customer)
        {
            if (ModelState.IsValid)
            {
                if (ValidateID(customer.CustomerID))
                {
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This ID already exists");
                }
            }

            return View(customer);
        }

        // GET:
        [Authorize(Roles = "Administrator, Manager, Salesperson")]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,Name,BirthDate,Location")] Customers customer)
        {
            if (ModelState.IsValid)
            {
                if (!ValidateID(customer.CustomerID))
                {
                    var details = db.Customers.Where(c => c.CustomerID == customer.CustomerID).ToList();

                    details.ForEach(c =>
                    {
                        c.Name = customer.Name;
                        c.BirthDate = customer.BirthDate;
                        c.Location = customer.Location;
                    });

                    db.SaveChanges();
                    return RedirectToAction("", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This ID doesn't exist");
                }
            }

            return View(customer);
        }

        // GET: Customers/Delete
        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customers customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("", "Home");
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
