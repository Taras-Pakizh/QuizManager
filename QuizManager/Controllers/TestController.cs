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
    [Authorize]
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

            if((result.Type == ReferenceType.Limited && 
                result.AttemptCount <= 0) ||
                result.Deadline < DateTime.Now)
            {
                cx.QuizReferences.Remove(result);

                cx.SaveChanges();

                return HttpNotFound();
            }

            if(result.Type == ReferenceType.Limited)
                result.AttemptCount--;

            cx.SaveChanges();

            Session["AttemptType"] = AttempType.ByReference;

            return RedirectToAction("TestStart", new { id = result.Quiz.Id });
        }

        /// <summary>
        /// Group allowance - redirect to quiz
        /// </summary>
        [HttpGet]
        public ActionResult GetTestAllowance(int? id)
        {
            var model = cx.GroupAllowances.Find(id);

            if(model == null)
            {
                return HttpNotFound();
            }

            var group = model.Group;

            var CurrentUser = UserManager.FindByName(User.Identity.Name);

            if (!group.ApplicationUsers.Any(x=>x.Id == CurrentUser.Id))
            {
                return HttpNotFound();
            }

            //attemptsCount check
            if(model.Type == ReferenceType.Limited)
            {
                var attempts = cx.QuizAttempts.
                    Count(x => x.Type == AttempType.ByGroup &&
                        x.Group.Id == group.Id &&
                        x.User.Id == CurrentUser.Id);

                if(attempts >= model.AttemptCount)
                {
                    Session["quizAllowance"] = "Quiz attempts is over";

                    return RedirectToAction("ViewGroup", "Group", new { id = group.Id });
                }
            }
            if(model.Deadline < DateTime.Now)
            {
                Session["quizAllowance"] = "Deadline: " + model.Deadline.ToString() + " is over";

                return RedirectToAction("ViewGroup", "Group", new { id = group.Id });
            }

            Session["AttemptType"] = AttempType.ByGroup;
            Session["GroupId"] = model.Group.Id;

            return RedirectToAction("TestStart", new { id = model.Quiz.Id });
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

            var starter = new TestStarter(quiz, cx, helper);

            Session["save"] = starter.SessionSave;

            if(quiz.TestingType == QuizTestingType.PerSection)
            {
                ViewBag.Section = starter.PerSectionModel;
            }
            else if(quiz.TestingType == QuizTestingType.PerQuestion)
            {
                ViewBag.Question = starter.PerQuestionModel;
            }

            return View(quiz);
        }

        [HttpPost]
        /// <summary>
        /// For perSection testing
        /// Saving sections in Session
        /// </summary>
        public ActionResult TestSection(SectionView view)
        {
            //saving session

            var testSave = (TestSave)Session["save"];

            view.Section = cx.Sections.Find(view.SectionId);

            testSave.Update(view);

            Session["save"] = testSave;

            //choosing action

            var quiz = cx.Quizzes.Find(view.QuizId);

            var prevSection = cx.Sections.Find(view.SectionId);

            helper.ChooseAction(prevSection, Request.Form["btnSubmit"], testSave,
                out bool isFinish, out Section section);

            //finish view
            if (isFinish)
            {
                var finishView = new FinishView()
                {
                    Quiz = quiz,
                };

                var sectionAnswersList = new List<SectionAnswersView>();

                foreach (var item in testSave.Saves)
                {
                    var sectionAnswers = new SectionAnswersView()
                    {
                        Section = cx.Sections.Find(item.Key),
                        QuestionIndex_IsInit = new Dictionary<int, bool>()
                    };

                    int index = 1;

                    foreach (var questionAnswer in item.Value.Answers)
                    {
                        sectionAnswers.QuestionIndex_IsInit.Add(
                            index,
                            ((IParseAnswer)questionAnswer.Value).IsValid()
                        );
                        index++;
                    }

                    sectionAnswersList.Add(sectionAnswers);
                }

                finishView.SectionAnswers = sectionAnswersList;

                ModelState.Clear();

                bool isEnded = false;

                if (Request.Form["btnSubmit"] == "QuizTime" || Request.Form["btnSubmit"] == "SectionTime")
                {
                    isEnded = true;
                }

                ViewBag.isEnded = isEnded;

                return PartialView("Finish", finishView);
            }

            //creating model

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
                    Section = section
                };

                var questions = helper.GetRandomSectionQuestions(section.Id);

                model.Tests = helper.CreateTestViews(questions, quiz);
            }

            model = helper.SetNavigation(model, section, quiz);

            ModelState.Clear();

            return PartialView("TestSection", model);
        }
        [HttpPost]
        public ActionResult TestQuestion(PerQuestionView view)
        {
            //saving session
            var testSave = (TestSave)Session["save"];
            view.Question = cx.Questions.Find(view.Question.Id);
            testSave.Update(view);
            Session["save"] = testSave;

            //choosing action
            helper.ChooseAction(
                view.Question, 
                Request.Form["btnSubmit"], 
                testSave,
                out bool isFinish, out Question nextQuestion);

            //finish view
            if (isFinish)
            {
                var finishView = new FinishView()
                {
                    Quiz = view.Question.Quiz,
                };

                var usedSections = new List<Section>();

                foreach(var question in testSave.QuestionSaves.
                    Select(x => cx.Questions.Find(x.Key)).
                    ToList())
                {
                    if(usedSections.Any(x=>x.Id == question.Section.Id))
                    {
                        continue;
                    }
                    usedSections.Add(question.Section);
                }

                var sectionAnswersList = new List<SectionAnswersView>();

                foreach (var section in usedSections)
                {
                    var sectionAnswers = new SectionAnswersView()
                    {
                        Section = section,
                        QuestionIndex_IsInit = new Dictionary<int, bool>()
                    };

                    int index = 1;

                    var sectionIds = cx.Questions.
                        Where(x => x.Section.Id == section.Id).
                        Select(y => y.Id).ToList();

                    foreach(var questionAnswer in testSave.QuestionSaves.
                        Where(x => sectionIds.Contains(x.Key)).ToList())
                    {
                        sectionAnswers.QuestionIndex_IsInit.Add(
                            index,
                            ((IParseAnswer)questionAnswer.Value.Answer).IsValid()
                        );
                        index++;
                    }

                    sectionAnswersList.Add(sectionAnswers);
                }

                finishView.SectionAnswers = sectionAnswersList;

                ModelState.Clear();

                bool isEnded = false;

                if (Request.Form["btnSubmit"] == "QuizTime" || 
                    Request.Form["btnSubmit"] == "QuestionTime")
                {
                    isEnded = true;
                }

                ViewBag.isEnded = isEnded;

                return PartialView("Finish", finishView);
            }

            //create model
            PerQuestionView model = null;

            if (testSave.QuestionSaves.ContainsKey(nextQuestion.Id))
            {
                model = testSave.QuestionSaves[nextQuestion.Id].View;
            }
            else
            {
                model = new PerQuestionView()
                {
                    Question = nextQuestion,
                };

                model.Test = helper.CreateTestView(nextQuestion);
            }

            model = helper.SetNavigation(model, nextQuestion, testSave);

            ModelState.Clear();

            return PartialView("TestQuestion", model);
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

            if(quiz.TestingType == QuizTestingType.PerSection)
            {
                var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).OrderBy(y => y.Order).ToList();
                var lastSection = sections.Last();
                var model = testSave.Saves[lastSection.Id].View;

                model.IsFinish = true;
                if (lastSection.Order != 1)
                {
                    model.PrevVisibility = true;
                }

                ModelState.Clear();
                return PartialView("TestSection", model);
            }
            else if(quiz.TestingType == QuizTestingType.PerQuestion)
            {
                var lastQuestionId = testSave.QuestionOrders.Last();
                var model = testSave.QuestionSaves[lastQuestionId].View;

                model.IsFinish = true;
                if(testSave.QuestionOrders.Count != 1)
                {
                    model.PrevVisibility = true;
                }

                ModelState.Clear();
                return PartialView("TestQuestion", model);
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Saving results is in Session files
        /// This action is calculating and saving to database result of quiz
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
                User = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id),
                Type = (AttempType)Session["AttemptType"],
            };

            if(attemp.Type == AttempType.ByGroup)
            {
                attemp.Group = cx.Groups.Find((int)Session["GroupId"]);
            }

            var saver = new AttemptSaver(testSave, cx, attemp, helper);

            attemp = saver.Attempt;

            return RedirectToAction("GetAttepmt", "Cabinet", new { id = attemp.Id });
        }
    }
}