using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.Logic;
using QuizManager.DBModels;

namespace QuizManager.ModelViews
{
    public class AttemptFilterView
    {
        public AttemptFilters Filters { get; set; }

        public IEnumerable<QuizAttempt> Data { get; set; }

        public IEnumerable<Quiz> Quizzes { get; set; }

        public IEnumerable<Group> Groups { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public string Navigation { get; set; }

        public string Command { get; set; }

        public int PagesCount { get; set; }
    }
}