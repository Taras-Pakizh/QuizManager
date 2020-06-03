using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuizType
    {
        [Description("Simple testing")]
        Test = 1,

        [Description("Adaptive testing")]
        Adaptive = 2,
    }
}
