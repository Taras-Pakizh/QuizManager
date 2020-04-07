using QuizManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class SectionView
    {
        public bool Next { get; set; }

        public bool Prev { get; set; }

        public int QuizId { get; set; }

        public int SectionId { get; set; }

        public List<AnswerBaseView> SectionData { get; set; }

        public List<TestView> Tests { get; set; }
    }

    public class AnswerBaseView
    {
        public int QuestionId { get; set; }

        public List<string> Answers { get; set; }
    }
}