using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class QuestionView
    {
        public QuestionType? Type { get; set; }

        public int QuizId { get; set; }

        public int QuestionId { get; set; }

        public QuestionObligation Obligation { get; set; }

        public int Order { get; set; }
    }
}