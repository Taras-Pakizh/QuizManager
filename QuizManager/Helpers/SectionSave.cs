using QuizManager.DBModels;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public class SectionSave
    {
        public SectionView View { get; set; }

        /// <summary>
        /// QuestionId - XmlAnswer
        /// </summary>
        public Dictionary<int, XmlBase> Answers { get; set; } 
            = new Dictionary<int, XmlBase>();

        public SectionSave(SectionView view, QuizContext cx)
        {
            View = view;

            View.Tests = new List<TestView>();

            foreach(var item in view.SectionData)
            {
                var question = cx.Questions.Find(item.QuestionId);

                var testSave = new TestView()
                {
                    Quiz = question.Quiz,
                    Question = question,
                    Model = XmlBase.Deserialize(question.XmlObject, question.TypeName),
                    Save = item
                };

                View.Tests.Add(testSave);

                var xmlQuestion = XmlBase.Deserialize(question.XmlObject, question.TypeName);

                var answerType = XmlBase.GetTypeAnswer(((IAnswerName)xmlQuestion).GetTypeName());

                var answerInstance = (XmlBase)Activator.CreateInstance(answerType);

                ((IParseAnswer)answerInstance).ParseAnswer(item.Answers);

                Answers.Add(item.QuestionId, answerInstance);
            }
        }
    }
}