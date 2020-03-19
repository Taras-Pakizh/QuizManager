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
using System.Text.Json;
using QuizManager.XmlModels;
using QuizManager.XmlModels.Answers;


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

        [HttpPost, ActionName("Question")]
        public ActionResult QRedactor_Ajax(string value, int? id)
        {
            if(id == null)
            {
                return HttpNotFound();
            }

            //---------finding question by order number-----

            var questionNumber = Int32.Parse(value);

            var question = cx.Questions.
                    Where(x => x.Quiz.Id == id).
                    Single(y => y.OrderNumber == questionNumber);

            //------

            var model = new RedactorContainerView()
            {
                Question = question
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
        /// Edit quiz info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            if (quiz == null)
            {
                return HttpNotFound();
            }

            var model = new QuizView()
            {
                Id = quiz.Id,
                Name = quiz.Name,
                QuestionCount = quiz.QuestionCount,
                TimeLimit = quiz.TimeLimit,
                Type = quiz.Type,
                UserData = quiz.UserData
            };

            return View(model);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(QuizView view)
        {
            var quiz = cx.Quizzes.Find(view.Id);

            quiz.Name = view.Name;
            quiz.QuestionCount = view.QuestionCount;
            quiz.TimeLimit = view.TimeLimit;
            quiz.Type = view.Type;
            quiz.UserData = view.UserData;

            cx.SaveChanges();

            return RedirectToAction("Open", new { id = view.Id });
        }


        /// <summary>
        /// Shows only COMBOBOX to choose type (which is create inner redactor)
        /// </summary>
        [HttpPost]
        public ActionResult AddQuestion(int? id)
        {
            int maxOrder = 0;

            if(cx.Questions.Where(x => x.Quiz.Id == id).Count() != 0)
            {
                maxOrder = cx.Questions.
                    Where(x => x.Quiz.Id == id).
                    Max(x => x.OrderNumber);
            }

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
        public ActionResult ChangeQuestion(RedactorContainerView view)
        {
            var question = cx.Questions.Find(view.Question.Id);

            question.Obligation = view.Question.Obligation;

            if(view.Question.OrderNumber != question.OrderNumber)
            {
                var questions = cx.Questions.
                    Where(x => x.Quiz.Id == view.Quiz.Id).
                    OrderBy(y=>y.OrderNumber).ToList();

                if (view.Question.OrderNumber > questions.Count)
                {
                    view.Question.OrderNumber = questions.Count;
                }

                questions.RemoveAt(question.OrderNumber - 1);

                questions.Insert(view.Question.OrderNumber - 1, question);

                int index = 1;

                foreach(var item in questions)
                {
                    item.OrderNumber = index;

                    ++index;
                }
            }

            question.Type = view.Question.Type;

            cx.SaveChanges();

            RedactorView model = null;

            if(view.Question.Type != null)
            {
                model = RedactorView.GetView(
                    (QuestionType)view.Question.Type, 
                    cx.Quizzes.Find(view.Quiz.Id).Type
                );

                model.Question = question;

                model.Quiz = cx.Quizzes.Find(view.Quiz.Id);

                if(question.XmlValue != null)
                {
                    model.Model = XmlBase.Deserialize(question.XmlObject, question.TypeName);
                }
            }

            return PartialView("Redactor", model);
        }



        /// <summary>
        /// A new action for every quiz type
        /// </summary>
        [HttpPost]
        public ActionResult SaveXmlListPoll(PollListRedactorView view)
        {
            var question = cx.Questions.Find(view.Question.Id);

            if(question == null)
            {
                return HttpNotFound();
            }

            question.Text = view.Question.Text;

            question.Value = view.Question.Value;

            question.TypeName = view._XmlModel.GetType().Name;

            if (!XmlValidator.Validate(view._XmlModel))
            {
                ViewBag.Errors = XmlValidator.ErrorList;

                return PartialView("Redactor", view);
            }

            question.XmlObject = XmlBase.Serialize<XmlPollList>(view._XmlModel);

            cx.SaveChanges();

            ViewBag.IsSaved = true;

            view.Question = question;

            return PartialView("Redactor", view);
        }

        [HttpPost]
        public ActionResult SaveXmlListTest(TestListRedactorView view)
        {
            if (view.Ids != null)
            {
                foreach (var id in view.Ids)
                {
                    view._XmlModel.Options.Single(x => x.Id == id).IsTrue = true;
                }
            }
            
            var question = cx.Questions.Find(view.Question.Id);

            if (question == null)
            {
                return HttpNotFound();
            }

            question.Text = view.Question.Text;

            question.Value = view.Question.Value;

            question.TypeName = view._XmlModel.GetType().Name;

            if (!XmlValidator.Validate(view._XmlModel))
            {
                ViewBag.Errors = XmlValidator.ErrorList;

                view.Question = cx.Questions.Find(view.Question.Id);

                return PartialView("Redactor", view);
            }

            question.XmlObject = XmlBase.Serialize(view._XmlModel);

            cx.SaveChanges();

            ViewBag.IsSaved = true;

            view.Question = question;
            view.Quiz = question.Quiz;

            return PartialView("Redactor", view);
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteQuestion(int? id)
        {
            var model = cx.Questions.Find(id);

            var quizId = model.Quiz.Id;

            if(model == null)
            {
                return HttpNotFound();
            }

            var questions = cx.Questions.
                Where(x => x.Quiz.Id == model.Quiz.Id).
                OrderBy(y => y.OrderNumber).ToList();

            for(int i = model.OrderNumber; i < questions.Count; ++i)
            {
                questions[i].OrderNumber--;
            }

            cx.Questions.Remove(model);

            cx.SaveChanges();

            return RedirectToAction("Open", new { id = quizId });
        }
    }
}