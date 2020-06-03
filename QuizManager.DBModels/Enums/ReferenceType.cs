using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum ReferenceType
    {
        [Description("Nonlimited attempt count")]
        NonLimited,

        [Description("Limited attempt count")]
        Limited
    }
}
