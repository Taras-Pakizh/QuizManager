using QuizManager.DBModels;
using QuizManager.Helpers;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    public class TestController : AbstractController
    {
        public TestController() : base()
        {
            cx = new QuizContext();
        }

        /// <summary>
        /// Link to quiz => redirect to quiz
        /// </summary>
        [HttpGet]
        public ActionResult GetTest(string link)
        {
            var result = cx.QuizReferences.Find(link);

            if(result == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("TestStart", new { id = result.Quiz.Id });
        }

        /// <summary>
        /// Asks 'Are you ready'
        /// </summary>
        [HttpGet]
        public ActionResult TestStart(int? id)
        {
            return View(cx.Quizzes.Find(id));
        }

        /// <summary>
        /// Main Action
        /// Take quiz id => pass quiz
        /// Timer here
        /// </summary>
        [HttpPost]
        public ActionResult Test(Quiz quiz)
        {
            quiz = cx.Quizzes.Find(quiz.Id);

            if(Session["page"] == null)
            {
                Session["page"] = 0;
            }

            var questions = cx.Questions.Where(x => x.Quiz.Id == quiz.Id).ToList();

            var Tests = new List<TestView>();

            foreach (var question in questions)
            {
                var testView = new TestView()
                {
                    Quiz = quiz,
                    Question = question,
                    Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
                };

                Tests.Add(testView);

                break;
            }

            ViewBag.Tests = Tests;

            return View(quiz);
        }

        //------------------доробити (hidden default val) -----

        [HttpPost]
        /// <summary>
        /// Pagination - future headers of quiz
        /// Partial
        /// Saving section which is scrolling
        /// </summary>
        public ActionResult TestSection(SectionView view)
        {
            if (Session["save"] == null)
            {
                Session["save"] = new TestSave(cx);
            }

            var testSave = (TestSave)Session["save"];

            testSave.Update(view);

            //-----------------Change to Section----------------------
            //-----------------Now just only try of first page--------

            var quiz = cx.Quizzes.Find(view.QuizId);

            var model = new SectionView()
            {
                QuizId = view.QuizId,
                SectionId = 1,
                Tests = new List<TestView>()
            };

            var questions = cx.Questions.Where(x => x.Quiz.Id == quiz.Id).ToList();

            foreach(var question in questions)
            {
                var testView = new TestView()
                {
                    Quiz = quiz,
                    Question = question,
                    Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
                };

                model.Tests.Add(testView);
            }

            //---------------------------------------

            Session["page"] = model.SectionId;

            return PartialView("TestSection", model);
        }
    }
}