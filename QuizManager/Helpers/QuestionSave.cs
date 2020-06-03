using QuizManager.DBModels;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public class QuestionSave
    {
        public PerQuestionView View { get; set; }

        public int QuestionId { get; set; }

        public XmlBase Answer { get; set; }

        public QuestionSave() { }

        public QuestionSave(PerQuestionView view, QuizContext cx)
        {
            View = view;

            var question = cx.Questions.Find(view.Question.Id);

            QuestionId = question.Id;

            View.Test = new TestView()
            {
                Quiz = question.Quiz,
                Question = question,
                Model = XmlBase.Deserialize(question.XmlObject, question.TypeName),
                Save = view.QuestionData
            };
            
            var xmlQuestion = XmlBase.Deserialize(question.XmlObject, question.TypeName);

            var answerType = XmlBase.GetType(((IAnswerName)xmlQuestion).GetTypeName());

            var answerInstance = (XmlBase)Activator.CreateInstance(answerType);

            ((IParseAnswer)answerInstance).ParseAnswer(view.QuestionData.Answers);

            Answer = answerInstance;
        }
    }
}