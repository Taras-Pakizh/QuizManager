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

        private static readonly Dictionary<List<QuestionType>, Type> _Describers = new Dictionary<List<QuestionType>, Type>()
        {
            {
                new List<QuestionType>()
                {
                    QuestionType.Radio,
                    QuestionType.Checkbox,
                    QuestionType.ComboBox
                }, typeof(TestListRedactorView)
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.Order
                }, typeof(TestOrderRedactorView)
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.MatchingSingle,
                    QuestionType.MatchingMulty
                }, typeof(TestMatchingRedactorView)
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.TextInput
                }, typeof(TestTextInputRedactorView)
            }
        };

        public static RedactorView GetView(QuestionType questionType)
        {
            var type = _Describers.Single(x => x.Key.
                Contains(questionType)).Value;

            return Activator.CreateInstance(type) as RedactorView;
        }
    }
}