using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    public class TestController : Controller
    {
        public QuizContext cx;

        public TestController() : base()
        {
            cx = new QuizContext();
        }

        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetTest(string link)
        {
            var result = cx.QuizReferences.Find(link);

            if(result == null)
            {
                throw new NotImplementedException();
                //Redirect
            }

            return View(result.Quiz);
        }
    }
}