using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.XmlModels;
using QuizManager.DBModels;

namespace QuizManager.ModelViews
{
    public class XmlQuestion_Answer
    {
        public XmlBase Question { get; set; }

        public XmlBase Answer { get; set; }

        public Question QuestionInfo { get; set; }

        public Answer AnswerInfo { get; set; }
    }
}