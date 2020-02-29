using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class CabinetIndexView
    {
        public IEnumerable<Quiz> Quizs { get; set; }

        public IEnumerable<Group> MyGroups { get; set; }

        public IEnumerable<Group> MemberGroups { get; set; }

        public IEnumerable<QuizAttempt> Attemps { get; set; }

        public IEnumerable<QuizReference> QuizReferences { get; set; }

        public IEnumerable<GroupReference> GroupReferences { get; set; }
    }
}