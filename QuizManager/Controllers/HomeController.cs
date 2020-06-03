using QuizManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(Role.Admin.ToString()))
            {
                return RedirectToAction("Index", "Cabinet");
            }
            else if (User.IsInRole(Role.Student.ToString()))
            {
                return RedirectToAction("OpenedGroups", "Group");
            }
            return RedirectToAction("Register", "Account");
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
    }
}