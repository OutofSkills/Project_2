﻿using RentC.UI.Models;
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
        private readonly RentC_Entities database = new RentC_Entities();

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
                if (user.Enabled == true)
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
                    else
                    {
                        ModelState.AddModelError("", "Invalid Password or user ID");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Account is disable");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Password or user ID");
            }


            return View(user);
        }
    }
}