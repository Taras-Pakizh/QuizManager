using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class MemberGroupView
    {
        public Group Group { get; set; }

        public List<MemberGroupAllowanceView> Allowances { get; set; }
    }
}