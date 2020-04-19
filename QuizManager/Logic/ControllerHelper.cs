using QuizManager.DBModels;
using QuizManager.Helpers;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Logic
{
    public class ControllerHelper
    {
        private QuizContext cx;

        public ControllerHelper(QuizContext context)
        {
            cx = context;
        }

        public List<TestView> CreateTestViews(IEnumerable<Question> questions, Quiz quiz)
        {
            var testViews = new List<TestView>();

            foreach (var question in questions)
            {
                var testView = new TestView()
                {
                    Quiz = quiz,
                    Question = question,
                    Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
                };

                testViews.Add(testView);
            }

            return testViews;
        }

        public SectionView SetNavigation(SectionView model, Section section, Quiz quiz)
        {
            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).OrderBy(y => y.Order).ToList();

            if (section.Order == sections.Max(x => x.Order))
            {
                model.IsFinish = true;
                if (sections.Count != 1)
                {
                    model.PrevVisibility = true;
                }
            }
            else
            {
                model.PrevVisibility = true;
                model.NextVisibility = true;
                if (section.Order == sections.Min(x => x.Order))
                {
                    model.PrevVisibility = false;
                }
            }

            return model;
        }

        public bool IsQuizValid(int quizId, out List<string> Errors)
        {
            var isValid = true;

            Errors = new List<string>();

            var quiz = cx.Quizzes.Find(quizId);

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            foreach(var section in sections)
            {
                var isSectionValid = IsSectionValid(section.Id, out List<string> sectionErrors);

                if (!isSectionValid)
                {
                    isValid = isSectionValid;

                    Errors.AddRange(sectionErrors);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validate section:
        /// Count of question - fix count
        /// is all questions is finished
        /// </summary>
        public bool IsSectionValid(int sectionId, out List<string> Errors)
        {
            var result = true;

            var section = cx.Sections.Find(sectionId);

            Errors = new List<string>();

            var questions = cx.Questions.Where(x => x.Section.Id == section.Id).ToList();

            if(questions.Count(x=>x.Obligation == QuestionObligation.Fixed) > section.QuestionCount)
            {
                result = false;

                Errors.Add("Section № " + section.Order + ". Number of fixed questions is bigger than whole section question count");
            }

            if (questions.Count() < section.QuestionCount)
            {
                result = false;

                Errors.Add("Section № " + section.Order + ". Section has not enough questions.");
            }

            foreach (var question in questions.OrderBy(x=>x.OrderNumber))
            {
                if(question.XmlValue == null || question.Text == null)
                {
                    result = false;

                    Errors.Add("Section № " + section.Order + ". Question № " + question.OrderNumber + " not finished");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns randomly generated questions of sections
        /// </summary>
        public List<Question> GetRandomSectionQuestions(int sectionId)
        {
            var section = cx.Sections.Find(sectionId);

            var questions = cx.Questions.Where(x => x.Section.Id == section.Id).ToList();

            var fixedQuestions = questions.Where(x => x.Obligation == QuestionObligation.Fixed).ToList();

            var randomQuestions = questions.Where(x => x.Obligation == QuestionObligation.Random).ToList();

            var randomCount = section.QuestionCount - fixedQuestions.Count;

            var selectedRandom = new List<Question>();

            var random = new Random();

            for(int i = 0; i < randomCount; ++i)
            {
                var index = random.Next(0, randomQuestions.Count - 1);

                selectedRandom.Add(randomQuestions[index]);

                randomQuestions.RemoveAt(index);
            }

            fixedQuestions.AddRange(selectedRandom);

            return fixedQuestions;
        }

        public List<Question> GetRandomQuizQuestions(int quizId)
        {
            var quiz = cx.Quizzes.Find(quizId);

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            var result = new List<Question>();

            foreach(var section in sections)
            {
                result.AddRange(GetRandomSectionQuestions(section.Id));
            }

            return result;
        }
    }
}