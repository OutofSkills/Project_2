using RentC.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentC.UI.Controllers
{
    
    public class AccountController : Controller
    {
        // GET: Account
        private readonly DatabaseEntity database = new DatabaseEntity();

        public ActionResult Welcome()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users user)
        {
            if (ModelState.IsValid)
            {
                var details = (from userList in database.Users
                               where userList.UserID == user.UserID && userList.Password == user.Password
                               select new
                               {
                                   userList.Roles.Name,
                                   userList.Roles.Description
                               }).ToList();
                if (details.FirstOrDefault() != null)
                {
                   Session["Name"] = details.FirstOrDefault().Name;
                   Session["Description"] = details.FirstOrDefault().Description;

                    return RedirectToAction("Welcome", "Account");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Credentials");
            }


            return View(user);
        }
    }
}