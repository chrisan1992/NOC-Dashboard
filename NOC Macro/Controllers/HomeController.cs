using NOC_Macro.com.hpe.saas.essentials;
using NOC_Macro.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NOC_Macro.Controllers
{
    public class HomeController : Controller
    {
        private NOCEntities db = new NOCEntities();

        public ActionResult Index()
        {
            if(Utilities.IsUserLogged())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserName, string Password)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int result = (int)(db.AutenticateUser(UserName, Utilities.Encrypt(Password)).First());
                    Session["user_logged"] = true;
                    Session["user_id"] = result;
                    Users us = db.Users.Find(result);
                    Session["username"] = us.UserName;
                    Session["user_email"] = us.Email;
                    Session["user_pass"] = Password;
                }
                catch (Exception ex)
                {
                    //if no succedded, to the login page
                    Session["message"] = "Please check your credentials and try again";
                    return View("Login");

                }
            }
            //if succedded, to the homepage
            return RedirectToAction("Index");
        }

        public ActionResult LogOut()
        {
            Session["user_logged"] = false;
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Generic method for guest users
        /// redirects to a marjoincident dashboard with a guest user
        /// </summary>
        /// <returns></returns>
        public ActionResult Dashboard(int? id)
        {
            //put code here
            return View();
        }
    }
}