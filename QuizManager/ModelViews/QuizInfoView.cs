using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class QuizInfoView
    {
        public Quiz Quiz { get; set; }

        public List<QuizAttempt> Attempts { get; set; }
    }
}