using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using QuizManager.Helpers;
using QuizManager.Logic;

namespace QuizManager.Controllers
{
    [Authorize]
    public class CabinetController : AbstractController
    {
        public CabinetController():base()
        {
            cx = new QuizContext();

            helper = new ControllerHelper(cx);
        }

        // GET: Cabinet
        public ActionResult Index()
        {
            ViewBag.QuizData = new List<SquereData>()
            {
                new SquereData()
                {
                    Name = "Open",
                    Link = Url.Action("Sections", "Constructor")
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

            model.IsQuizValid = helper.IsQuizValid(quiz.Id, out List<string> Errors);

            if (!model.IsQuizValid)
            {
                model.Errors = Errors;
            }

            if(reference.Count() != 0 && model.IsQuizValid)
            {
                model.Reference = reference.Single();

                model.IsRefExists = true;

                model.Link = GenerateLink(reference.Single());
            }

            return View(model);
        }

        /// <summary>
        /// Contain localhost address
        /// </summary>
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

        /// <summary>
        /// Delete Quiz
        /// </summary>
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

            var questions = cx.Questions.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.Questions.RemoveRange(questions);

            var sections = cx.Sections.Where(x => x.Quiz.Id == model.Id).ToList();
            cx.Sections.RemoveRange(sections);

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
        /// Using inner partial view for generate table of attempts
        /// Poll sucks
        /// </summary>
        [HttpGet]
        public ActionResult QuizInfo(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            var attempts = cx.QuizAttempts.Where(x => x.Quiz.Id == quiz.Id).ToList();

            var model = new QuizInfoView()
            {
                Quiz = quiz,
                Attempts = attempts
            };

            return View(model);
        }

        /// <summary>
        /// Generete page with answered quiz
        /// Shows info about QuizAttemp
        /// </summary>
        [HttpGet]
        public ActionResult GetAttepmt(int? id)
        {
            var view = cx.QuizAttempts.Find(id);

            var quiz = cx.Quizzes.Find(view.Quiz.Id);

            var model = new QuizAttempView()
            {
                Attempt = view,
                Quiz = quiz,
                Sections = new List<AttempSectionView>()
            };

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            foreach(var section in sections)
            {
                var sectionAttemp = new AttempSectionView()
                {
                    Section = section,
                    Tests = new List<AttempTestView>()
                };
                
                var answers = view.Answers.
                    Where(x => x.Question.Section.Id == section.Id).
                    OrderBy(y => y.Question.OrderNumber).ToList();

                foreach(var answer in answers)
                {
                    var question = answer.Question;

                    var testAttemp = new AttempTestView()
                    {
                        Question = question,
                        Answer = answer,
                        XmlAnswer = XmlBase.Deserialize(answer.XmlObject, answer.TypeName),
                        Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
                    };

                    sectionAttemp.Tests.Add(testAttemp);
                }

                model.Sections.Add(sectionAttemp);
            }

            return View(model);
        }
    }
}