using QuizManager.DBModels;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public class TestView
    {
        public int Index { get; set; }

        public AnswerBaseView Save { get; set; }

        public Quiz Quiz { get; set; }

        public Question Question { get; set; }

        public XmlBase Model { get; set; }

        public string View
        {
            get
            {
                return _Describers.Single(x => x.Key.
                        Contains((QuestionType)Question.Type)).Value;
            }
        }

        private static readonly Dictionary<List<QuestionType>, string> _Describers = new Dictionary<List<QuestionType>, string>()
        {
            {
                new List<QuestionType>()
                {
                    QuestionType.Radio,
                    QuestionType.Checkbox,
                    QuestionType.ComboBox
                }, "ListView" 
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.Order
                }, "OrderView"
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.MatchingSingle,
                }, "MatchingSingleView"
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.MatchingMulty
                }, "MatchingMultyView"
            },
            {
                new List<QuestionType>()
                {
                    QuestionType.TextInput
                }, "TextInputView"
            }
        };
    }
}