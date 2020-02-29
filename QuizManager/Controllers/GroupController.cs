using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuizManager.DBModels;
using QuizManager.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    public class GroupController : Controller
    {
        public QuizContext cx;

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public GroupController() : base()
        {
            cx = new QuizContext();
        }

        // GET: Group
        public ActionResult MyGroups()
        {
            ViewBag.QuizData = new List<SquereData>()
            {
                new SquereData()
                {
                    Name = "Open",
                    Link = Url.Action("OpenGroup", "Group")
                },
                new SquereData()
                {
                    Name = "Delete",
                    Link = Url.Action("Delete", "Group")
                },
                new SquereData()
                {
                    Name = "Create link",
                    Link = Url.Action("GroupLink", "Group")
                },
                new SquereData()
                {
                    Name = "Add quiz",
                    Link = Url.Action("AddQuiz", "Group")
                }
            };

            var CurrentUser = UserManager.FindByName(User.Identity.Name);

            return View(cx.Groups.Where(x => x.Creator.Id == CurrentUser.Id).ToList());
        }

        public ActionResult OpenedGroups()
        {
            ViewBag.QuizData = new List<SquereData>()
            {
                new SquereData()
                {
                    Name = "Open",
                    Link = Url.Action("ViewGroup", "Group")
                },
                new SquereData()
                {
                    Name = "Leave",
                    Link = Url.Action("LeaveGroup", "Group")
                },
            };

            var CurrentUser = UserManager.FindByName(User.Identity.Name);

            return View(cx.Groups.Where(x=>x.ApplicationUsers.Contains(CurrentUser)).ToList());
        }

        public ActionResult Delete(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeletePost(int? id)
        {
            return RedirectToAction("Index");
        }

        public ActionResult OpenGroup(int? id)
        {
            return View();
        }

        public ActionResult ViewGroup(int? id)
        {
            return View();
        }

        public ActionResult GroupLink(int? id)
        {
            return View();
        }

        public ActionResult GetLink(int? id)
        {
            return View();
        }

        public ActionResult DeleteLink(int? id)
        {
            return View();
        }

        public ActionResult AddQuiz(int? id)
        {
            return View();
        }

        public ActionResult LeaveGroup(int? id)
        {
            return View();
        }

        public ActionResult CreateGroup()
        {
            return View();
        }
    }
}