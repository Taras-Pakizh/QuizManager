using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.Helpers;
using QuizManager.ModelViews;
using QuizManager.Logic;

namespace QuizManager.Logic
{
    public class TestStarter
    {
        private QuizContext _cx;
        private Quiz _quiz;
        private ControllerHelper _helper;

        public TestStarter(Quiz quiz, QuizContext context, ControllerHelper helper)
        {
            _cx = context; 
            _quiz = quiz;
            _helper = helper;

            if(quiz.TestingType == QuizTestingType.PerSection)
            {
                PerSection();
            }
            else if(quiz.TestingType == QuizTestingType.PerQuestion)
            {
                PerQuestion();
            }
        }

        public SectionView PerSectionModel { get; set; }
        public PerQuestionView PerQuestionModel { get; set; }
        public TestSave SessionSave { get; set; }

        private void PerSection()
        {
            SectionView model = null;

            Section section = null;

            if (_quiz.Type == QuizType.Adaptive)
            {
                section = _helper.GetSection_Adaptive(_quiz, null, null, out bool isFinish, true);

                var questions = _helper.GetRandomSectionQuestions(section.Id);

                var testViews = _helper.CreateTestViews(questions, _quiz);

                model = new SectionView()
                {
                    QuizId = _quiz.Id,
                    SectionId = section.Id,
                    Tests = testViews,
                    Section = section
                };

                SessionSave = new TestSave(_cx);
            }
            else
            {
                section = _cx.Sections.Where(x => x.Quiz.Id == _quiz.Id).
                    OrderBy(y => y.Order).
                    First();

                var testSave = _helper.CreateWholeTest(_quiz);

                SessionSave = testSave;

                model = testSave.Saves[section.Id].View;
            }

            model = _helper.SetNavigation(model, section, _quiz);

            PerSectionModel = model;
        }

        private void PerQuestion()
        {
            PerQuestionView model = null;

            Question question = null;

            if (_quiz.Type == QuizType.Adaptive)
            {
                question = _helper.GetQuestion_Adaptive(_quiz, null, null, out bool isFinish, true);

                model = new PerQuestionView()
                {
                    Question = question,
                    Test = _helper.CreateTestView(question),
                    QuestionData = new AnswerBaseView()
                };

                SessionSave = new TestSave(_cx);
            }
            else
            {
                var testSave = _helper.CreateWholeTest(_quiz);

                SessionSave = testSave;

                question = _cx.Questions.Find(SessionSave.QuestionOrders[0]);

                model = testSave.QuestionSaves[question.Id].View;
            }

            model = _helper.SetNavigation(model, question, SessionSave);

            PerQuestionModel = model;
        }
    }
}