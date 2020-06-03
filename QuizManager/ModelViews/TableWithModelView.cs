using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class TableWithModelView
    {
        public IEnumerable<object> data { get; set; }

        public IEnumerable<string> PropertyNames { get; set; }

        public IEnumerable<string> HeaderNames { get; set; }

        public IList<IList<string>> Ids { get; set; }

        public IList<string> ControllerNames { get; set; }

        public IList<string> ActionNames { get; set; }

        public IList<string> LinkContents { get; set; }
    }
}