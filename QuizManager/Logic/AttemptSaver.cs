using QuizManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.DBModels;
using QuizManager.XmlModels;
using QuizManager.ModelViews;

namespace QuizManager.Logic
{
    public class AttemptSaver
    {
        private QuizContext _cx;
        private TestSave _session;
        private QuizAttempt _attempt;
        private ControllerHelper _helper;

        public QuizAttempt Attempt { get; set; }

        public AttemptSaver(TestSave save, 
            QuizContext context, 
            QuizAttempt attempt,
            ControllerHelper helper)
        {
            _cx = context;
            _session = save;
            _attempt = attempt;
            _helper = helper;

            if(_attempt.Quiz.TestingType == QuizTestingType.PerSection)
            {
                PerSectionSave();
            }
            else if(_attempt.Quiz.TestingType == QuizTestingType.PerQuestion)
            {
                PerQuestionSave();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void PerSectionSave()
        {
            var answers = new List<Answer>();

            foreach (var sectionSave in _session.Saves)
            {
                foreach (var questionIdAnswers in sectionSave.Value.Answers)
                {
                    var answer = _helper.ExamineQuestion(
                        _cx.Questions.Find(questionIdAnswers.Key), questionIdAnswers.Value);

                    answers.Add(answer);
                }
            }

            _attempt.Mark = _helper.ExamineQuiz(_attempt.Quiz, answers, _session);

            _cx.QuizAttempts.Add(_attempt);
            foreach (var item in answers)
            {
                item.Attempt = _attempt;
            }
            _cx.Answers.AddRange(answers);

            _cx.SaveChanges();

            Attempt = _attempt;
        }

        private void PerQuestionSave()
        {
            var answers = new List<Answer>();

            foreach (var questionSave in _session.QuestionSaves)
            {
                var answer = _helper.ExamineQuestion(
                    _cx.Questions.Find(questionSave.Key), 
                    questionSave.Value.Answer);

                answers.Add(answer);
            }

            _attempt.Mark = _helper.ExamineQuiz(_attempt.Quiz, answers, _session);

            _cx.QuizAttempts.Add(_attempt);
            foreach (var item in answers)
            {
                item.Attempt = _attempt;
            }
            _cx.Answers.AddRange(answers);

            _cx.SaveChanges();

            Attempt = _attempt;
        }
    }
}