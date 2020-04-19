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
using QuizManager.Logic;

namespace QuizManager.Controllers
{
    [Authorize]
    public class ConstructorController : AbstractController
    {
        public ConstructorController() : base()
        {
            cx = new QuizContext();

            helper = new ControllerHelper(cx);
        }

        //--------------------Sections------------------------------------------------------------


        /// <summary>
        /// Shows page for creating and viewing sections
        /// </summary>
        public ActionResult Sections(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            var model = new QuizSectionsView()
            {
                Quiz = quiz,
                Sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList()
            };

            ViewBag.helper = helper;

            return View(model);
        }

        /// <summary>
        /// For Changing order of sections
        /// </summary>
        public ActionResult SaveOrder(QuizSectionsView view)
        {
            if (view.SectionIds == null)
            {
                return HttpNotFound();
            }

            var i = 1;

            foreach (var index in view.SectionIds)
            {
                var section = cx.Sections.Find(index);

                section.Order = i;

                ++i;
            }

            cx.SaveChanges();

            var quiz = cx.Quizzes.Find(view.Quiz.Id);

            var model = new QuizSectionsView()
            {
                Quiz = quiz,
                Sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList()
            };

            return View("Sections", model);
        }

        [HttpGet]
        public ActionResult DeleteSection(int? id)
        {
            var model = cx.Sections.Find(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            var sections = cx.Sections.Where(x => x.Quiz.Id == model.Quiz.Id).ToList();

            for (int i = model.Order; i < sections.Count; ++i)
            {
                sections[i].Order--;
            }

            int quizId = model.Quiz.Id;

            var questions = cx.Questions.Where(x => x.Section.Id == model.Id).ToList();

            cx.Questions.RemoveRange(questions);

            cx.Sections.Remove(model);

            cx.SaveChanges();

            var quiz = cx.Quizzes.Find(quizId);

            var quizModel = new QuizSectionsView()
            {
                Quiz = quiz,
                Sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList()
            };

            return View("Sections", quizModel);
        }

        [HttpPost]
        public ActionResult AddSection(QuizSectionsView view)
        {
            var section = view.NewSection;

            section.Quiz = cx.Quizzes.Find(section.Quiz.Id);

            section.Order = cx.Sections.
                    Where(x => x.Quiz.Id == section.Quiz.Id).
                    Max(y => y.Order) + 1;

            cx.Sections.Add(section);

            cx.SaveChanges();

            var quiz = cx.Quizzes.Find(section.Quiz.Id);

            var quizModel = new QuizSectionsView()
            {
                Quiz = quiz,
                Sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList()
            };

            return View("Sections", quizModel);
        }

        [HttpGet]
        public ActionResult EditSection(int? id)
        {
            var model = cx.Sections.Find(id);

            return View(model);
        }
        [HttpPost, ActionName("EditSection")]
        public ActionResult EditSectionPost(Section view)
        {
            var model = cx.Sections.Find(view.Id);

            model.Name = view.Name;
            model.TimeLimit = view.TimeLimit;
            model.Type = view.Type;
            model.QuestionCount = view.QuestionCount;

            cx.SaveChanges();

            return RedirectToAction("Open", new { id = model.Id });
        }


        //-------------------------------------------------------------------------------------


        /// <summary>
        /// Open page for constructing questions
        /// Change for one section
        /// </summary>
        [HttpGet]
        public ActionResult Open(int? id)
        {
            var model = cx.Sections.Find(id);

            if(model == null)
            {
                return HttpNotFound();
            }

            ViewBag.QuestionOrders = cx.Questions.
                        Where(x => x.Section.Id == model.Id).
                        Select(y => y.OrderNumber).
                        OrderBy(z => z).
                        ToList();

            return View(model);
        }

        //---------------------------------------------------------------------------------------


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
               Value = view.Value,
               TimeLimit = view.TimeLimit,
               Type = view.Type,
               UserData = view.UserData,
               User = CurrentUser,
            };

