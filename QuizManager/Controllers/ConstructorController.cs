using Microsoft.AspNet.Identity;
using QuizManager.DBModels;
using QuizManager.Helpers;
using QuizManager.Helpers.Models;
using QuizManager.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Controllers
{
    [Authorize]
    public class ConstructorController : AbstractController
    {
        //view = Name
        //value = Type Name
        
        public ConstructorController() : base()
        {
            cx = new QuizContext();
        }

        

        /// <summary>
        /// Holy shit
        /// </summary>
        [HttpGet]
        public ActionResult Open(int? id)
        {
            var model = cx.Quizzes.Find(id);

            if(model == null)
            {
                return HttpNotFound();
            }

            ViewBag.QuestionOrders = cx.Questions.
                        Where(x => x.Quiz.Id == model.Id).
                        Select(y => y.OrderNumber).
                        OrderBy(z => z).
                        ToList();

            return View(model);
        }

        [HttpGet, ActionName("Question")]
        public ActionResult QRedactor_Ajax(string value, int? id)
        {
            if(id == null)
            {
                return HttpNotFound();
            }

            //---------finding question id by order number-----

            var questionNumber = Int32.Parse(value);

            var questionId = cx.Questions.
                    Where(x => x.Quiz.Id == id).
                    Single(y => y.OrderNumber == questionNumber);

            //------

            var model = new RedactorContainerView()
            {
                Question = cx.Questions.Find(questionId),
            };

            model.Quiz = model.Question.Quiz;

            return PartialView("RedactorContainer", model);
        }

        /// <summary>
        /// Panel for creating new Quiz
        /// After naming and adding new quiz
        /// Redirect to Open
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost, ActionName("Add")]
        public ActionResult AddPost(QuizView view)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }

            var CurrentUser = UserManager.FindByName(User.Identity.Name);

            var model = new Quiz()
            {
               Name = view.Name,
               QuestionCount = view.QuestionCount,
               TimeLimit = view.TimeLimit,
               Type = view.Type,
               UserData = view.UserData,
               User = CurrentUser,
            };

            cx.Quizzes.Add(model);

            cx.SaveChanges();

            return RedirectToAction("Open", new { id = model.Id });
        }



        /// <summary>
        /// NotImlemented = create new view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            return View();
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(QuizView view)
        {
            return View();
            //return RedirectToAction("Open", new { id =  });
        }


        /// <summary>
        /// Shows only COMBOBOX to choose type (which is create inner redactor)
        /// </summary>
        [HttpGet]
        public ActionResult AddQuestion(int? id)
        {
            var maxOrder = cx.Questions.
                Where(x => x.Quiz.Id == id).
                Max(x => x.OrderNumber);

            var newQuestion = new Question()
            {
                OrderNumber = maxOrder + 1,

                Quiz = cx.Quizzes.Find(id)
            };

            cx.Questions.Add(newQuestion);

            cx.SaveChanges();

            var model = new RedactorContainerView()
            {
                Quiz = cx.Quizzes.Find(id),

                Question = newQuestion
            };

            return PartialView("RedactorContainer", model);
        }

        /// <summary>
        /// Create real redactor (using RedactorHelper)
        /// </summary>
        [HttpPost, ActionName("Change")]
        public ActionResult ChangeQuestion(QuestionView view)
        {
            var question = cx.Questions.Find(view.QuestionId);

            question.Obligation = view.Obligation;

            if(view.Order != question.OrderNumber)
            {
                var questions = cx.Questions.Where(x => x.Quiz.Id == view.QuizId).OrderBy(y=>y.OrderNumber).ToList();

                questions.RemoveAt(question.OrderNumber - 1);

                questions.Insert(view.Order - 1, question);

                int index = 1;

                foreach(var item in questions)
                {
                    item.OrderNumber = index;

                    ++index;
                }
            }

            question.Type = view.Type;

            cx.SaveChanges();

            RedactorView model = null;

            if(view.Type != null)
            {
                model = RedactorView.GetView(
                    (QuestionType)view.Type, 
                    cx.Quizzes.Find(view.QuizId).Type
                );

                model.Question = question;

                model.Quiz = cx.Quizzes.Find(view.QuizId);
            }

            return PartialView("Redactor", model);
        }





        [HttpPost, ActionName("Save")]
        public ActionResult SaveQuestion(RedactorView view)
        {
            //var g = new HashSet<int>();

            //var j = g.ElementAt(1);

            //Redirect
            return View();
        }
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteQuestion(int? id)
        {
            return View();
        }
    }
}