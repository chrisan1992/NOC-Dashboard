using NOC_Macro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
                    return View("Login");

                }
            }
            //if succedded, to the homepage
            return RedirectToAction("Index");
        }
    }
}