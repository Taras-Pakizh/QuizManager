using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.XmlModels;

namespace QuizManager.ModelViews
{
    public class QuizAttempView
    {
        public Quiz Quiz { get; set; }

        public QuizAttempt Attempt { get; set; }

        public List<AttempSectionView> Sections { get; set; }

    }

    public class AttempSectionView
    {
        public Section Section { get; set; }

        public List<AttempTestView> Tests { get; set; }
    }

    public class AttempTestView
    {
        public Question Question { get; set; }

        public Answer Answer { get; set; }

        public XmlBase Model { get; set; }

        public XmlBase XmlAnswer { get; set; }

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
                }, "AttempListView"
            }
        };
    }
}