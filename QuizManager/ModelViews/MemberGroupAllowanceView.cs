using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class MemberGroupAllowanceView
    {
        public GroupAllowance Allowance { get; set; }

        public int Attempts { get; set; }
    }
}