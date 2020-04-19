using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class QuizLinkView
    {
        public Quiz Quiz { get; set; }

        public QuizReference Reference { get; set; }

        public string Link { get; set; }

        public bool IsRefExists { get; set; }

        public bool IsQuizValid { get; set; }

        public List<string> Errors { get; set; }
    }
}