using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class TableView
    {
        public IEnumerable<object> data { get; set; }

        public IEnumerable<string> PropertyNames { get; set; }

        public IEnumerable<string> HeaderNames { get; set; }
    }
}