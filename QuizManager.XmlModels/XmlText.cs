using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlText : XmlBase, IXmlTask, IAnswerName
    {
        public string Text { get; set; }

        public double Compare(XmlBase answer, double Value)
        {
            var _answer = answer as XmlTextAnswer;

            if (Text == _answer.Answer)
            {
                return Value;
            }

            return 0;
        }

        public string GetTypeName()
        {
            return (typeof(XmlTextAnswer)).Name;
        }
    }
}
