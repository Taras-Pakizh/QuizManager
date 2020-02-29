using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuestionType
    {
        Radio,

        Checkbox,

        ComboBox,

        TrueFalse,

        Matching,

        TableOneChoice,

        TableCheckboxes,

        TextInput,

        Essay
    }
}
