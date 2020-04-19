using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class QuizSectionsView
    {
        public Quiz Quiz { get; set; }

        public IEnumerable<Section> Sections { get; set; }

        public List<int> SectionIds { get; set; }

        public Section NewSection { get; set; }
    }
}