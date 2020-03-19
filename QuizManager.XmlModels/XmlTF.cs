using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTF:XmlBase, IXmlTask, IAnswerName
    {
        public bool Answer { get; set; }

        public double Compare(XmlBase answer, double Value)
        {
            var _answer = answer as XmlTFAnswer;

            if (Answer == _answer.Answer)
            {
                return Value;
            }

            return 0;
        }

        public string GetTypeName()
        {
            return (typeof(XmlTFAnswer)).Name;
        }
    }
}
