using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum AttempType
    {
        [Description("By quiz generated link")]
        ByReference,

        [Description("By group allowance")]
        ByGroup
    }
}
