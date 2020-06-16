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
using System.IO;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Microsoft.AspNet.Identity.EntityFramework;
using QuizManager.Models;

namespace QuizManager.Controllers
{
    [Authorize]
    public class CabinetController : AbstractController
    {
        public CabinetController():base()
        {
            cx = new QuizContext();

            helper = new ControllerHelper(cx);

            ViewBag.helper = helper;
        }

        [Authorize(Roles = "Admin")]
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
                    Name = "View attempts",
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
            var baseurl = Request.Url.GetLeftPart(UriPartial.Authority);

            return baseurl + "/Test/GetTest?link=" + reference.Id;

            //return "https://localhost:44339/Test/GetTest?link=" + reference.Id;
        }

        /// <summary>
        /// Generate new link and redirect to QuizLink
        /// </summary>
        [HttpPost]
        public ActionResult GetLink(QuizLinkView view)
        {
            var quiz = cx.Quizzes.Find(view.Quiz.Id);

            if(quiz == null)
            {
                return HttpNotFound();
            }

            var reference = cx.QuizReferences.Where(x => x.Quiz.Id == quiz.Id).ToList();

            if(reference.Count != 0)
            {
                cx.QuizReferences.RemoveRange(reference);
            }

            var model = new QuizReference()
            {
                Id = helper.LinkGenerator(),
                Quiz = quiz,
                Deadline = view.Reference.Deadline,
                AttemptCount = view.Reference.AttemptCount,
                Type = view.Reference.Type
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
        /// Should change to filters
        /// </summary>
        [HttpGet]
        public ActionResult QuizInfo(int? id)
        {
            var quiz = cx.Quizzes.Find(id);

            Session["filters"] = new AttemptFilters()
            {
                Page = 1,
                Quiz = quiz
            };

            return RedirectToAction("Attempts");
        }

        [HttpGet]
        /// <summary>
        /// Main page
        /// inside partial view 
        /// </summary>
        public ActionResult Attempts()
        {
            var filters = new AttemptFilters()
            {
                Page = 1
            };

            var user = cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id);

            if (Session["filters"] != null)
            {
                var savedFilters = (AttemptFilters)Session["filters"];
                if(savedFilters.CurrentUser == null ||
                    savedFilters.CurrentUser.Id == user.Id)
                {
                    filters = savedFilters;
                }
            }

            return View(filters.Filter(cx, user));
        }

        /// <summary>
        /// PartialView - table and filters
        /// </summary>
        [HttpPost]
        public ActionResult FilterAttempts(AttemptFilterView view)
        {
            view.Filters.Quiz = cx.Quizzes.Find(view.Filters.Quiz?.Id);
            view.Filters.Group = cx.Groups.Find(view.Filters.Group?.Id);
            view.Filters.User = cx.Users.Find(view.Filters.User?.Id);

            if (view.Command == "Reset")
            {
                Session["filters"] = null;

                return PartialView(new AttemptFilters()
                {
                    Page = 1
                }.Filter
                (
                    cx, 
                    cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id)
                ));
            }

            if (view.Navigation != null || view.Navigation == "")
            {
                if(view.Navigation == "Back")
                {
                    view.Filters.Page--;
                }
                else if(view.Navigation == "Next")
                {
                    view.Filters.Page++;
                }
                else if(Int32.TryParse(view.Navigation, out int page))
                {
                    view.Filters.Page = page;
                }
                else
                {
                    throw new Exception("Navigation is initialized wrong");
                }
            }
            else
            {
                view.Filters.Page = 1;
            }

            Session["filters"] = view.Filters;

            //my attempts окрема (checkbox)

            ModelState.Clear();

            return PartialView(
                view.Filters.Filter(
                    cx, 
                    cx.Users.Find(UserManager.FindByName(User.Identity.Name).Id)
           ));
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
                
                var answers = view.Answers.Where(x => x.Question != null).
                    Where(y => y.Question.Section.Id == section.Id).
                    OrderBy(z => z.Question.OrderNumber).ToList();

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

                if(sectionAttemp.Tests.Count != 0)
                {
                    model.Sections.Add(sectionAttemp);
                }
            }

            ModelState.Clear();

            return View(model);
        }


        //------------PDF----------------------------------------------------------------

        [HttpPost]
        [ValidateInput(false)]
        public void GeneratePDF(string htmlValue)
        {
            var css = PdfGenerator.ParseStyleSheet(System.IO.File.ReadAllText(Server.MapPath("~/Content/bootstrap.min.css")));
            var msPDF = PdfSharpConvert(htmlValue, css);

            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=myPdf.pdf");
            Response.BinaryWrite(msPDF.ToArray());

            Response.End();
        }

        public static MemoryStream PdfSharpConvert(string html, CssData css)
        {
            using (var file = new MemoryStream())
            {
                var pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4, 25, css);
                pdf.Save(file);
                return file;
            }
        }
    }
}