using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class GroupLinkView
    {
        public Group Group { get; set; }

        public GroupReference Reference { get; set; }

        public string Link { get; set; }

        public bool IsRefExist { get; set; }
    }
}