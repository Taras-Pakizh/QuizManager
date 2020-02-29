using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollResOption : XmlOption
    {
        public int OptionId { get; set; }

        public int Value { get; set; }
    }
}
