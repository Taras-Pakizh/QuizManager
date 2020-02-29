using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuizManager.ModelViews;
using QuizManager.XmlModels.Answers;
using QuizManager.XmlModels;

namespace QuizManager.Controllers
{
    [Authorize]
    public class CabinetController : Controller
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



        public CabinetController():base()
        {
            cx = new QuizContext();
        }

        // GET: Cabinet
        public ActionResult Index()
        {
            ViewBag.QuizData = new List<SquereData>()
            {
                new SquereData()
                {
                    Name = "Open",
                    Link = Url.Action("Open", "Constructor")
                },
                new SquereData()
                {
                    Name = "Create link",
                    Link = Url.Action("QuizLink", "Cabinet")
                },
                new SquereData()
                {
                    Name = "Delete",
                    Link = Url.Action("Delete", "Cabinet")
                },
                new SquereData()
                {
                    Name = "Statistic",
                    Link = Url.Action("QuizInfo", "Cabinet")
                }
            };

            var CurrentUser = UserManager.FindByName(User.Identity.Name);

            return View(cx.Quizzes.Where(x => x.User.Id == CurrentUser.Id).ToList());
        }

        /// <summary>
        /// Return page with link to copy 
        /// if link is already exist - delete previous and create new
        /// </summary>
        /// <param name="id">Id of quiz</param>
        public ActionResult QuizLink(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            if (quiz == null)
            {
                return HttpNotFound();
            }

            var reference = cx.QuizReferences.Where(x => x.Quiz.Id == quiz.Id).ToList();

            var model = new QuizLinkView()
            {
                Quiz = quiz
            };

            if(reference.Count() != 0)
            {
                model.Reference = reference.Single();

                model.IsRefExists = true;

                model.Link = GenerateLink(reference.Single());
            }
            
            return View(model);
        }

        private string GenerateLink(QuizReference reference)
        {
            return "https://localhost:44339/Test/GetTest?link=" + reference.Id;
        }

        /// <summary>
        /// Generate new link and redirect to QuizLink
        /// </summary>
        [HttpGet]
        public ActionResult GetLink(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            if(quiz == null)
            {
                return HttpNotFound();
            }

            var reference = cx.QuizReferences.Where(x => x.Quiz.Id == quiz.Id).ToList();

            if(reference.Count != 0)
            {
                cx.QuizReferences.RemoveRange(reference);
            }

            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "A");
            GuidString = GuidString.Replace("+", "B");
            GuidString = GuidString.Replace("/", "C");

            var model = new QuizReference()
            {
                Id = GuidString,
                Quiz = quiz,
                Deadline = DateTime.Now
            };

            cx.QuizReferences.Add(model);

            cx.SaveChanges();

            return RedirectToAction("QuizLink", new { id = quiz.Id });
        }
        
        public ActionResult DeleteLink(string Id)
        {
            var link = cx.QuizReferences.Find(Id);

            if(link == null)
            {
                return HttpNotFound();
            }

            var quizId = link.Quiz.Id;

            cx.QuizReferences.Remove(link);

            cx.SaveChanges();

            return RedirectToAction("QuizLink", new { id = quizId });
        }


        [HttpGet]
        public ActionResult Delete(int? id)
        {
            var model = cx.Quizzes.Find(id);
            
            if(model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(int? id)
        {
            //Make all work for cascade delete
            var model = cx.Quizzes.Find(id);

            if(model == null)
            {
                return HttpNotFound();
            }

            var answers = cx.Answers.Where(x => x.Attempt.Quiz.Id == model.Id).ToList();
            cx.Answers.RemoveRange(answers);

            var attepmts = cx.QuizAttempts.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.QuizAttempts.RemoveRange(attepmts);

            var references = cx.QuizReferences.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.QuizReferences.RemoveRange(references);

            var resultTypes = cx.ResultTypes.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.ResultTypes.RemoveRange(resultTypes);

            var questions = cx.Questions.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.Questions.RemoveRange(questions);

            foreach(var group in model.Groups.ToList())
            {
                group.Quizs.Remove(model);
            }

            cx.Quizzes.Remove(model);

            cx.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Open quiz statistic
        /// Should generate models for view
        /// View is list of attempts (3 types  of table)
        /// Attemps has button open
        /// </summary>
        [HttpGet]
        public ActionResult QuizInfo(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            if(quiz == null)
            {
                return HttpNotFound();
            }

            var attempts = cx.QuizAttempts.Where(x => x.Quiz.Id == quiz.Id).ToList();

            ViewBag.QuizType = quiz.Type;

            return View(attempts);
        }

        /// <summary>
        /// Scroll where I can see all answers (close to testControll)
        /// </summary>
        [HttpGet]
        public ActionResult GetAttepmt(int? id)
        {
            var model = cx.QuizAttempts.Find(id);

            if(model == null)
            {
                return HttpNotFound();
            }

            var list = new List<XmlQuestion_Answer>();

            foreach(var item in model.Answers.ToList())
            {
                var question = XmlBase.Deserialize(item.Question.XmlObject, item.Question.TypeName);

                var answerName = ((IAnswerName)question).GetTypeName();

                var answer = XmlBase.Deserialize(item.XmlObject, answerName);

                list.Add(new XmlQuestion_Answer()
                {
                    Question = question,
                    Answer = answer,
                    QuestionInfo = item.Question,
                    AnswerInfo = item
                });
            }

            ViewBag.Attempt = model;

            ViewBag.Quiz = cx.Quizzes.Find(model.Quiz.Id);

            return View(list);
        }

        /// <summary>
        /// Open page for choosen question
        /// models are data for:
        /// grafics(columns % (for matching would be several grafics))
        /// </summary>
        [HttpGet]
        public ActionResult QuestionInfo(int? id)
        {
            var question = cx.Questions.Find(id);

            if(question == null)
            {
                return HttpNotFound();
            }

            //next is statistic
            var answers = cx.Answers.Where(x => x.Question.Id == question.Id).ToList();

            var list = new List<XmlBase>();

            var AnswerName = ((IAnswerName)XmlBase.Deserialize(question.XmlObject, question.TypeName)).GetTypeName();

            foreach(var item in answers)
            {
                list.Add(XmlBase.Deserialize(item.XmlObject, AnswerName));
            }

            //Make generic class to create model for grafic
            //answers, list

            return View();
        }
    }
}