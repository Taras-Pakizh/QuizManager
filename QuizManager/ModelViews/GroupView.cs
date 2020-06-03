using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class GroupView
    {
        public Group Group { get; set; }

        public List<GroupAllowance> Allowances { get; set; }
    }
}