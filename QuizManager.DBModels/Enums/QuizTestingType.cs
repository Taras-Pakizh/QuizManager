using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuizTestingType
    {
        [Description("Testing per section")]
        PerSection,

        [Description("Testing per question")]
        PerQuestion
    }
}
