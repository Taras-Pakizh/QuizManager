using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class FinishView
    {
        public Quiz Quiz { get; set; }

        public IEnumerable<SectionAnswersView> SectionAnswers { get; set; }
    }

    public class SectionAnswersView
    {
        public Section Section { get; set; }

        //Question index - bool is initialized
        public Dictionary<int, bool> QuestionIndex_IsInit { get; set; } 
    }
}