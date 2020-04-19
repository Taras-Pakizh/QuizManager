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
    public abstract class AbstractController:Controller
    {
        public QuizContext cx;

        public ControllerHelper helper;

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

        public AbstractController() : base()
        {

        }
    }
}