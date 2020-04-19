using QuizManager.DBModels;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Helpers.Models
{
    public abstract class RedactorView
    {
        public Quiz Quiz { get; set; }

        public Section Section { get; set; }

        public Question Question { get; set; }


        public abstract XmlBase Model { get; set; }

        public string View
        {
            get { return this.GetType().Name; }
        }




        private static readonly List<TypeDescriber> _Describers = new List<TypeDescriber>()
        {
            new TypeDescriber()
            {
                Names = new List<QuestionType>()
                {
                    QuestionType.Radio, 
                    QuestionType.Checkbox, 
                    QuestionType.ComboBox 
                },
                Types = new Dictionary<QuizType, Type>()
                {
                    {QuizType.Poll, typeof(PollListRedactorView)},
                    {QuizType.Test, typeof(TestListRedactorView)},
                }
            },
        };

        public static RedactorView GetView(QuestionType questionType, QuizType quizType)
        {
            var decriber = _Describers.Single(x => x.Names.Contains(questionType));

            var type = decriber.Types[quizType];

            return Activator.CreateInstance(type) as RedactorView;
        }
    }
}