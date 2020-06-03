using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuizTimeLimitType
    {
        [Description("Limited quiz time")]
        Limited,

        [Description("Nonlimited quiz time")]
        NonLimited,

        [Description("Section limited time")]
        SectionLimited,

        [Description("Question limited time")]
        QuestionLimited,
    }
}
