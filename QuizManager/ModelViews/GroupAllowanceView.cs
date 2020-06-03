using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class GroupAllowanceView
    {
        public GroupAllowance Allowance { get; set; }

        public Group Group { get; set; }

        public List<Quiz> RestQuizzes { get; set; }

        public List<Quiz> AllowedQuizzes { get; set; }

        public List<GroupAllowance> Allowances { get; set; }
    }
}