            cx.Quizzes.Add(model);

            cx.SaveChanges();

            return RedirectToAction("Sections", new { id = model.Id });
        }

        /// <summary>
        /// Edit quiz info
        /// </summary>
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
                Value = quiz.Value,
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
            quiz.Value = view.Value;
            quiz.TimeLimit = view.TimeLimit;
            quiz.Type = view.Type;
            quiz.UserData = view.UserData;

            cx.SaveChanges();

            return RedirectToAction("Sections", new { id = view.Id });
        }

        /// <summary>
        /// Comman for every quiz type
        /// Contains logic for editing order numbers of questions
        /// </summary>
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteQuestion(int? id)
        {
            var model = cx.Questions.Find(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            var sectionId = model.Section.Id;

            var questions = cx.Questions.
                Where(x => x.Section.Id == model.Section.Id).
                OrderBy(y => y.OrderNumber).ToList();

            for (int i = model.OrderNumber; i < questions.Count; ++i)
            {
                questions[i].OrderNumber--;
            }

            var answers = cx.Answers.Where(x => x.Question.Id == model.Id).ToList();

            foreach(var answer in answers)
            {
                answer.Question = null;
            }

            cx.Questions.Remove(model);

            cx.SaveChanges();

            return RedirectToAction("Open", new { id = sectionId });
        }


        //--------------------------------------------------------------------------------------


        /// <summary>
        /// Action for choosing question by order buttons
        /// </summary>
        [HttpPost, ActionName("Question")]
        public ActionResult QRedactor_Ajax(string value, int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            //---------finding question by order number-----

            var questionNumber = Int32.Parse(value);

            var question = cx.Questions.
                    Where(x => x.Section.Id == id).
                    Single(y => y.OrderNumber == questionNumber);

            //------

            var model = new RedactorContainerView()
            {
                Question = question,
                Quiz = question.Quiz,
                Section = question.Section
            };

            return PartialView("RedactorContainer", model);
        }

        /// <summary>
        /// Action for button AddQuestion
        /// Shows only COMBOBOX to choose type (which is create inner redactor)
        /// </summary>
        [HttpPost]
        public ActionResult AddQuestion(int? id)
        {
            int maxOrder = 0;

            if(cx.Questions.Where(x => x.Section.Id == id).Count() != 0)
            {
                maxOrder = cx.Questions.
                    Where(x => x.Section.Id == id).
                    Max(x => x.OrderNumber);
            }

            var newQuestion = new Question()
            {
                OrderNumber = maxOrder + 1,

                Section = cx.Sections.Find(id),

                Quiz = cx.Sections.Find(id).Quiz
            };

            cx.Questions.Add(newQuestion);

            cx.SaveChanges();

            var model = new RedactorContainerView()
            {
                Question = newQuestion,

                Section = newQuestion.Section,

                Quiz = newQuestion.Quiz
            };

            return PartialView("RedactorContainer", model);
        }

        /// <summary>
        /// Create real redactor after selecting type of question
        /// </summary>
        [HttpPost, ActionName("Change")]
        public ActionResult ChangeQuestion(RedactorContainerView view)
        {
            var question = cx.Questions.Find(view.Question.Id);

            question.Obligation = view.Question.Obligation;

            if(view.Question.OrderNumber != question.OrderNumber)
            {
                var questions = cx.Questions.
                    Where(x => x.Section.Id == view.Section.Id).
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

                model.Section = cx.Sections.Find(view.Section.Id);

                if(question.XmlValue != null)
                {
                    model.Model = XmlBase.Deserialize(question.XmlObject, question.TypeName);
                }
            }

            return PartialView("Redactor", model);
        }


        //-----------------------------------------------------------------------------------


        /// <summary>
        /// A new action for saving every quiz type
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
            view.Quiz = question.Quiz;
            view.Section = question.Section;

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
            view.Section = question.Section;

            return PartialView("Redactor", view);
        }

    }
}