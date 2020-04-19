using Microsoft.AspNet.Identity;
using QuizManager.DBModels;
using QuizManager.Helpers;
using QuizManager.Logic;
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

            helper = new ControllerHelper(cx);
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

            var section = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).
                    OrderBy(y => y.Order).
                    First();

            var questions = helper.GetRandomSectionQuestions(section.Id);

            var testViews = helper.CreateTestViews(questions, quiz);

            var model = new SectionView()
            {
                QuizId = quiz.Id,
                SectionId = section.Id,
                Tests = testViews,
            };

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            model = helper.SetNavigation(model, section, quiz);

            ViewBag.Section = model;

            return View(quiz);
        }

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

            Session["save"] = testSave;

            var quiz = cx.Quizzes.Find(view.QuizId);

            var prevSection = cx.Sections.Find(view.SectionId);

            Section section = null;

            if (Request.Form["submit"] == "Next")
            {
                section = cx.Sections.Where(y=>y.Quiz.Id == quiz.Id).Single(x => x.Order == prevSection.Order + 1);
            }
            else if(Request.Form["submit"] == "Previous")
            {
                section = cx.Sections.Where(y => y.Quiz.Id == quiz.Id).Single(x => x.Order == prevSection.Order - 1);
            }
            else if(Request.Form["submit"] == "Finish")
            {
                return Finish(new FinishView() { Quiz = quiz });
            }

            SectionView model = null;

            if (testSave.Saves.ContainsKey(section.Id))
            {
                model = testSave.Saves[section.Id].View;
            }
            else
            {
                model = new SectionView()
                {
                    QuizId = quiz.Id,
                    SectionId = section.Id,
                };

                var questions = helper.GetRandomSectionQuestions(section.Id);

                model.Tests = helper.CreateTestViews(questions, quiz);
            }

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).OrderBy(y => y.Order).ToList();

            model = helper.SetNavigation(model, section, quiz);

            ModelState.Clear();

            return PartialView("TestSection", model);
        }

        /// <summary>
        /// Page for sumbiting finish of test
        /// shows list of questions and is they initialized
        /// </summary>
        [HttpPost]
        public ActionResult Finish(FinishView view)
        {
            var sectionAnswersList = new List<SectionAnswersView>();

            view.Quiz = cx.Quizzes.Find(view.Quiz.Id);

            var testSave = (TestSave)Session["save"];

            foreach(var item in testSave.Saves)
            {
                var sectionAnswers = new SectionAnswersView()
                {
                    Section = cx.Sections.Find(item.Key),
                    QuestionIndex_IsInit = new Dictionary<int, bool>()
                };

                int index = 1;

                foreach(var questionAnswer in item.Value.Answers)
                {
                    sectionAnswers.QuestionIndex_IsInit.Add(
                        index, 
                        ((IParseAnswer)questionAnswer.Value).IsValid()
                    );
                    index++;
                }

                sectionAnswersList.Add(sectionAnswers);
            }

            view.SectionAnswers = sectionAnswersList;

            ModelState.Clear();

            return PartialView("Finish", view);
        }

        [HttpPost]
        /// <summary>
        /// using for backing from Finish page
        /// </summary>
        public ActionResult UndoFinish(FinishView view)
        {
            if (Session["save"] == null)
            {
                return HttpNotFound();
            }

            var testSave = (TestSave)Session["save"];

            var quiz = cx.Quizzes.Find(view.Quiz.Id);

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).OrderBy(y => y.Order).ToList();

            var lastSection = sections.Last();

            var model = testSave.Saves[lastSection.Id].View;

            model.IsFinish = true;

            if(lastSection.Order != 1)
            {
                model.PrevVisibility = true;
            }
            
            ModelState.Clear();

            return PartialView("TestSection", model);
        }

        /// <summary>
        /// Saving results is in Session files
        /// This action is calculating and saving to database result of quiz
        /// Just for TestQuiz!!!!!!! (Poll sucks)
        /// </summary>
        [HttpGet]
        public ActionResult SaveAttemp(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            var testSave = (TestSave)Session["save"];

            quiz = cx.Quizzes.Find(quiz.Id);

            var attemp = new QuizAttempt()
            {
                Quiz = quiz,
                Time = DateTime.Now,
                User = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id)
            };

            var answers = new List<Answer>();

            foreach(var sectionSave in testSave.Saves)
            {
                foreach(var questionIdAnswers in sectionSave.Value.Answers)
                {
                    var answer = new Answer()
                    {
                        Question = cx.Questions.Find(questionIdAnswers.Key),
                        XmlObject = XmlBase.SerializeAbstract(questionIdAnswers.Value),
                    };

                    var question = cx.Questions.Find(questionIdAnswers.Key);

                    XmlBase questionXml = XmlBase.Deserialize(question.XmlObject, question.TypeName);

                    double mark = 0;

                    if (((IParseAnswer)questionIdAnswers.Value).IsValid())
                    {
                        mark = ((IXmlTask)questionXml).Compare(questionIdAnswers.Value, question.Value);
                    }

                    answer.Mark = mark;

                    answer.TypeName = ((IAnswerName)questionXml).GetTypeName();

                    answers.Add(answer);
                }
            }

            var scoredPoints = answers.Sum(x => x.Mark);

            var wholePoints = cx.Questions.Where(x => x.Quiz.Id == quiz.Id).Sum(y => y.Value);

            attemp.Mark = Math.Round(quiz.Value * (scoredPoints / wholePoints), 2);

            cx.QuizAttempts.Add(attemp);

            foreach(var item in answers)
            {
                item.Attempt = attemp;
            }

            cx.Answers.AddRange(answers);

            cx.SaveChanges();

            return RedirectToAction("GetAttepmt", "Cabinet", new { id = attemp.Id });
        }
    }
}