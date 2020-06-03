using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuizManager.DBModels;
using QuizManager.Logic;
using QuizManager.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    [Authorize]
    public class GroupController : AbstractController
    {
        public GroupController() : base()
        {
            cx = new QuizContext();

            helper = new ControllerHelper(cx);

            ViewBag.helper = helper;
        }

        /// Navigation Pages
        [Authorize(Roles = "Admin")]
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
                    Name = "View attempts",
                    Link = Url.Action("GroupAttempts", "Group")
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

            var result = new List<Group>();

            foreach(var group in cx.Groups.ToList())
            {
                if(group.ApplicationUsers.Any(x=>x.Id == CurrentUser.Id))
                {
                    result.Add(group);
                }
            }

            return View(result);
        }



        /// <summary>
        /// Redirect to main attempts with parameter
        /// Before should make attempt saving reference or group it was made with
        /// </summary>
        public ActionResult GroupAttempts(int? id)
        {
            var group = cx.Groups.Find(id);

            Session["filters"] = new AttemptFilters()
            {
                Page = 1,
                Group = group,
            };

            return RedirectToAction("Attempts", "Cabinet");
        }


        /// <summary>
        /// Enter group as administrator
        /// </summary>
        public ActionResult OpenGroup(int? id)
        {
            var group = cx.Groups.Find(id);

            var creator = group.Creator;

            var currentUser = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            if(creator.Id != currentUser.Id)
            {
                return HttpNotFound();
            }

            var model = new GroupView()
            {
                Group = group,
                Allowances = cx.GroupAllowances.Where(x=>x.Group.Id == group.Id).ToList()
            };

            return View(model);
        }

        /// <summary>
        /// Enter group as member
        /// </summary>
        public ActionResult ViewGroup(int? id)
        {
            var group = cx.Groups.Find(id);

            var currentUser = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            if (!group.ApplicationUsers.Contains(currentUser))
            {
                return HttpNotFound();
            }

            var view = new MemberGroupView()
            {
                Group = group,
                Allowances = cx.GroupAllowances.Where(x => x.Group.Id == group.Id).ToList()
                    .Select(y=>new MemberGroupAllowanceView()
                    {
                        Allowance = y,
                        Attempts = cx.QuizAttempts.
                            Count(z=>z.Group != null 
                                && z.Group.Id == group.Id 
                                && z.Quiz.Id == y.Quiz.Id)
                    }).ToList()
            };

            if(Session["quizAllowance"] != null)
            {
                ViewBag.Alert = (string)Session["quizAllowance"];
                
                Session["quizAllowance"] = null;
            }

            return View(view);
        }

        /// <summary>
        /// Page for creating group link
        /// </summary>
        public ActionResult GroupLink(int? id)
        {
            var group = cx.Groups.Find(id);

            var references = cx.GroupReferences.Where(x => x.Group.Id == group.Id).ToList();

            var model = new GroupLinkView()
            {
                Group = group,
            };

            if(references.Count() != 0)
            {
                model.IsRefExist = true;

                model.Reference = references.Single();

                model.Link = GenerateLink(references.Single());
            }

            return View(model);
        }

        /// <summary>
        /// Actualy creating group link
        /// </summary>
        [HttpPost]
        public ActionResult GetLink(GroupLinkView view)
        {
            var group = cx.Groups.Find(view.Group.Id);

            var references = cx.GroupReferences.Where(x => x.Group.Id == group.Id).ToList();

            if(references.Count() != 0)
            {
                cx.GroupReferences.RemoveRange(references);
            }

            var model = new GroupReference()
            {
                Id = helper.LinkGenerator(),
                Group = group,
                Deadline = view.Reference.Deadline
            };

            cx.GroupReferences.Add(model);

            cx.SaveChanges();

            return RedirectToAction("GroupLink", new { id = group.Id });
        }

        /// <summary>
        /// Actualy deleting group link
        /// </summary>
        public ActionResult DeleteLink(string id)
        {
            var link = cx.GroupReferences.Find(id);

            var group = cx.Groups.Find(link.Group.Id);

            cx.GroupReferences.Remove(link);

            cx.SaveChanges();

            return RedirectToActionPermanent("GroupLink", new { id=group.Id });
        }

        //Action for joinig group
        public ActionResult JoinGroup(string link)
        {
            var reference = cx.GroupReferences.Find(link);

            if(reference == null)
            {
                return HttpNotFound();
            }

            var group = reference.Group;

            var currentUser = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            if (group.ApplicationUsers.Contains(currentUser))
            {
                return RedirectToAction("ViewGroup", group.Id);
            }
            if(group.Creator.Id == currentUser.Id)
            {
                return HttpNotFound();
            }

            group.ApplicationUsers.Add(currentUser);

            cx.SaveChanges();

            return RedirectToAction("ViewGroup", new { id= group.Id });
        }

        private string GenerateLink(GroupReference reference)
        {
            var baseurl = Request.Url.GetLeftPart(UriPartial.Authority);

            return baseurl + "/Group/JoinGroup?link=" + reference.Id;

            //return "https://localhost:44339/Group/JoinGroup?link=" + reference.Id;
        }

        /// <summary>
        /// User leaves group
        /// </summary>
        public ActionResult LeaveGroup(int? id)
        {
            var group = cx.Groups.Find(id);

            return View(group);
        }
        [HttpPost, ActionName("LeaveGroup")]
        public ActionResult LeaveGroupPost(Group _group)
        {
            var group = cx.Groups.Find(_group.Id);

            var currentUser = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            currentUser.Groups.Remove(group);

            cx.SaveChanges();

            return RedirectToAction("OpenedGroups");
        }

        /// <summary>
        /// Add new group
        /// </summary>
        [HttpGet]
        public ActionResult AddGroup()
        {
            return View();
        }
        [HttpPost, ActionName("AddGroup")]
        public ActionResult AddGroupPost(Group group)
        {
            var creator = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            group.Creator = creator;

            cx.Groups.Add(group);

            cx.SaveChanges();

            return RedirectToAction("MyGroups");
        }

        /// <summary>
        /// Edit group
        /// </summary>
        [HttpGet]
        public ActionResult EditGroup(int? id)
        {
            var group = cx.Groups.Find(id);

            return View(group);
        }
        [HttpPost, ActionName("EditGroup")]
        public ActionResult EditGroupPost(Group group)
        {
            var realGroup = cx.Groups.Find(group.Id);

            realGroup.Name = group.Name;

            cx.SaveChanges();

            return RedirectToAction("OpenGroup", new { id = group.Id });
        }

        /// <summary>
        /// Deleting group
        /// </summary>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            return View(cx.Groups.Find(id));
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(int? id)
        {
            var group = cx.Groups.Find(id);

            var allowances = cx.GroupAllowances.
                Where(x => x.Group.Id == group.Id).ToList();

            cx.GroupAllowances.RemoveRange(allowances);

            var attempts = cx.QuizAttempts.
                Where(x => x.Group != null && x.Group.Id == group.Id).ToList();

            cx.QuizAttempts.RemoveRange(attempts);

            foreach (var user in group.ApplicationUsers)
            {
                group.ApplicationUsers.Remove(user);
            }

            cx.Groups.Remove(group);

            cx.SaveChanges();

            return RedirectToAction("MyGroups");
        }

        /// <summary>
        /// Remove member from group
        /// id is (groupId + "_" + userId)
        /// </summary>
        [HttpGet]
        public ActionResult RemoveMember(string id)
        {
            var ids = id.Split(new char[] { '_' });
            var groupId = Int32.Parse(ids[0]);
            var userId = ids[1];

            var group = cx.Groups.Find(groupId);
            var user = cx.Users.Find(userId);
            
            group.ApplicationUsers.Remove(user);

            cx.SaveChanges();

            return RedirectToAction("OpenGroup", new { id = groupId });
        }

        /// <summary>
        /// id = groupId
        /// For adding allowances
        /// </summary>
        [HttpGet]
        public ActionResult AddAllowance(int? id)
        {
            var group = cx.Groups.Find(id);

            var allowances = cx.GroupAllowances.
                    Where(x => x.Group.Id == group.Id).ToList();

            var allowedQuizs = allowances.Select(x => x.Quiz).ToList();

            var restQuizzes = cx.Quizzes.
                    Where(x => x.User.Id == group.Creator.Id).ToList().
                    Where(y => !allowedQuizs.Contains(y)).ToList();

            var model = new GroupAllowanceView()
            {
                Group = group,
                Allowances = allowances,
                AllowedQuizzes = allowedQuizs,
                RestQuizzes = restQuizzes,
                Allowance = new GroupAllowance()
                {
                    Group = new Group()
                    {
                        Id = group.Id
                    }
                }
            };

            return View(model);
        }
        [HttpPost, ActionName("AddAllowance")]
        public ActionResult AddAllowance(GroupAllowanceView view)
        {
            if (view.Allowance == null)
            {
                return HttpNotFound();
            }
            
            view.Allowance.Group = cx.Groups.Find(view.Allowance.Group.Id);
            view.Allowance.Quiz = cx.Quizzes.Find(view.Allowance.Quiz.Id);

            cx.GroupAllowances.Add(view.Allowance);

            cx.SaveChanges();

            return RedirectToAction("OpenGroup", new { id = view.Allowance.Group.Id });
        }

        [HttpGet]
        public ActionResult EditAllowance(int? id)
        {
            var model = cx.GroupAllowances.Find(id);

            var group = model.Group;

            var allowances = cx.GroupAllowances.
                    Where(x => x.Group.Id == model.Group.Id).ToList();

            var allowedQuizs = allowances.Select(x => x.Quiz).ToList();

            allowedQuizs.Remove(model.Quiz);

            var restQuizzes = cx.Quizzes.
                    Where(x => x.User.Id == group.Creator.Id).ToList().
                    Where(y => !allowedQuizs.Contains(y)).ToList();

            var view = new GroupAllowanceView()
            {
                Allowance = model,
                Group = model.Group,
                Allowances = allowances,
                AllowedQuizzes = allowedQuizs,
                RestQuizzes = restQuizzes
            };

            return View(view);
        }
        [HttpPost]
        public ActionResult EditAllowance(GroupAllowanceView view)
        {
            if (view.Allowance == null)
            {
                return HttpNotFound();
            }

            var allowance = cx.GroupAllowances.Find(view.Allowance.Id);

            allowance.Group = cx.Groups.Find(view.Allowance.Group.Id);
            allowance.Quiz = cx.Quizzes.Find(view.Allowance.Quiz.Id);
            allowance.AttemptCount = view.Allowance.AttemptCount;
            allowance.Type = view.Allowance.Type;
            allowance.Deadline = view.Allowance.Deadline;

            cx.SaveChanges();

            return RedirectToAction("OpenGroup", new { id = view.Allowance.Group.Id });
        }

        [HttpGet]
        public ActionResult RemoveAllowance(int? id)
        {
            var model = cx.GroupAllowances.Find(id);

            var groupId = model.Group.Id;

            cx.GroupAllowances.Remove(model);

            cx.SaveChanges();

            return RedirectToAction("OpenGroup", new { id = groupId });
        }
    }
}