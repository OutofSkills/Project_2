using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using RentC.UI.Models;

namespace RentC.UI.Controllers
{
    public class ReservationsController : Controller
    {
        private RentC_Entities db = new RentC_Entities();

        // GET: Reservations
        public ActionResult ListRents(string sortOrder)
        {
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "" : "";
            ViewBag.rNameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.PlateSortParm = sortOrder == "Plate" ? "plate_desc" : "Plate";
            ViewBag.sDateSortParm = sortOrder == "sDate" ? "sdate_desc" : "sDate";
            ViewBag.eDateSortParm = sortOrder == "eDate" ? "edate_desc" : "eDate";
            ViewBag.rLocationSortParm = sortOrder == "Location" ? "location_desc" : "Location";

            var rents = from r in db.Reservations
                        select r;

            switch(sortOrder)
            {
                case "name_desc":
                    rents = rents.OrderByDescending(r => r.Customers.Name);
                    break;
                case "Name":
                    rents = rents.OrderBy(r => r.Customers.Name);
                    break;
                case "Plate":
                    rents = rents.OrderBy(r => r.Cars.Plate);
                    break;
                case "plate_desc":
                    rents = rents.OrderByDescending(r => r.Cars.Plate);
                    break;
                case "sDate":
                    rents = rents.OrderBy(r => r.StartDate);
                    break;
                case "sdate_desc":
                    rents = rents.OrderByDescending(r => r.StartDate);
                    break;
                case "eDate":
                    rents = rents.OrderBy(r => r.EndDate);
                    break;
                case "edate_desc":
                    rents = rents.OrderByDescending(r => r.EndDate);
                    break;
                case "Location":
                    rents = rents.OrderBy(r => r.Location);
                    break;
                case "location_desc":
                    rents = rents.OrderByDescending(r => r.Location);
                    break;
                default:
                    rents = rents.OrderBy(r => r.ReservationID);
                    break;
            }


            UpdateRentStatus();
            return View(rents.ToList());
        }

        // GET: Reservations/Details
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CarID,CustomerID,StartDate,EndDate,Location,CouponCode")] Reservations reservation)
        {
            if (ModelState.IsValid)
            {
                if (ValidateCar(reservation))
                {
                    if(ValidateCustomer(reservation.CustomerID))
                    {
                        if (ValidateLocation(reservation.Location, reservation.CarID))
                        {
                            if (reservation.EndDate > reservation.StartDate && reservation.EndDate > DateTime.Today)
                            {
                                db.Reservations.Add(new Reservations
                                {
                                    CarID = reservation.CarID,
                                    CustomerID = reservation.CustomerID,
                                    StartDate = reservation.StartDate,
                                    EndDate = reservation.EndDate,
                                    Location = reservation.Location,
                                    CouponCode = reservation.CouponCode,
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

            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservation.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservation.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservation.CustomerID);

            return View();
        }

        // GET: Reservations/Edit
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
        public ActionResult Edit([Bind(Include = "ReservationID, CarID,CustomerID,StartDate,EndDate,Location,CouponCode")] Reservations reservation)
        {
           
            if (ModelState.IsValid)
            {
                if (db.Reservations.Find(reservation.ReservationID) != null)
                {
                    if (ValidateCar(reservation))
                    {
                        if (ValidateCustomer(reservation.CustomerID) && ValidateChangeAttempt(reservation.ReservationID, reservation.CustomerID)) 
                        {
                            if (ValidateLocation(reservation.Location, reservation.CarID))
                            {
                                if (reservation.EndDate > reservation.StartDate && reservation.EndDate > DateTime.Today)
                                {
                                    var details = db.Reservations.Where(d => d.ReservationID == reservation.ReservationID).First();

                                    details.CarID = reservation.CarID;
                                    details.CustomerID = reservation.CustomerID;
                                    details.StartDate = reservation.StartDate;
                                    details.EndDate = reservation.EndDate;
                                    details.Location = reservation.Location;
                                    details.CouponCode = reservation.CouponCode;
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
                            ModelState.AddModelError("", "Your ID is not valid or there's no existing reservation with this ID");
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
            ViewBag.CarID = new SelectList(db.Cars, "CarID", "Plate", reservation.CarID);
            ViewBag.CouponCode = new SelectList(db.Coupons, "CouponCode", "Description", reservation.CouponCode);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", reservation.CustomerID);

            return View();
        }

        // GET: Reservations/Cancel
        public ActionResult CancelReservation()
        {
            return View();
        }

        // POST: Reservations/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReservation([Bind(Include = "ReservationID, CustomerID")] Reservations reservation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (reservation.ReservationID != default(int) && reservation.CustomerID != default(int))
                {
                    if (ValidateChangeAttempt(reservation.ReservationID, reservation.CustomerID))
                    {
                        var rent = db.Reservations.Where(r => r.ReservationID == reservation.ReservationID).ToList();
                        rent.ForEach(r => r.ReservStatsID = 3);
                        db.SaveChanges();

                        return RedirectToAction("", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your ID is not valid or there's no existing reservation with this ID");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid inserted data");
                }
            }

            return View();
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

        //Validation for the data input
        private bool ValidateCar(Reservations reservation)
        {
            string sDate = reservation.StartDate.ToString("yyyy-MM-dd");
            string eDate = reservation.EndDate.ToString("yyyy-MM-dd");

            var carList = db.Database.SqlQuery<int>("SELECT DISTINCT ReservationID FROM Reservations WHERE CarID = @carID" +
                " and ReservationID IN (SELECT ReservationID FROM Reservations WHERE NOT ((StartDate > @endDate) OR (EndDate < @startDate)))"
                , new SqlParameter("carID", reservation.CarID), new SqlParameter("endDate", eDate), new SqlParameter("startDate", sDate)).ToList();
         
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
        private bool ValidateChangeAttempt(int rentID, int customerID)  
        {
            if (db.Reservations.Find(rentID) != null && db.Customers.Find(customerID) != null)
            {
                var validateCustomerID = db.Reservations.Where(id => id.ReservationID == rentID && id.CustomerID == customerID);

                if (validateCustomerID.FirstOrDefault() != null)
                    return true;
            }
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
