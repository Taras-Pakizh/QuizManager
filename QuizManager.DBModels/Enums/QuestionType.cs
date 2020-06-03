using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public enum QuestionType
    {
        [Description("Radiobuttons")]
        Radio,

        [Description("Checkboxes")]
        Checkbox,

        [Description("Comboboxes")]
        ComboBox,

        [Description("Matching - single answer")]
        MatchingSingle,

        [Description("Matching - multiple answers")]
        MatchingMulty,

        [Description("Text input")]
        TextInput,

        [Description("Ordering options")]
        Order
    }
}
