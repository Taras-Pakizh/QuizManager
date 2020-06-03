using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuestionObligation
    {
        [Description("Fixed question")]
        Fixed,

        [Description("Random question")]
        Random
    }
}
