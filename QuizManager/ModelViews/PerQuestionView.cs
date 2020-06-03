using QuizManager.DBModels;
using QuizManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class PerQuestionView
    {
        public Question Question { get; set; }

        public AnswerBaseView QuestionData { get; set; } 

        public TestView Test { get; set; }

        public bool PrevVisibility { get; set; }

        public bool NextVisibility { get; set; }

        public bool IsFinish { get; set; }

        public int QuestionOrder { get; set; }
    }
}